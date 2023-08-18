using System.IO.Ports;

namespace RotelNetworkApi.Communication
{
    internal static class Connection
    {
        public static class IP
        {
            public const int Port = 9590;
            public const string Address = "192.168.0.92";
        }

        public static class RS232
        {
            public const int BaudRate = 115200;
            public const Parity Parity = System.IO.Ports.Parity.None;
            public const int DataBits = 8;
            public const StopBits StopBits = System.IO.Ports.StopBits.One;
            public const Handshake Handshake = System.IO.Ports.Handshake.None;
            public const int WriteTimeout = 500;
            public const int ConnectTimeout = 1000;
        }
    }
}