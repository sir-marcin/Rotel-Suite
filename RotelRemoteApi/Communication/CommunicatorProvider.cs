using System;

namespace RotelNetworkApi.Communication;

public static class CommunicatorProvider
{
    public static Communicator GetCommunicator(CommunicationType communicationType, DeviceConfig deviceConfig)
    {
        return communicationType switch
        {
            CommunicationType.IP => new IPCommunicator(deviceConfig),
            CommunicationType.RS232 => new RS232Communicator(deviceConfig),
            _ => throw new Exception($"Communicator type {communicationType} not supported")
        };
    }
}

public enum CommunicationType
{
    IP = 0,
    RS232 = 1
}