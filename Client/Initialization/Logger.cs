using System;

namespace Client
{
    class Logger
    {
        private source _ThisDevice;
        private static bool _Enabled = false;

        public static class Actions
        {
            public const string PROBE = "PROBE";               // IP + result.
            public const string RECONNECT = "RECONNECTING";    // Failed/Success.
            public const string INFO = "INFO";                 // General output.
            public const string NEW_THREAD = "SPAWN THREAD";   // New System.Threading.thread created.
            public const string PROCCESS_STATE = "PROC STATE"; // Killed/Created.
        }

        public enum source
        {
            SERVER,
            CLIENT
        }

        public Logger(source device)
        {
            _ThisDevice = device;
        }

        public void Write(string action, string message, string result = null)
        {
            if (_Enabled)
            {
                string output = string.Format("[{3}] [{0}]\t[{1}] {2}", _ThisDevice, action, message, result);
                Console.WriteLine(output);
            }
        }

        public source Device
        {
            get { return _ThisDevice; }
            set { _ThisDevice = value; }
        }

        public static bool IsEnabled 
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }


    }
}
