using System.Net.NetworkInformation;


namespace Client
{
    class Server
    {
        readonly Logger _Debugger=  new Logger(Logger.source.SERVER);
        public const string _HOST = @"59.101.1.112";

        public State Status { get; private set; }
        public enum State
        {
            OFFLINE,
            ONLINE
        }

        /// <summary>
        /// Check if we can reach the server.
        /// </summary>
        public void Probe()
        {
            try
            {
                Ping probe = new Ping();
                var result = probe.Send(_HOST);

                if (result != null)
                {
                    if (result.Status == IPStatus.Success)
                    {
                        Status = State.ONLINE;
                        _Debugger.Write(Logger.Actions.PROBE, _HOST, "OK");
                    }
                    else
                    {
                        Status = State.OFFLINE;
                        _Debugger.Write(Logger.Actions.PROBE, _HOST, "FAIL");
                    }
                }
            }
            catch (PingException e)
            {
                // No network connection.
            }

            
        }
  }
}
