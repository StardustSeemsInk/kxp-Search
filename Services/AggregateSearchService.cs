using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// 聚合搜索服务 - 统一入口
    /// </summary>
    public class AggregateSearchService
    {
        private readonly RipgrepSearchService _ripgrepService;
        private readonly EverythingSearchService _everythingService;
        private readonly SearxngSearchService _searxngService;
        private readonly ConfigService _configService;

        public AggregateSearchService(ConfigService configService)
        {
            _configService = configService;
            _ripgrepService = new RipgrepSearchService(configService);
            _everythingService = new EverythingSearchService(configService);
            _searxngService = new SearxngSearchService(configService);
        }

        /// <summary>
        /// Ripgrep 文本搜索
        /// </summary>
        public async Task<List<SearchResultItem>> RipgrepSearchAsync(RipgrepRequest request)
        {
            return await _ripgrepService.RipgrepSearchAsync(request);
        }

        /// <summary>
        /// Everything(ES) 文件名搜索
        /// </summary>
        public async Task<List<SearchResultItem>> EverythingSearchAsync(EverythingRequest request)
        {
            return await _everythingService.EverythingSearchAsync(request);
        }

        /// <summary>
        /// SearXNG 网络搜索
        /// </summary>
        public async Task<List<SearchResultItem>> SearxngSearchAsync(SearxngRequest request)
        {
            return await _searxngService.SearxngSearchAsync(request);
        }
    }
}
