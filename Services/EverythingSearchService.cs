using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// Everything(ES) 搜索服务
    /// </summary>
    public class EverythingSearchService : ISearchService
    {
        private readonly ConfigService _configService;

        public EverythingSearchService(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task<List<SearchResultItem>> EverythingSearchAsync(EverythingRequest request)
        {
            var results = new List<SearchResultItem>();
            var config = _configService.Config.Everything;

            if (!config.IsEnabled)
            {
                return results;
            }

            try
            {
                var arguments = BuildEverythingArguments(request, config);
                Console.WriteLine($"Executing Everything(es) on path: {request.Path} with arguments: {arguments}");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = config.ExecutablePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                var timeoutCts = new System.Threading.CancellationTokenSource(
                    TimeSpan.FromSeconds(config.TimeoutSeconds));

                try
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    results = ParseEverythingOutput(output);
                }
                catch (OperationCanceledException)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Everything(ES) search error: {ex.Message}");
            }

            return results;
        }

        private string BuildEverythingArguments(EverythingRequest request, EverythingConfig config)
        {
            var args = new List<string>();

            // 搜索关键词
            if (request.IsRegex)
            {
                args.Add($"-regex \"{request.Keyword}\"");
            }
            else
            {
                args.Add($"-search \"{request.Keyword}\"");
            }

            // 结果数量
            args.Add($"-max-results {request.MaxResults}");

            // 路径限制
            if (!string.IsNullOrEmpty(request.Path))
            {
                args.Add($"-path \"{request.Path}\"");
            }

            // 文件类型
            if (!string.IsNullOrEmpty(request.FileType))
            {
                args.Add($"-filetype {request.FileType}");
            }

            // 区分大小写
            if (request.IsCaseSensitive)
            {
                args.Add("-case");
            }

            // Todo: 添加额外的参数（如-p），需要配合修改request结构的定义

            return string.Join(" ", args);
        }

        private List<SearchResultItem> ParseEverythingOutput(string output)
        {
            var results = new List<SearchResultItem>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var filePath = line.Trim();
                if (string.IsNullOrEmpty(filePath)) continue;

                try
                {
                    var fileInfo = new FileInfo(filePath);
                    var fileName = fileInfo.Name;
                    var fileSize = fileInfo.Exists ? fileInfo.Length : 0L;
                    var isDirectory = !fileInfo.Exists || (fileInfo.Attributes & FileAttributes.Directory) != 0;

                    results.Add(new SearchResultItem
                    {
                        Source = "everything",
                        Title = fileName,
                        Subtitle = FormatFileSize(fileSize),
                        Content = filePath,
                        Extra = new Dictionary<string, object>
                        {
                            { "filePath", filePath },
                            { "fileSize", fileSize },
                            { "modifiedTime", fileInfo.Exists ? fileInfo.LastWriteTimeUtc.ToString("o") : "" },
                            { "isDirectory", isDirectory }
                        },
                        Url = filePath.Replace("\\", "/")
                    });
                }
                catch
                {
                    // Skip invalid paths
                }
            }

            return results;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        public Task<List<SearchResultItem>> RipgrepSearchAsync(RipgrepRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }

        public Task<List<SearchResultItem>> SearxngSearchAsync(SearxngRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }
    }
}
