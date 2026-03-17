using System;
using System.Collections.Generic;

namespace kxp_Search.Models
{
    /// <summary>
    /// 搜索结果项 - 统一数据结构
    /// </summary>
    public class SearchResultItem
    {
        /// <summary>
        /// 搜索来源: ripgrep / Everything(ES) / searxng
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 标题 (如文件名、网页标题)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 副标题 (如文件大小、行号、域名)
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// 内容摘要 (如文件内容片段、网页描述)
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 额外信息 (JSON 对象，各源特有)
        /// </summary>
        public Dictionary<string, object>? Extra { get; set; }

        /// <summary>
        /// 链接 (如网页 URL)
        /// </summary>
        public string? Url { get; set; }
    }
}
