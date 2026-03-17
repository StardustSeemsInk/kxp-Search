using System;
using System.Collections.Generic;

namespace kxp_Search.Models
{
    /// <summary>
    /// 工具状态
    /// </summary>
    public class ToolStatus
    {
        /// <summary>
        /// 搜索来源名称
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// 状态消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
