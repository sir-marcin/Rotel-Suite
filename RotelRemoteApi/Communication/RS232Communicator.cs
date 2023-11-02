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

    public override void Dispose()
    {
        base.Dispose();
        
        _serialPort?.Dispose();
    }
    
    public override bool IsConnected => _serialPort?.IsOpen == true;
    
    public override Task<bool> Connect()
    {
        if (IsConnected)
        {
            return Task.FromResult(true);
        }

        var serialPortName = Connection.RS232.SerialPortName;
        _serialPort = new SerialPort(serialPortName, Connection.RS232.BaudRate, Connection.RS232.Parity,
            Connection.RS232.DataBits, Connection.RS232.StopBits);
        _serialPort.Handshake = Connection.RS232.Handshake;
        
        _serialPort.DataReceived += HandleDataReceived;

        try
        {
            _serialPort.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Connection error (port '{serialPortName}')");
            Console.WriteLine(e);
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var response = _serialPort.ReadLine();
            HandleResponseReceived(response);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error handling RS232 response\n{exception}");
        }
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

        return Task.CompletedTask;
    }
}