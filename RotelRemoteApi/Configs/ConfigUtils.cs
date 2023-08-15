using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RotelNetworkApi;

internal static class ConfigUtils
{
    private const string DefaultConfigFileName = "a14mkii.json";
    
    public static void TryGenerateDefaultConfig()
    {
        if (File.Exists(DefaultConfigFileName))
        {
            return;
        }
        
        var config = new DeviceConfig
        {
            DisplayName = "A14 MKII",
            ModelId = "a14",
            Commands = new Dictionary<int, string>()
            {
                { 0, "vol_up!" },
                { 1, "vol_dwn!" },
                { 999, "model?" },
            }
        };

        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(DefaultConfigFileName, json);
    }
}