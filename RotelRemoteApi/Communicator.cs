using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RotelNetworkApi.Extensions;

namespace RotelNetworkApi
{
    public class Communicator : IDisposable
    {
        private readonly byte[] ReadBufferCache = new byte[1024];
        private const int MessageIntervalMs = 500;
        private Socket _socket;
        private DeviceConfig _deviceConfig;
        private DateTime _lastMessageTime;

        private double TimeSinceLastMessage => (DateTime.Now - _lastMessageTime).TotalMilliseconds;
        public bool IsConnected => _socket?.Connected == true;

        public Communicator(DeviceConfig deviceConfig)
        {
            SetDeviceConfig(deviceConfig);
        }
        
        void IDisposable.Dispose()
        {
            Disconnect();
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }
            
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(Network.Address), Network.Port);
            
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            await _socket.ConnectAsync(ipEndPoint);
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }
            
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();
        }

        public void SetDeviceConfig(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
        }
        
        public async Task SendMessage(MessageType messageType)
        {
            if (!IsConnected)
            {
                throw new Exception("Communicator not connected");
            }
            
            var message = _deviceConfig.GetMessage(messageType);

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"No command available for {messageType} in current config ({_deviceConfig.DisplayName})");
                return;
            }
            
            var timeSinceLastMessage = TimeSinceLastMessage;
            
            if (timeSinceLastMessage < MessageIntervalMs)
            {
                var wait = (int)(MessageIntervalMs - timeSinceLastMessage);
                await Task.Delay(wait);
            }

            // Send message.
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var messageArray = new ArraySegment<byte>(messageBytes);
            await _socket.SendAsync(messageArray, SocketFlags.None);
            Console.Write($"{message}");

            // Receive ack.
            var bufferArray = new ArraySegment<byte>(ReadBufferCache);
            var received = await _socket.ReceiveAsync(bufferArray, SocketFlags.None);
            var response = Encoding.UTF8.GetString(bufferArray.Array, 0, received);
            
            Console.WriteLine($" ==> {response}");
        }
    }
}