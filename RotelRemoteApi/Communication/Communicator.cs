using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using RotelNetworkApi.Extensions;

namespace RotelNetworkApi.Communication
{
    public abstract class Communicator : IDisposable
    {
        private const int MessageIntervalMs = 500;
        
        private DeviceConfig _deviceConfig;
        private DateTime _lastMessageTime;

        protected double TimeSinceLastMessage => (DateTime.Now - _lastMessageTime).TotalMilliseconds;
        protected DeviceConfig DeviceConfig => _deviceConfig;
        public abstract bool IsConnected { get; }
        public event Action<string> ResponseReceived;

        public Communicator(DeviceConfig deviceConfig)
        {
            SetDeviceConfig(deviceConfig);
        }
        
        void IDisposable.Dispose()
        {
            Disconnect();
        }


        public void SetDeviceConfig(DeviceConfig deviceConfig)
        {
            _deviceConfig = deviceConfig;
        }

        protected void HandleResponseReceived(string response)
        {
            ResponseReceived?.Invoke(response);
        }

        public async Task SendMessage(MessageType messageType)
        {
            if (!IsConnected)
            {
                throw new Exception("Communicator not connected");
            }
            
            var message = DeviceConfig.GetMessage(messageType);

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"No command available for {messageType} in current config ({DeviceConfig.DisplayName})");
                return;
            }
            
            while (TimeSinceLastMessage < MessageIntervalMs)
            {
                var wait = (int)(MessageIntervalMs - TimeSinceLastMessage);
                await Task.Delay(wait);
            }

            _lastMessageTime = DateTime.Now;
            
            await SendMessage(message);
        }
        
        public abstract Task<bool> Connect();
        public abstract void Disconnect();
        protected abstract Task SendMessage(string message);
    }
}