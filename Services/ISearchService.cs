using System.Collections.Generic;
using System.Threading.Tasks;
using kxp_Search.Models;

namespace kxp_Search.Services
{
    /// <summary>
    /// 搜索服务接口
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// 使用 Ripgrep 进行文本搜索
        /// </summary>
        Task<List<SearchResultItem>> RipgrepSearchAsync(RipgrepRequest request);

        /// <summary>
        /// 使用 Everything(ES) 进行文件名搜索
        /// </summary>
        Task<List<SearchResultItem>> EverythingSearchAsync(EverythingRequest request);

        /// <summary>
        /// 使用 SearXNG 进行网络搜索
        /// </summary>
        Task<List<SearchResultItem>> SearxngSearchAsync(SearxngRequest request);
    }
}
