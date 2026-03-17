using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KitX.Contract.CSharp;
using KitX.Shared.CSharp.Plugin;
using KitX.Shared.CSharp.WebCommand;
using KitX.Shared.CSharp.WebCommand.Details;
using KitX.Shared.CSharp.WebCommand.Infos;
using kxp_Search.Models;
using kxp_Search.Services;

namespace kxp_Search.Plugin;

public class Controller : IController
{
    private Action<Request>? _sendCommandAction;
    private AggregateSearchService? _searchService;
    private ConfigService? _configService;
    private ToolCheckerService? _toolCheckerService;
    // 获取dll所在目录，作为配置文件的存储位置
    private string _pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    private string? _requestId;
    private string? _pluginConnectionId;

    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public void Start()
    {
        Console.WriteLine("kxp-Search Plugin Started");
        _configService = new ConfigService(_pluginDirectory);
        _searchService = new AggregateSearchService(_configService);
        _toolCheckerService = new ToolCheckerService(_configService);
    }

    public void Pause()
    {
        Console.WriteLine("kxp-Search Plugin Paused");
    }

    public void End()
    {
        Console.WriteLine("kxp-Search Plugin Ended");
    }

    public void Execute(Command cmd)
    {
        Console.WriteLine($"Execute: {JsonSerializer.Serialize(cmd)}");
        Console.WriteLine($"Execute: {cmd.FunctionName}");

        // 从 Tags 中获取 RequestId，用于发送响应
        if (cmd.Tags is not null && cmd.Tags.TryGetValue("RequestId", out var reqId))
        {
            _requestId = reqId;
        }
        _pluginConnectionId = cmd.PluginConnectionId;

        Task.Run(async () =>
        {
            try
            {
                switch (cmd.FunctionName)
                {
                    case "RipgrepSearch":
                        await ExecuteRipgrepSearch(cmd);
                        break;
                    case "EverythingSearch":
                        await ExecuteEverythingSearch(cmd);
                        break;
                    case "SearxngSearch":
                        await ExecuteSearxngSearch(cmd);
                        break;
                    case "CheckToolAvailability":
                        await ExecuteCheckToolAvailability();
                        break;
                    case "GetConfig":
                        ExecuteGetConfig();
                        break;
                    case "SetConfig":
                        ExecuteSetConfig(cmd);
                        break;
                    default:
                        Console.WriteLine($"Unknown function: {cmd.FunctionName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing {cmd.FunctionName}: {ex.Message}");
            }
        });
    }

    private async Task ExecuteRipgrepSearch(Command cmd)
    {
        var request = new RipgrepRequest
        {
            Keyword = GetParameterValue(cmd, "keyword", ""),
            Path = GetParameterValue<string>(cmd, "path", null!),
            IsRegex = GetParameterValue(cmd, "isRegex", false),
            IsCaseSensitive = GetParameterValue(cmd, "isCaseSensitive", false),
            FilePattern = GetParameterValue<string>(cmd, "filePattern", null!),
            MaxCount = GetParameterValue(cmd, "maxCount", 10),
            ContextLines = GetParameterValue(cmd, "contextLines", 0)
        };

        var results = await _searchService!.RipgrepSearchAsync(request);
        SendResult(results);
    }

    private async Task ExecuteEverythingSearch(Command cmd)
    {
        var request = new EverythingRequest
        {
            Keyword = GetParameterValue(cmd, "keyword", ""),
            Path = GetParameterValue<string>(cmd, "path", null!),
            IsRegex = GetParameterValue(cmd, "isRegex", false),
            IsCaseSensitive = GetParameterValue(cmd, "isCaseSensitive", false),
            MaxResults = GetParameterValue(cmd, "maxResults", 100),
            FileType = GetParameterValue<string>(cmd, "fileType", null!)
        };

        var results = await _searchService!.EverythingSearchAsync(request);
        SendResult(results);
    }

    private async Task ExecuteSearxngSearch(Command cmd)
    {
        var request = new SearxngRequest
        {
            Keyword = GetParameterValue(cmd, "keyword", ""),
            Category = GetParameterValue(cmd, "category", "general"),
            MaxResults = GetParameterValue(cmd, "maxResults", 0),
            Language = GetParameterValue(cmd, "language", "zh-CN"),
            SafeSearch = GetParameterValue(cmd, "safeSearch", true)
        };

        var results = await _searchService!.SearxngSearchAsync(request);
        SendResult(results);
    }

    private async Task ExecuteCheckToolAvailability()
    {
        var results = await _toolCheckerService!.CheckAllToolsAsync();
        SendResult(results);
    }

    private void ExecuteGetConfig()
    {
        var config = _configService!.Config;
        SendResult(config);
    }

    private void ExecuteSetConfig(Command cmd)
    {
        try
        {
            var configJson = GetParameterValue(cmd, "configJson", "");
            if (string.IsNullOrWhiteSpace(configJson))
            {
                Console.WriteLine("SetConfig: Empty configJson provided");
                SendResult(false);
                return;
            }

            var newConfig = JsonSerializer.Deserialize<PluginConfig>(configJson);
            if (newConfig == null)
            {
                Console.WriteLine("SetConfig: Failed to parse configJson");
                SendResult(false);
                return;
            }

            _configService!.UpdateConfig(existingConfig =>
            {
                // Merge new config with existing config
                if (newConfig.Ripgrep != null)
                {
                    existingConfig.Ripgrep.ExecutablePath = newConfig.Ripgrep.ExecutablePath ?? existingConfig.Ripgrep.ExecutablePath;
                    existingConfig.Ripgrep.DefaultPath = newConfig.Ripgrep.DefaultPath ?? existingConfig.Ripgrep.DefaultPath;
                    existingConfig.Ripgrep.DefaultFilePattern = newConfig.Ripgrep.DefaultFilePattern ?? existingConfig.Ripgrep.DefaultFilePattern;
                    existingConfig.Ripgrep.IsEnabled = newConfig.Ripgrep.IsEnabled;
                    existingConfig.Ripgrep.TimeoutSeconds = newConfig.Ripgrep.TimeoutSeconds > 0 ? newConfig.Ripgrep.TimeoutSeconds : existingConfig.Ripgrep.TimeoutSeconds;
                }

                if (newConfig.Everything != null)
                {
                    existingConfig.Everything.ExecutablePath = newConfig.Everything.ExecutablePath ?? existingConfig.Everything.ExecutablePath;
                    existingConfig.Everything.IsEnabled = newConfig.Everything.IsEnabled;
                    existingConfig.Everything.TimeoutSeconds = newConfig.Everything.TimeoutSeconds > 0 ? newConfig.Everything.TimeoutSeconds : existingConfig.Everything.TimeoutSeconds;
                }

                if (newConfig.Searxng != null)
                {
                    existingConfig.Searxng.InstanceUrl = newConfig.Searxng.InstanceUrl ?? existingConfig.Searxng.InstanceUrl;
                    existingConfig.Searxng.ApiKey = newConfig.Searxng.ApiKey ?? existingConfig.Searxng.ApiKey;
                    existingConfig.Searxng.DefaultCategory = newConfig.Searxng.DefaultCategory ?? existingConfig.Searxng.DefaultCategory;
                    existingConfig.Searxng.IsEnabled = newConfig.Searxng.IsEnabled;
                    existingConfig.Searxng.TimeoutSeconds = newConfig.Searxng.TimeoutSeconds > 0 ? newConfig.Searxng.TimeoutSeconds : existingConfig.Searxng.TimeoutSeconds;
                }

                if (newConfig.General != null)
                {
                    existingConfig.General.MaxResultsPerSource = newConfig.General.MaxResultsPerSource > 0 ? newConfig.General.MaxResultsPerSource : existingConfig.General.MaxResultsPerSource;
                    existingConfig.General.DefaultMaxResults = newConfig.General.DefaultMaxResults > 0 ? newConfig.General.DefaultMaxResults : existingConfig.General.DefaultMaxResults;
                }
            });

            Console.WriteLine("SetConfig: Configuration updated successfully");
            SendResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetConfig error: {ex.Message}");
            SendResult(false);
        }
    }

    private T GetParameterValue<T>(Command cmd, string name, T defaultValue)
    {
        var param = cmd.FunctionArgs?.FirstOrDefault(x => x.Name == name);
        Console.WriteLine($"GetParameterValue: Looking for parameter '{name}', found: {param != null}, value: {param?.Value}, type: {param?.Value?.GetType().FullName}");
        if (param?.Value == null)
        {
            return defaultValue;
        }

        try
        {
            if (typeof(T) == typeof(string))
            {
                var stringValue = param?.Value ?? "";
                Console.WriteLine($"GetParameterValue: Returning string value for parameter '{name}': {stringValue}");
                return (T)(object)stringValue;
            }
            if (typeof(T) == typeof(int))
            {
                var stringValue = param?.Value ?? "";
                Console.WriteLine($"GetParameterValue: Attempting to parse int value for parameter '{name}' from string: {stringValue}");
                return (T)(object)Convert.ToInt32(stringValue);
            }
            if (typeof(T) == typeof(bool))
            {
                var stringValue = param?.Value ?? "";
                Console.WriteLine($"GetParameterValue: Attempting to parse int value for parameter '{name}' from string: {stringValue}");
                return (T)(object)Convert.ToBoolean(stringValue);
            }

            var json = JsonSerializer.Serialize(param.Value);
            return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"GetParameterValue: Failed to convert parameter '{name}' to type {typeof(T).FullName}. Exception: {ex.Message}");
            return defaultValue;
        }
    }

    private void SendResult(object result)
    {
        if (_requestId is null || _sendCommandAction is null)
        {
            Console.WriteLine("SendResult: No requestId or sendCommandAction, skipping response");
            return;
        }

        var resultJson = JsonSerializer.Serialize(result, serializerOptions);
        var responseBytes = Encoding.UTF8.GetBytes(resultJson);

        var responseCommand = new Command
        {
            Request = CommandRequestInfo.ReceiveCommand,
            PluginConnectionId = _pluginConnectionId ?? string.Empty,
            Body = responseBytes,
            BodyLength = responseBytes.Length,
            Tags = new Dictionary<string, string>
            {
                { "RequestId", _requestId }
            }
        };

        var responseRequest = new Request
        {
            Content = JsonSerializer.Serialize(responseCommand, serializerOptions)
        };

        _sendCommandAction(responseRequest);
    }

    public List<Function> GetFunctions() => Functions.GetFunctions();

    public void SetSendCommandAction(Action<Request> action) => _sendCommandAction = action;

    public void SetWorkingDetail(PluginWorkingDetail workingDetail)
    {
        _pluginDirectory = workingDetail.PluginSaveDirectory ?? ".";
        Console.WriteLine($"Working directory: {_pluginDirectory}");
    }
}
