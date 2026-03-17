using System;
using System.Collections.Generic;

namespace kxp_Search.Models
{
    /// <summary>
    /// 插件配置
    /// </summary>
    public class PluginConfig
    {
        /// <summary>
        /// Ripgrep 配置
        /// </summary>
        public RipgrepConfig Ripgrep { get; set; } = new();

        /// <summary>
        /// Everything 配置
        /// </summary>
        public EverythingConfig Everything { get; set; } = new();

        /// <summary>
        /// SearXNG 配置
        /// </summary>
        public SearxngConfig Searxng { get; set; } = new();

        /// <summary>
        /// 通用配置
        /// </summary>
        public GeneralConfig General { get; set; } = new();
    }

    public class RipgrepConfig
    {
        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string ExecutablePath { get; set; } = "rg";

        /// <summary>
        /// 默认搜索路径
        /// </summary>
        public string DefaultPath { get; set; } = ".";

        /// <summary>
        /// 默认文件匹配模式
        /// </summary>
        public string DefaultFilePattern { get; set; } = "*";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public int TimeoutSeconds { get; set; } = 10;
    }

    public class EverythingConfig
    {
        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string ExecutablePath { get; set; } = "Everything.exe";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public int TimeoutSeconds { get; set; } = 10;
    }

    public class SearxngConfig
    {
        /// <summary>
        /// SearXNG 实例 URL
        /// </summary>
        public string InstanceUrl { get; set; } = "";

        /// <summary>
        /// API 密钥
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// 默认分类
        /// </summary>
        public string DefaultCategory { get; set; } = "general";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public int TimeoutSeconds { get; set; } = 10;
    }

    public class GeneralConfig
    {
        /// <summary>
        /// 每个搜索源的最大结果数
        /// </summary>
        public int MaxResultsPerSource { get; set; } = 100;

        /// <summary>
        /// 默认最大结果数
        /// </summary>
        public int DefaultMaxResults { get; set; } = 20;
    }
}
