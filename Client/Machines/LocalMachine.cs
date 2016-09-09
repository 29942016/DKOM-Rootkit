using System;
using System.Net;

namespace Client
{
    class LocalMachine
    {
        readonly Logger _Debugger = new Logger(Logger.source.CLIENT);
        public string IP,
                      Machine_Name,
                      Operating_System,
                      Current_User,
                      Domain;

        public State Status = State.OFFLINE;

        public enum State
        {
            ONLINE,
            OFFLINE
        }

        public LocalMachine()
        {
            Machine_Name = Environment.MachineName;
            Current_User = Environment.UserName;
            Domain = Environment.UserDomainName;
            Operating_System = Environment.OSVersion.ToString();
            IP = GetPublicIP();
        }

        /// <summary>
        /// Attempts to see if we can reach a reliable host.
        /// </summary>
        public void Reconnect()
        {
            string result = (IsOnline()) ? "OK" : "FAIL";
            _Debugger.Write(Logger.Actions.PROBE, "Checking network access.", result);

            System.Threading.Thread.Sleep(1000);
        }

        #region Harvest Machine Details
        private bool IsOnline()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        Status = State.ONLINE;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Status = State.OFFLINE;
                return false;
            }
        }

        private string GetPublicIP()
        {
            string result = Status.ToString();

            if (IsOnline())
            {
                result = new WebClient().DownloadString(@"http://icanhazip.com");
            }
            return result;
        }
        #endregion
    }
}
