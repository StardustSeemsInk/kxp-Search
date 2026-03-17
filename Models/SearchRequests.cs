using System;
using System.Collections.Generic;

namespace kxp_Search.Models
{
    /// <summary>
    /// Ripgrep 搜索请求
    /// </summary>
    public class RipgrepRequest
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 搜索路径
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 是否使用正则表达式
        /// </summary>
        public bool IsRegex { get; set; } = false;

        /// <summary>
        /// 是否区分大小写
        /// </summary>
        public bool IsCaseSensitive { get; set; } = false;

        /// <summary>
        /// 文件名匹配模式
        /// </summary>
        public string? FilePattern { get; set; }

        /// <summary>
        /// 每个文件的最大结果数
        /// </summary>
        public int MaxCount { get; set; } = 10;

        /// <summary>
        /// 上下文行数
        /// </summary>
        public int ContextLines { get; set; } = 0;
    }

    /// <summary>
    /// Everything(ES) 搜索请求
    /// </summary>
    public class EverythingRequest
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 限定搜索路径
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 是否使用正则表达式
        /// </summary>
        public bool IsRegex { get; set; } = false;

        /// <summary>
        /// 是否区分大小写
        /// </summary>
        public bool IsCaseSensitive { get; set; } = false;

        /// <summary>
        /// 最大结果数
        /// </summary>
        public int MaxResults { get; set; } = 100;

        /// <summary>
        /// 文件类型筛选 (如 txt, cs)
        /// </summary>
        public string? FileType { get; set; }
    }

    /// <summary>
    /// SearXNG 搜索请求
    /// </summary>
    public class SearxngRequest
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        /// <summary>
        /// 搜索分类 (general/academic/music/video)
        /// </summary>
        public string Category { get; set; } = "general";

        /// <summary>
        /// 最大结果数(0则为无限制)
        /// </summary>
        public int MaxResults { get; set; } = 0;

        /// <summary>
        /// 搜索语言
        /// </summary>
        public string Language { get; set; } = "zh-CN";

        /// <summary>
        /// 安全搜索
        /// </summary>
        public bool SafeSearch { get; set; } = true;
    }
}
