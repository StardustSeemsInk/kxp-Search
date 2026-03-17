using System.Collections.Generic;
using KitX.Contract.CSharp;
using KitX.Contract.CSharp.Attributes;
using KitX.Shared.CSharp.Plugin;
using KitX.Shared.CSharp.WebCommand.Details;

namespace kxp_Search.Plugin;

[EntryClass]
public static class Functions
{
    public static List<Function> GetFunctions()
    {
        return new List<Function>
        {
            // RipgrepSearch
            new Function
            {
                Name = "RipgrepSearch",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "Ripgrep 文本搜索" },
                    { "en-us", "Ripgrep Text Search" }
                },
                ReturnValueType = "List<SearchResultItem>",
                Parameters = new List<Parameter>
                {
                    new() { Name = "keyword", Type = "string", IsOptional = false,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "关键词" }, { "en-us", "Keyword" } } },
                    new() { Name = "path", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "搜索路径" }, { "en-us", "Search Path" } } },
                    new() { Name = "isRegex", Type = "bool", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "正则表达式" }, { "en-us", "Regex" } } },
                    new() { Name = "isCaseSensitive", Type = "bool", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "区分大小写" }, { "en-us", "Case Sensitive" } } },
                    new() { Name = "filePattern", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "文件模式" }, { "en-us", "File Pattern" } } },
                    new() { Name = "maxCount", Type = "int", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "最大结果数" }, { "en-us", "Max Count" } } },
                    new() { Name = "contextLines", Type = "int", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "上下文行数" }, { "en-us", "Context Lines" } } }
                }
            },

            // EverythingSearch
            new Function
            {
                Name = "EverythingSearch",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "Everything(ES) 文件名搜索" },
                    { "en-us", "Everything(ES) File Search" }
                },
                ReturnValueType = "List<SearchResultItem>",
                Parameters = new List<Parameter>
                {
                    new() { Name = "keyword", Type = "string", IsOptional = false,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "关键词" }, { "en-us", "Keyword" } } },
                    new() { Name = "path", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "搜索路径" }, { "en-us", "Search Path" } } },
                    new() { Name = "isRegex", Type = "bool", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "正则表达式" }, { "en-us", "Regex" } } },
                    new() { Name = "isCaseSensitive", Type = "bool", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "区分大小写" }, { "en-us", "Case Sensitive" } } },
                    new() { Name = "maxResults", Type = "int", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "最大结果数" }, { "en-us", "Max Results" } } },
                    new() { Name = "fileType", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "文件类型" }, { "en-us", "File Type" } } }
                }
            },

            // SearxngSearch
            new Function
            {
                Name = "SearxngSearch",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "SearXNG 网络搜索" },
                    { "en-us", "SearXNG Web Search" }
                },
                ReturnValueType = "List<SearchResultItem>",
                Parameters = new List<Parameter>
                {
                    new() { Name = "keyword", Type = "string", IsOptional = false,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "关键词" }, { "en-us", "Keyword" } } },
                    new() { Name = "category", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "分类" }, { "en-us", "Category" } } },
                    new() { Name = "maxResults", Type = "int", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "最大结果数" }, { "en-us", "Max Results" } } },
                    new() { Name = "language", Type = "string", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "语言" }, { "en-us", "Language" } } },
                    new() { Name = "safeSearch", Type = "bool", IsOptional = true,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "安全搜索" }, { "en-us", "Safe Search" } } }
                }
            },

            // CheckToolAvailability
            new Function
            {
                Name = "CheckToolAvailability",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "检查工具可用性" },
                    { "en-us", "Check Tool Availability" }
                },
                ReturnValueType = "Dictionary<string, bool>",
                Parameters = new List<Parameter>()
            },

            // GetConfig
            new Function
            {
                Name = "GetConfig",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "获取配置" },
                    { "en-us", "Get Config" }
                },
                ReturnValueType = "PluginConfig",
                Parameters = new List<Parameter>()
            },

            // SetConfig
            new Function
            {
                Name = "SetConfig",
                DisplayNames = new Dictionary<string, string>
                {
                    { "zh-cn", "设置配置" },
                    { "en-us", "Set Config" }
                },
                ReturnValueType = "void",
                Parameters = new List<Parameter>
                {
                    new() { Name = "configJson", Type = "string", IsOptional = false,
                        DisplayNames = new Dictionary<string, string> { { "zh-cn", "配置 JSON" }, { "en-us", "Config JSON" } } }
                }
            }
        };
    }
}
