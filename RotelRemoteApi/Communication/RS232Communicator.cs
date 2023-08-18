using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace RotelNetworkApi.Communication;

public class RS232Communicator : Communicator
{
    private SerialPort _serialPort;
    
    public RS232Communicator(DeviceConfig deviceConfig) : base(deviceConfig)
    {
    }

    public override bool IsConnected => _serialPort?.IsOpen == true;
    
    public override async Task<bool> Connect()
    {
        if (IsConnected)
        {
            return true;
        }

        _serialPort = new SerialPort("COM1", Connection.RS232.BaudRate, Connection.RS232.Parity,
            Connection.RS232.DataBits, Connection.RS232.StopBits);
        _serialPort.Handshake = Connection.RS232.Handshake;
        
        _serialPort.DataReceived += HandleDataReceived;

        try
        {
            _serialPort.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine("Connection error");
            Console.WriteLine(e);
            return false;
        }

        var connectionTime = 0;
        var connectionCheckInterval = 100;
        
        while (!_serialPort.IsOpen && connectionTime <= Connection.RS232.ConnectTimeout)
        {
            await Task.Delay(connectionCheckInterval);
            connectionTime += connectionCheckInterval;
        }

        return _serialPort.IsOpen;
    }

    private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var response = _serialPort.ReadLine();
        HandleResponseReceived(response);
    }

    public override void Disconnect()
    {
        if (!IsConnected)
        {
            return;
        }
        
        _serialPort.Close();
    }

    protected override Task SendMessage(string message)
    {
        _serialPort.Write(message);

        return null;
    }
}