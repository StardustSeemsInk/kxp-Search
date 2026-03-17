using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// Ripgrep 搜索服务
    /// </summary>
    public class RipgrepSearchService : ISearchService
    {
        private readonly ConfigService _configService;

        public RipgrepSearchService(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task<List<SearchResultItem>> RipgrepSearchAsync(RipgrepRequest request)
        {
            var results = new List<SearchResultItem>();
            var config = _configService.Config.Ripgrep;

            if (!config.IsEnabled)
            {
                return results;
            }

            try
            {
                var arguments = BuildRipgrepArguments(request, config);
                Console.WriteLine($"Executing Ripgrep on path: {request.Path} or {config.DefaultPath} with arguments: {arguments}");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.ExecutablePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = request.Path ?? config.DefaultPath
                    }
                };

                process.Start();

                var timeoutCts = new System.Threading.CancellationTokenSource(
                    TimeSpan.FromSeconds(config.TimeoutSeconds));

                try
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    results = ParseRipgrepOutput(output, request.Path ?? config.DefaultPath);
                    for (var i = 0; i < results.Count; i++)
                    {
                        Console.WriteLine($"Ripgrep search result: {results[i].Title} - {results[i].Subtitle}");
                    }
                }
                catch (OperationCanceledException)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ripgrep search error: {ex.Message}");
            }

            return results;
        }

        private string BuildRipgrepArguments(RipgrepRequest request, RipgrepConfig config)
        {
            var args = new List<string>();

            // 关键词
            if (request.IsRegex)
            {
                args.Add($"-e \"{request.Keyword}\"");
            }
            else
            {
                args.Add($"\"{request.Keyword}\"");
            }

            // 最大结果数
            args.Add($"--max-count {request.MaxCount}");

            // 显示行号
            args.Add("-n");

            // 区分大小写
            if (request.IsCaseSensitive)
            {
                args.Add("-s");
            }
            else
            {
                args.Add("-i");
            }

            // 文件模式
            if (!string.IsNullOrEmpty(request.FilePattern))
            {
                args.Add($"--glob \"{request.FilePattern}\"");
            }
            else if (!string.IsNullOrEmpty(config.DefaultFilePattern))
            {
                args.Add($"--glob \"{config.DefaultFilePattern}\"");
            }

            // 上下文行数
            if (request.ContextLines > 0)
            {
                args.Add($"-C {request.ContextLines}");
            }

            return string.Join(" ", args);
        }

        private List<SearchResultItem> ParseRipgrepOutput(string output, string basePath)
        {
            var results = new List<SearchResultItem>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Ripgrep 输出格式: filePath:lineNumber:content
                var match = Regex.Match(line, @"^(.+?):(\d+):(.*)$");
                if (match.Success)
                {
                    var filePath = match.Groups[1].Value;
                    var lineNumber = int.Parse(match.Groups[2].Value);
                    var content = match.Groups[3].Value;

                    var fileName = Path.GetFileName(filePath);

                    results.Add(new SearchResultItem
                    {
                        Source = "ripgrep",
                        Title = fileName,
                        Subtitle = $"Line {lineNumber}",
                        Content = content.Trim(),
                        Extra = new Dictionary<string, object>
                        {
                            { "filePath", filePath },
                            { "lineNumber", lineNumber }
                        },
                        Url = Path.Combine(basePath, filePath).Replace("\\", "/")
                    });
                }
            }

            return results;
        }

        public Task<List<SearchResultItem>> EverythingSearchAsync(EverythingRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }

        public Task<List<SearchResultItem>> SearxngSearchAsync(SearxngRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }
    }
}
