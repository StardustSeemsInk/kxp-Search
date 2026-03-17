using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// SearXNG 搜索服务
    /// </summary>
    public class SearxngSearchService : ISearchService
    {
        private readonly ConfigService _configService;

        public SearxngSearchService(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task<List<SearchResultItem>> SearxngSearchAsync(SearxngRequest request)
        {
            var results = new List<SearchResultItem>();
            var config = _configService.Config.Searxng;

            if (!config.IsEnabled || string.IsNullOrEmpty(config.InstanceUrl))
            {
                return results;
            }

            try
            {
                var url = BuildSearxngUrl(request, config);
                Console.WriteLine($"Searching SearXNG with URL: {url}");
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SearXNG response: {json}");
                    results = ParseSearxngResponse(json);
                    // 如果request.MaxResults > 0，则只返回request.MaxResults个结果
                    if (request.MaxResults > 0 && results.Count > request.MaxResults)
                    {
                        results = results.GetRange(0, request.MaxResults);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SearXNG search error: {ex.Message}");
            }

            return results;
        }

        private string BuildSearxngUrl(SearxngRequest request, SearxngConfig config)
        {
            // 如果InsranceUrl没有以/结尾，确保URL正确
            var baseUrl = config.InstanceUrl.EndsWith("/") ? config.InstanceUrl : config.InstanceUrl + "/";
            var url = $"{baseUrl}search?" +
                      $"q={Uri.EscapeDataString(request.Keyword)}" +
                      $"&format=json" +
                      $"&engines={GetEngineForCategory(request.Category)}" +
                      $"&lang={request.Language}" +
                      $"&safesearch={(request.SafeSearch ? 1 : 0)}";

            return url;
        }

        private string GetEngineForCategory(string category)
        {
            return category.ToLower() switch
            {
                "academic" => "wikipedia,arxiv",
                "music" => "spotify,soundcloud",
                "video" => "youtube,digging",
                _ => "general"
            };
        }

        private List<SearchResultItem> ParseSearxngResponse(string json)
        {
            var results = new List<SearchResultItem>();

            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("results", out var resultsArray))
                {
                    foreach (var result in resultsArray.EnumerateArray())
                    {
                        var title = result.GetProperty("title").GetString() ?? "";
                        var url = result.GetProperty("url").GetString() ?? "";
                        var content = result.TryGetProperty("content", out var contentProp)
                            ? contentProp.GetString()
                            : null;
                        var engine = result.TryGetProperty("engine", out var engineProp)
                            ? engineProp.GetString()
                            : null;
                        var publishedDate = result.TryGetProperty("publishedDate", out var dateProp)
                            ? dateProp.GetString()
                            : null;

                        // 提取域名作为副标题
                        var subtitle = "";
                        try
                        {
                            var uri = new Uri(url);
                            subtitle = uri.Host;
                        }
                        catch { }

                        results.Add(new SearchResultItem
                        {
                            Source = "searxng",
                            Title = title,
                            Subtitle = subtitle,
                            Content = content,
                            Extra = new Dictionary<string, object>
                            {
                                { "engine", engine ?? "" },
                                { "publishedDate", publishedDate ?? "" }
                            },
                            Url = url
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parse SearXNG response error: {ex.Message}");
            }
            return results;
        }

        public Task<List<SearchResultItem>> RipgrepSearchAsync(RipgrepRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }

        public Task<List<SearchResultItem>> EverythingSearchAsync(EverythingRequest request)
        {
            return Task.FromResult(new List<SearchResultItem>());
        }
    }
}
