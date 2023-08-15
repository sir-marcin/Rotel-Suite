namespace RotelNetworkApi.Extensions;

public static class DeviceConfigExtensions
{
    public static string GetMessage(this DeviceConfig config, MessageType messageType)
    {
        if (config.Commands.TryGetValue((int)messageType, out var message))
        {
            return message;
        }

        return string.Empty;
    }
}