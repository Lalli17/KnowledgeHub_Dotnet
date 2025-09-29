using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.DTOs;
using HarmanKnowledgeHubPortal.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleReviewController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleReviewController(IArticleService articleService, IHttpContextAccessor httpContextAccessor)
        {
            _articleService = articleService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ----------------------------
        // Admin approve/reject articles
        // ----------------------------
        [HttpPost("review")]
        public async Task<IActionResult> ReviewArticlesAsync([FromBody] ReviewArticleDto dto)
        {
            try
            {
                await _articleService.ReviewArticlesAsync(dto);
                return Ok(new { message = $"Articles {dto.Action.ToLower()}ed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingArticlesAsync()
        {
            var articles = await _articleService.GetPendingArticlesAsync(null);
            return Ok(articles);
        }

        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedArticlesAsync()
        {
            var articles = await _articleService.GetRejectedArticlesAsync();
            return Ok(articles);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitArticle([FromBody] SubmitUrlDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _articleService.SubmitArticleAsync(dto);
                return Ok(new { message = "Article submitted successfully for review." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("browse")]
        public async Task<IActionResult> BrowseArticles()
        {
            try
            {
                var articles = await _articleService.BrowseArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Rate an article
        // ----------------------------
        [HttpPost("rate/{id}")]
        public async Task<IActionResult> RateArticle(int id, [FromBody] RatingRequestDto dto)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName)) return Unauthorized("User not authenticated.");

            try
            {
                await _articleService.AddRatingAsync(id, dto.Rating, userName);
                return Ok(new { message = "Rating submitted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Add a review
        // ----------------------------
        [HttpPost("review/{id}")]
        public async Task<IActionResult> ReviewArticle(int id, [FromBody] ReviewRequestDto dto)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName)) return Unauthorized("User not authenticated.");

            try
            {
                await _articleService.AddReviewAsync(id, dto.Review, userName);
                return Ok(new { message = "Review submitted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Delete a review (Admin only)
        // ----------------------------
        [HttpDelete("review/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userRoles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            if (userRoles == null || !userRoles.Any(r => r.Value == "Admin"))
            {
                return Forbid("Only admin users can delete reviews.");
            }

            try
            {
                await _articleService.DeleteReviewAsync(id);
                return Ok(new { message = "Review deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
