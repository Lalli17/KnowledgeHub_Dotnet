using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Entities;
using HarmanKnowledgeHubPortal.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticlesRepository _articleRepo;
        private readonly INotificationService _notificationService;

        public ArticleService(IArticlesRepository articleRepo, INotificationService notificationService)
        {
            _articleRepo = articleRepo;
            _notificationService = notificationService;
        }

        public async Task ReviewArticlesAsync(ReviewArticleDto dto)
        {
            if (dto == null || dto.ArticleIds == null || !dto.ArticleIds.Any())
                throw new ArgumentException("No articles selected for review.");

            var action = dto.Action?.ToLower();
            if (action == "approve")
            {
                await _articleRepo.ApproveAsync(dto.ArticleIds);
            }
            else if (action == "reject")
            {
                await _articleRepo.RejectAsync(dto.ArticleIds);
            }
            else
            {
                throw new Exception("Invalid action. Only 'approve' or 'reject' are allowed.");
            }
        }

        public async Task<List<ReviewArticleDto>> GetPendingArticlesAsync(int categoryId)
        {
            var articles = await _articleRepo.ReviewAsync(categoryId);
            return articles.Select(article => new ReviewArticleDto
            {
                ArticleIds = new List<int> { article.Id },
                Action = "Pending"
            }).ToList();
        }

        public async Task SubmitArticleAsync(SubmitUrlDTO dto)
        {
            var userName = "User"; // Placeholder

            var article = new Article
            {
                Title = dto.Title,
                Url = dto.Url,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                PostedBy = userName,
                DateSubmitted = DateTime.UtcNow,
                Status = ArticleStatus.PENDING
            };

            await _articleRepo.SubmitAsync(article);
        }

        public async Task<List<ArticleDto>> BrowseArticlesAsync()
        {
            // Use repository method only
            var articles = await _articleRepo.BrowseAsync();

            return articles.Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Url = a.Url,
                Description = a.Description,
                PostedBy = a.PostedBy,
                CategoryName = a.Category?.CategoryName ?? "Uncategorized",
                AverageRating = a.Ratings.Any() ? a.Ratings.Average(r => r.RatingNumber) : 0,
                RatingsCount = a.Ratings.Count,
                Reviews = a.Ratings.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    RatingNumber = r.RatingNumber,
                    Review = r.Review,
                    Name = r.User?.Name ?? "Unknown",
                    RatedAt = r.RatedAt
                }).ToList()
            }).ToList();
        }
    }
}
