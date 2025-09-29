using HarmanKnowledgeHubPortal.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Repositories
{
    public interface IArticlesRepository
    {
        Task SubmitAsync(Article article);
        Task<List<Article>> BrowseAsync();
        Task<List<Article>> ReviewAsync(int? categoryId);
        Task<List<Article>> GetRejectedArticlesAsync();

        Task ApproveAsync(IEnumerable<int> articleIds);
        Task RejectAsync(IEnumerable<int> articleIds);

        Task AddRatingAsync(int articleId, int rating, string userName);
        Task AddReviewAsync(int articleId, string review, string userName);

        // Admin: Get & Delete review
        Task<Rating> GetReviewByIdAsync(int reviewId);
        Task DeleteReviewAsync(int reviewId);
    }
}
