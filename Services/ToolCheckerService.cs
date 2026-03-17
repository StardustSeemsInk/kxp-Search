using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// 工具检查服务
    /// </summary>
    public class ToolCheckerService
    {
        private readonly ConfigService _configService;

        public ToolCheckerService(ConfigService configService)
        {
            _configService = configService;
        }

        /// <summary>
        /// 检查 Ripgrep 是否可用
        /// </summary>
        public async Task<ToolStatus> CheckRipgrepAsync()
        {
            var status = new ToolStatus { Source = "ripgrep" };
            var config = _configService.Config.Ripgrep;

            if (!config.IsEnabled)
            {
                status.IsAvailable = false;
                status.Message = "Ripgrep is disabled in configuration";
                return status;
            }

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.ExecutablePath,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    var version = await process.StandardOutput.ReadToEndAsync();
                    status.IsAvailable = true;
                    status.Version = version.Trim();
                    status.Message = "";
                }
                else
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    status.IsAvailable = false;
                    status.Message = $"Ripgrep check failed: {error}";
                }
            }
            catch (Exception ex)
            {
                status.IsAvailable = false;
                status.Message = $"Ripgrep not found: {ex.Message}";
            }

            return status;
        }

        /// <summary>
        /// 检查 Everything(ES) 是否可用
        /// </summary>
        public async Task<ToolStatus> CheckEverythingAsync()
        {
            var status = new ToolStatus { Source = "everything(es)" };
            var config = _configService.Config.Everything;

            if (!config.IsEnabled)
            {
                status.IsAvailable = false;
                status.Message = "Everything(ES) is disabled in configuration";
                return status;
            }

            // 先检查文件是否存在
            if (!System.IO.File.Exists(config.ExecutablePath))
            {
                status.IsAvailable = false;
                status.Message = $"Everything(ES) executable not found at: {config.ExecutablePath}";
                return status;
            }

            try
            {
                // Everything(ES) 需要安装ES。
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.ExecutablePath,
                        Arguments = "-version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                // Everything(ES) -version 在成功时返回 0
                if (process.ExitCode == 0)
                {
                    var version = await process.StandardOutput.ReadToEndAsync();
                    status.IsAvailable = true;
                    // 尝试从输出中提取版本信息，如果没有输出则使用默认值
                    status.Version = string.IsNullOrWhiteSpace(version) ? "Available" : version.Trim();
                    status.Message = "";
                }
                else
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    status.IsAvailable = false;
                    status.Message = string.IsNullOrWhiteSpace(error) ? "Everything(ES) check failed" : error.Trim();
                }
            }
            catch (Exception ex)
            {
                status.IsAvailable = false;
                status.Message = $"Everything(ES) not found: {ex.Message}";
            }

            return status;
        }

        /// <summary>
        /// 检查 SearXNG 是否可用
        /// </summary>
        public async Task<ToolStatus> CheckSearxngAsync()
        {
            var status = new ToolStatus { Source = "searxng" };
            var config = _configService.Config.Searxng;

            if (!config.IsEnabled)
            {
                status.IsAvailable = false;
                status.Message = "SearXNG is disabled in configuration";
                return status;
            }

            if (string.IsNullOrEmpty(config.InstanceUrl))
            {
                status.IsAvailable = false;
                status.Message = "SearXNG instance URL is not configured";
                return status;
            }

            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);

                var url = $"{config.InstanceUrl}/healthz";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    status.IsAvailable = true;
                    status.Version = "Available";
                    status.Message = "";
                }
                else
                {
                    status.IsAvailable = false;
                    status.Message = $"SearXNG instance returned status: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                status.IsAvailable = false;
                status.Message = $"Cannot connect to SearXNG instance: {ex.Message}";
            }

            return status;
        }

        /// <summary>
        /// 检查所有工具
        /// </summary>
        public async Task<List<ToolStatus>> CheckAllToolsAsync()
        {
            var tasks = new List<Task<ToolStatus>>
            {
                CheckRipgrepAsync(),
                CheckEverythingAsync(),
                CheckSearxngAsync()
            };

            var results = await Task.WhenAll(tasks);
            return new List<ToolStatus>(results);
        }
    }
}
