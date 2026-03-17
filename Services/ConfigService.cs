using System;
using System.IO;
using System.Text.Json;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// 配置服务
    /// </summary>
    public class ConfigService
    {
        private readonly string _configPath;
        private PluginConfig _config;

        public PluginConfig Config => _config;

        public ConfigService(string pluginDirectory)
        {
            _configPath = Path.Combine(pluginDirectory, "config.json");
            _config = LoadConfig();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private PluginConfig LoadConfig()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                if (File.Exists(_configPath))
                {
                    Console.WriteLine($"Loading config from {_configPath}");
                    var json = File.ReadAllText(_configPath);
                    Console.WriteLine(json);
                    var config = JsonSerializer.Deserialize<PluginConfig>(json, options);
                    Console.WriteLine($"Config deserialized successfully: {config}");
                    if (config != null)
                    {
                        Console.WriteLine("Config loaded successfully");
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
            }

            // 创建默认配置
            var defaultConfig = new PluginConfig();
            SaveConfig(defaultConfig);
            return defaultConfig;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public void SaveConfig(PluginConfig config)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_configPath, json);
                _config = config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save config: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        public void UpdateConfig(Action<PluginConfig> updateAction)
        {
            updateAction(_config);
            SaveConfig(_config);
        }
    }
}
