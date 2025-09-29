using HarmanKnowledgeHubPortal.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Services
{
    public interface IArticleService
    {
        Task ReviewArticlesAsync(ReviewArticleDto dto);
        Task<List<PendingArticleDto>> GetPendingArticlesAsync(int? categoryId);
        Task SubmitArticleAsync(SubmitUrlDTO dto);
        Task<List<ArticleDto>> BrowseArticlesAsync();
        Task<List<PendingArticleDto>> GetRejectedArticlesAsync();

        Task AddRatingAsync(int articleId, int rating, string userName);
        Task AddReviewAsync(int articleId, string review, string userName);

        Task DeleteReviewAsync(int reviewId);


    }
}
