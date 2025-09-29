using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Entities;
using HarmanKnowledgeHubPortal.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticlesRepository _articleRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleService(IArticlesRepository articleRepo, IHttpContextAccessor httpContextAccessor)
        {
            _articleRepo = articleRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        // ----------------------------
        // Admin approve/reject articles
        // ----------------------------
        public async Task ReviewArticlesAsync(ReviewArticleDto dto)
        {
            if (dto.Action?.ToLower() == "approve")
                await _articleRepo.ApproveAsync(dto.ArticleIds);
            else if (dto.Action?.ToLower() == "reject")
                await _articleRepo.RejectAsync(dto.ArticleIds);
            else
                throw new Exception("Invalid action.");
        }

        public async Task<List<PendingArticleDto>> GetPendingArticlesAsync(int? categoryId)
        {
            var articles = await _articleRepo.ReviewAsync(categoryId);
            return articles.Select(a => new PendingArticleDto
            {
                ArticleIds = new[] { a.Id },
                Title = a.Title,
                Url = a.Url,
                CategoryName = a.Category?.CategoryName ?? "Uncategorized",
                DateSubmitted = a.DateSubmitted
            }).ToList();
        }

        public async Task<List<PendingArticleDto>> GetRejectedArticlesAsync()
        {
            var articles = await _articleRepo.GetRejectedArticlesAsync();
            return articles.Select(a => new PendingArticleDto
            {
                ArticleIds = new[] { a.Id },
                Title = a.Title,
                Url = a.Url,
                CategoryName = a.Category?.CategoryName ?? "Uncategorized",
                DateSubmitted = a.DateSubmitted
            }).ToList();
        }

        // ----------------------------
        // User submits a new article
        // ----------------------------
        public async Task SubmitArticleAsync(SubmitUrlDTO dto)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName)) throw new Exception("User not authenticated.");

            await _articleRepo.SubmitAsync(new Article
            {
                Title = dto.Title,
                Url = dto.Url,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                PostedBy = userName,
                Status = ArticleStatus.PENDING,
                DateSubmitted = DateTime.UtcNow
            });
        }

        // ----------------------------
        // Browse articles with ratings & reviews
        // ----------------------------
        public async Task<List<ArticleDto>> BrowseArticlesAsync()
        {
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
                Reviews = a.Ratings
                    .Where(r => !string.IsNullOrEmpty(r.Review))
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        RatingNumber = r.RatingNumber,
                        Review = r.Review,
                        Name = r.User?.Name ?? "Unknown",
                        RatedAt = r.RatedAt
                    }).ToList()
            }).ToList();
        }

        // ----------------------------
        // Add a rating
        // ----------------------------
        public async Task AddRatingAsync(int articleId, int rating, string userName)
        {
            await _articleRepo.AddRatingAsync(articleId, rating, userName);
        }

        // ----------------------------
        // Add a review
        // ----------------------------
        public async Task AddReviewAsync(int articleId, string review, string userName)
        {
            await _articleRepo.AddReviewAsync(articleId, review, userName);
        }

        // ----------------------------
        // Delete a review (Admin only)
        // ----------------------------
        public async Task DeleteReviewAsync(int reviewId)
        {
            // Check if current user is Admin
            var userRoles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            if (userRoles == null || !userRoles.Any(r => r.Value == "Admin"))
                throw new UnauthorizedAccessException("Only admin users can delete reviews.");

            // Delete review from repository
            var review = await _articleRepo.GetReviewByIdAsync(reviewId);
            if (review == null)
                throw new Exception("Review not found.");

            await _articleRepo.DeleteReviewAsync(reviewId);
        }
    }
}
