using System;
using System.Text.Json;
using System.Threading.Tasks;
using kxp_Search.Models;
using kxp_Search.Services;

namespace kxp_Search.Test
{
    /// <summary>
    /// KitX 聚合搜索插件测试程序
    /// 直接调用 DLL 中的服务进行功能测试
    /// </summary>
    class Program
    {
        private static ConfigService? _configService;
        private static AggregateSearchService? _searchService;
        private static ToolCheckerService? _toolCheckerService;

        static async Task Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("  KitX Aggregate Search Plugin Test Suite");
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // 初始化服务 - 使用当前目录作为插件目录
            var pluginDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Plugin Directory: {pluginDirectory}");

            try
            {
                _configService = new ConfigService(pluginDirectory);
                Console.WriteLine("[OK] ConfigService initialized");

                _searchService = new AggregateSearchService(_configService);
                Console.WriteLine("[OK] AggregateSearchService initialized");

                _toolCheckerService = new ToolCheckerService(_configService);
                Console.WriteLine("[OK] ToolCheckerService initialized");

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to initialize services: {ex.Message}");
                return;
            }

            // 显示主菜单
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("========== Main Menu ==========");
                Console.WriteLine("1. Check Tool Availability (ToolChecker)");
                Console.WriteLine("2. Ripgrep Search Test");
                Console.WriteLine("3. Everything(ES) Search Test");
                Console.WriteLine("4. SearXNG Search Test");
                Console.WriteLine("5. View Current Config");
                Console.WriteLine("6. Update Config");
                Console.WriteLine("0. Exit");
                Console.WriteLine("==============================");
                Console.Write("Select: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await TestToolCheckerAsync();
                        break;
                    case "2":
                        await TestRipgrepSearchAsync();
                        break;
                    case "3":
                        await TestEverythingSearchAsync();
                        break;
                    case "4":
                        await TestSearxngSearchAsync();
                        break;
                    case "5":
                        ShowConfig();
                        break;
                    case "6":
                        UpdateConfig();
                        break;
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        /// <summary>
        /// 测试工具可用性检测
        /// </summary>
        static async Task TestToolCheckerAsync()
        {
            Console.WriteLine("=== Tool Availability Check ===");

            if (_toolCheckerService == null)
            {
                Console.WriteLine("[ERROR] ToolCheckerService not initialized");
                return;
            }

            var results = await _toolCheckerService.CheckAllToolsAsync();

            Console.WriteLine();
            foreach (var status in results)
            {
                Console.WriteLine($"Source: {status.Source}");
                Console.WriteLine($"  Available: {status.IsAvailable}");
                Console.WriteLine($"  Version: {status.Version}");
                Console.WriteLine($"  Message: {status.Message}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 测试 Ripgrep 搜索
        /// </summary>
        static async Task TestRipgrepSearchAsync()
        {
            Console.WriteLine("=== Ripgrep Search Test ===");

            if (_searchService == null)
            {
                Console.WriteLine("[ERROR] SearchService not initialized");
                return;
            }

            // 构建请求
            var request = new RipgrepRequest
            {
                Keyword = ReadInput("Keyword", "class"),
                Path = ReadInput("Path (empty = default)", ""),
                IsRegex = ReadBool("IsRegex (true/false)", "false"),
                IsCaseSensitive = ReadBool("IsCaseSensitive (true/false)", "false"),
                FilePattern = ReadInput("FilePattern (e.g., *.cs)", "*.cs"),
                MaxCount = ReadInt("MaxCount", "50"),
                ContextLines = ReadInt("ContextLines", "0")
            };

            if (string.IsNullOrEmpty(request.Path))
            {
                request.Path = _configService?.Config.Ripgrep.DefaultPath ?? ".";
            }

            Console.WriteLine();
            Console.WriteLine($"Searching for: {request.Keyword}");
            Console.WriteLine($"Path: {request.Path}");
            Console.WriteLine();

            var results = await _searchService.RipgrepSearchAsync(request);

            Console.WriteLine($"Found {results.Count} results:");
            foreach (var item in results)
            {
                Console.WriteLine($"  [{item.Source}] {item.Title}");
                Console.WriteLine($"    Line: {item.Subtitle}");
                Console.WriteLine($"    Content: {item.Content}");
                if (item.Extra != null && item.Extra.TryGetValue("filePath", out var fp))
                {
                    Console.WriteLine($"    Path: {fp}");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 测试 Everything(ES) 搜索
        /// </summary>
        static async Task TestEverythingSearchAsync()
        {
            Console.WriteLine("=== Everything(ES) Search Test ===");

            if (_searchService == null)
            {
                Console.WriteLine("[ERROR] SearchService not initialized");
                return;
            }

            // 构建请求
            var request = new EverythingRequest
            {
                Keyword = ReadInput("Keyword", "readme"),
                Path = ReadInput("Path (empty = all)", ""),
                IsRegex = ReadBool("IsRegex (true/false)", "false"),
                IsCaseSensitive = ReadBool("IsCaseSensitive (true/false)", "false"),
                MaxResults = ReadInt("MaxResults", "50"),
                FileType = ReadInput("FileType (e.g., md)", "")
            };

            Console.WriteLine();
            Console.WriteLine($"Searching for: {request.Keyword}");
            Console.WriteLine();

            var results = await _searchService.EverythingSearchAsync(request);

            Console.WriteLine($"Found {results.Count} results:");
            foreach (var item in results)
            {
                Console.WriteLine($"  [{item.Source}] {item.Title}");
                Console.WriteLine($"    Size: {item.Subtitle}");
                Console.WriteLine($"    Path: {item.Content}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 测试 SearXNG 搜索
        /// </summary>
        static async Task TestSearxngSearchAsync()
        {
            Console.WriteLine("=== SearXNG Search Test ===");

            if (_searchService == null)
            {
                Console.WriteLine("[ERROR] SearchService not initialized");
                return;
            }

            // 检查 SearXNG 是否配置
            if (!_configService!.Config.Searxng.IsEnabled)
            {
                Console.WriteLine("[WARNING] SearXNG is disabled in config. Enable it first.");
            }

            if (string.IsNullOrEmpty(_configService.Config.Searxng.InstanceUrl))
            {
                Console.WriteLine("[WARNING] SearXNG InstanceUrl is not configured.");
            }

            // 构建请求
            var request = new SearxngRequest
            {
                Keyword = ReadInput("Keyword", "KitX"),
                Category = ReadInput("Category (general/academic/music/video)", "general"),
                MaxResults = ReadInt("MaxResults", "10"),
                Language = ReadInput("Language (zh-CN/en-US)", "zh-CN"),
                SafeSearch = ReadBool("SafeSearch (true/false)", "true")
            };

            Console.WriteLine();
            Console.WriteLine($"Searching for: {request.Keyword}");
            Console.WriteLine($"Category: {request.Category}");
            Console.WriteLine();

            var results = await _searchService.SearxngSearchAsync(request);

            Console.WriteLine($"Found {results.Count} results:");
            foreach (var item in results)
            {
                Console.WriteLine($"  [{item.Source}] {item.Title}");
                Console.WriteLine($"    Domain: {item.Subtitle}");
                Console.WriteLine($"    Description: {item.Content}");
                Console.WriteLine($"    URL: {item.Url}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 显示当前配置
        /// </summary>
        static void ShowConfig()
        {
            Console.WriteLine("=== Current Configuration ===");
            if (_configService == null) return;

            var config = _configService.Config;
            var options = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(config, options));
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        static void UpdateConfig()
        {
            if (_configService == null)
            {
                Console.WriteLine("[ERROR] ConfigService not initialized");
                return;
            }

            Console.WriteLine("=== Update Configuration ===");
            Console.WriteLine("1. Ripgrep Executable Path");
            Console.WriteLine("2. Everything(ES) Executable Path");
            Console.WriteLine("3. SearXNG Instance URL");
            Console.WriteLine("4. Toggle Ripgrep Enabled");
            Console.WriteLine("5. Toggle Everything(ES) Enabled");
            Console.WriteLine("6. Toggle SearXNG Enabled");
            Console.Write("Select: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var rgPath = ReadInput("Ripgrep Path", _configService.Config.Ripgrep.ExecutablePath);
                    _configService.UpdateConfig(c => c.Ripgrep.ExecutablePath = rgPath);
                    Console.WriteLine("[OK] Updated");
                    break;
                case "2":
                    var evPath = ReadInput("Everything(ES) Path", _configService.Config.Everything.ExecutablePath);
                    _configService.UpdateConfig(c => c.Everything.ExecutablePath = evPath);
                    Console.WriteLine("[OK] Updated");
                    break;
                case "3":
                    var sxUrl = ReadInput("SearXNG URL", _configService.Config.Searxng.InstanceUrl);
                    _configService.UpdateConfig(c => c.Searxng.InstanceUrl = sxUrl);
                    Console.WriteLine("[OK] Updated");
                    break;
                case "4":
                    _configService.UpdateConfig(c => c.Ripgrep.IsEnabled = !c.Ripgrep.IsEnabled);
                    Console.WriteLine($"[OK] Ripgrep enabled: {_configService.Config.Ripgrep.IsEnabled}");
                    break;
                case "5":
                    _configService.UpdateConfig(c => c.Everything.IsEnabled = !c.Everything.IsEnabled);
                    Console.WriteLine($"[OK] Everything(ES) enabled: {_configService.Config.Everything.IsEnabled}");
                    break;
                case "6":
                    _configService.UpdateConfig(c => c.Searxng.IsEnabled = !c.Searxng.IsEnabled);
                    Console.WriteLine($"[OK] SearXNG enabled: {_configService.Config.Searxng.IsEnabled}");
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }

        #region Helper Methods

        static string ReadInput(string prompt, string defaultValue)
        {
            Console.Write($"{prompt}: [{defaultValue}] ");
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
        }

        static bool ReadBool(string prompt, string defaultValue)
        {
            var input = ReadInput(prompt, defaultValue);
            return bool.TryParse(input, out var result) && result;
        }

        static int ReadInt(string prompt, string defaultValue)
        {
            var input = ReadInput(prompt, defaultValue);
            return int.TryParse(input, out var result) ? result : int.Parse(defaultValue);
        }

        #endregion
    }
}
