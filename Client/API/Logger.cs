using System;

namespace Client
{
    class Logger
    {
        private source _ThisDevice;
        private static bool _Enabled = false;

        public static class Actions
        {
            public const string PROBE = "PROBE";
            public const string RECONNECT = "RECONNECTING";
            public const string INFO = "INFO";
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
