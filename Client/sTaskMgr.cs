using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Client.API;

namespace Client
{
    struct sTaskMgr
    {
            // Main window
            private IntPtr _lhWndParent;
            // UI Controls, Mainwindow, mainpanel, ListView, ListViewHeader.
            private IntPtr _lhTaskManagerMain;
            private IntPtr _lhDirectUIHWND;
            private IntPtr _lhSysListView32;
            private IntPtr _lhSysHeader32;

            // Task Manager Menus
            private IntPtr _hMenu;
            private IntPtr _hSubMenu;
            private IntPtr _hSubSubMenu;

            // Menu refresh options
            private List<int> _mRefreshOptions;
            public enum RefreshRates
            {
                High = 0,
                Normal = 1,
                Slow = 2,
                Stop = 3
            }

            // Refresh now button
            private int _refreshNowButton;

            private List<Process> _processChain;

            public sTaskMgr(IntPtr TaskManagerParentAddress)
            {
                _lhWndParent = TaskManagerParentAddress;

                _lhTaskManagerMain = new IntPtr();
                _lhDirectUIHWND = new IntPtr();
                _lhSysListView32 = new IntPtr();
                _lhSysHeader32 = new IntPtr();

                _hMenu = new IntPtr();
                _hSubMenu = new IntPtr();
                _hSubSubMenu = new IntPtr();

                _refreshNowButton = 0;
                _mRefreshOptions = new List<int>();
                _processChain = Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();

                PopulateOffsets();
                OutputOffsets();
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
                if (index == -1)
                {
                    Console.WriteLine("Can't find process.");
                    return;
                }

                User32.PostMessage(_lhSysListView32, User32.LVM_DELETEITEM, index, 0); 
                _processChain.RemoveAt(index);
            }

            public void DisableManualRefresh(bool disable)
            {
                User32.EnableMenuItem(_hMenu, (uint)_refreshNowButton,
                    disable ? User32.MF_GRAYED : User32.MF_ENABLED);
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
                    User32.EnableMenuItem(_hMenu, (uint)b, disable ? User32.MF_GRAYED : User32.MF_ENABLED);
            }


        public int GetProcessIndex(string procName)
        {

            foreach (Process p in _processChain)
            {
                if (p.ProcessName.ToLower().Contains(procName))
                {
                    return _processChain.IndexOf(p);
                }
            }

            return -1;
        }

        private void PopulateOffsets()
            {
                #region Window Panel

                do
                {
                    _lhTaskManagerMain = User32.FindWindowEx(_lhWndParent, _lhTaskManagerMain, null, null);
                    _lhDirectUIHWND = User32.FindWindowEx(_lhTaskManagerMain, _lhDirectUIHWND, null, null);

                    for (int i = 0; i < 7; i++)
                    {
                        _lhSysListView32 = User32.FindWindowEx(_lhDirectUIHWND, _lhSysListView32, null, null);

                        if (User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                        {
                            Console.WriteLine(" Found tasklist window at 0x" + _lhSysListView32.ToString("X"));
                            lhSysListView32 = User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysListView32",
                                null);
                            lhSysHeader32 = User32.FindWindowEx(_lhSysListView32, IntPtr.Zero, "SysHeader32", null);
                            break;
                        }
                    }
                    Thread.Sleep(100);
                } while (_lhSysHeader32 == IntPtr.Zero);

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

            private void OutputOffsets()
            {
                Console.WriteLine("\n\t[Task Manager Instance Offsets]");
                Console.WriteLine("\t lhWndParent:\t\t0x" + lhWndParent.ToString("X"));
                Console.WriteLine("\t lhTaskManagerMain:\t0x" + lhTaskManagerMain.ToString("X"));
                Console.WriteLine("\t lhDirectUIHWND:\t0x" + lhDirectUIHWND.ToString("X"));
                Console.WriteLine("\t lhSysListView32:\t0x" + lhSysListView32.ToString("X"));
                Console.WriteLine("\t lhSysHeader32:\t\t0x" + lhSysHeader32.ToString("X"));
                Console.WriteLine("\n");
            }

            public IntPtr lhWndParent
            {
                get { return _lhWndParent; }
                set { _lhWndParent = value; }
            }
            public IntPtr lhTaskManagerMain
            {
                get { return _lhTaskManagerMain; }
                set { _lhTaskManagerMain = value; }
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

    }
