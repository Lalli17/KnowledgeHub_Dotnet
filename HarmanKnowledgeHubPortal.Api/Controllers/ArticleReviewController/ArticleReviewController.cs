using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleReviewController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleReviewController(IArticleService articleService)
        {
            _articleService = articleService;
        }

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

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitArticle([FromBody] SubmitUrlDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                var approvedArticles = await _articleService.BrowseArticlesAsync();

                var result = approvedArticles.Select(u => new BrowseUrlDTO
                {
                    Title = u.Title,
                    Url = u.Url,
                    Description = u.Description,
                    PostedBy = u.PostedBy,
                    CategoryName = u.Category?.CategoryName ?? "Uncategorized"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}