using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RotelNetworkApi;

public class ConfigsManager
{
    private const string ConfigsPath = "/";

    private Dictionary<string, DeviceConfig> _configs = new();
    
    public void LoadConfigs()
    {
        ConfigUtils.TryGenerateDefaultConfig();
        
        var jsonFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.TopDirectoryOnly);

        foreach (var jsonFilePath in jsonFilePaths)
        {
            var jsonContent = File.ReadAllText(jsonFilePath);
            var config = JsonConvert.DeserializeObject<DeviceConfig>(jsonContent);

            if (config != null)
            {
                _configs.Add(config.ModelId, config);
            }
        }
    }

    public bool TryGetConfig(string modelId, out DeviceConfig deviceConfig)
    {
        return _configs.TryGetValue(modelId, out deviceConfig);
    }
}