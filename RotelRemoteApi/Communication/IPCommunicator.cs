using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RotelNetworkApi.Communication;
using RotelNetworkApi.Extensions;

namespace RotelNetworkApi;

public class IPCommunicator : Communicator
{
    private readonly byte[] ReadBufferCache = new byte[1024];
    private Socket _socket;
    
    public override bool IsConnected => _socket?.Connected == true;
    
    public IPCommunicator(DeviceConfig deviceConfig) : base(deviceConfig)
    {
    }
    
    public override async Task<bool> Connect()
        {
            if (IsConnected)
            {
                return true;
            }
            
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(Connection.IP.Address), Connection.IP.Port);
            
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await _socket.ConnectAsync(ipEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error");
                Console.WriteLine(e);
                return false;
            }
            
            return _socket.Connected;
        }

        public override void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }
            
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();
        }

        protected override async Task SendMessage(string message)
        {
            // Send message.
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var messageArray = new ArraySegment<byte>(messageBytes);
            await _socket.SendAsync(messageArray, SocketFlags.None);
            Console.Write($"{message}");

            // Receive ack.
            var bufferArray = new ArraySegment<byte>(ReadBufferCache);
            var received = await _socket.ReceiveAsync(bufferArray, SocketFlags.None);
            var response = Encoding.UTF8.GetString(bufferArray.Array, 0, received);
            
            HandleResponseReceived(response);
            
            //TODO: move this to Program class
            Console.WriteLine($" ==> {response}");
        }
}