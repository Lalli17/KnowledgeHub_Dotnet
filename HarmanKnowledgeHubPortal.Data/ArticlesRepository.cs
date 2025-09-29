using HarmanKnowledgeHubPortal.Data;
using HarmanKnowledgeHubPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Repositories
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly AppDbContext _context;

        public ArticlesRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------- Articles ----------
        public async Task SubmitAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> BrowseAsync()
        {
            return await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Ratings)
                .ThenInclude(r => r.User)
                .Where(a => a.Status == ArticleStatus.APPROVED)
                .ToListAsync();
        }

        public async Task<List<Article>> ReviewAsync(int? categoryId)
        {
            var query = _context.Articles
                .Include(a => a.Category)
                .Where(a => a.Status == ArticleStatus.PENDING);

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Article>> GetRejectedArticlesAsync()
        {
            return await _context.Articles
                .Include(a => a.Category)
                .Where(a => a.Status == ArticleStatus.REJECTED)
                .ToListAsync();
        }

        public async Task ApproveAsync(IEnumerable<int> articleIds)
        {
            var articles = await _context.Articles.Where(a => articleIds.Contains(a.Id)).ToListAsync();
            foreach (var a in articles) a.Status = ArticleStatus.APPROVED;
            await _context.SaveChangesAsync();
        }

        public async Task RejectAsync(IEnumerable<int> articleIds)
        {
            var articles = await _context.Articles.Where(a => articleIds.Contains(a.Id)).ToListAsync();
            foreach (var a in articles) a.Status = ArticleStatus.REJECTED;
            await _context.SaveChangesAsync();
        }

        // ---------- Ratings & Reviews ----------
        public async Task AddRatingAsync(int articleId, int rating, string userName)
        {
            var article = await _context.Articles.FindAsync(articleId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            if (article == null || user == null) return;

            _context.Ratings.Add(new Rating
            {
                ArticleId = articleId,
                RatingNumber = rating,
                UserId = user.Id,
                User = user
            });

            await _context.SaveChangesAsync();
        }

        public async Task AddReviewAsync(int articleId, string review, string userName)
        {
            var article = await _context.Articles.FindAsync(articleId);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            if (article == null || user == null) return;

            _context.Ratings.Add(new Rating
            {
                ArticleId = articleId,
                Review = review,
                UserId = user.Id,
                User = user
            });

            await _context.SaveChangesAsync();
        }

        // ---------- Admin: Get & Delete review ----------
        public async Task<Rating> GetReviewByIdAsync(int reviewId)
        {
            var review = await _context.Ratings
                .Include(r => r.User)
                .Include(r => r.Article)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            return review;
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Ratings.FindAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            _context.Ratings.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
