using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Services
{
    public interface IArticleService
    {
        Task ReviewArticlesAsync(ReviewArticleDto dto);
        Task<List<PendingArticleDto>> GetPendingArticlesAsync(int? categoryId);
        Task SubmitArticleAsync(SubmitUrlDTO dto);
        Task<List<Article>> BrowseArticlesAsync();
    }
}