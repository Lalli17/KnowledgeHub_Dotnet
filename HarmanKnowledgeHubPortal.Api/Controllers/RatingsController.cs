using HarmanKnowledgeHubPortal.Data;
using HarmanKnowledgeHubPortal.Domain.Entities;
using HarmanKnowledgeHubPortal.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly AppDbContext _context;
    public RatingsController(AppDbContext context) => _context = context;

    // GET all ratings for an article
    [HttpGet("article/{articleId}")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetByArticle(int articleId)
    {
        var ratings = await _context.Ratings
            .Include(r => r.User)
            .Where(r => r.ArticleId == articleId)
            .OrderByDescending(r => r.RatedAt)
            .Select(r => new RatingDto
            {
                Id = r.Id,
                RatingNumber = r.RatingNumber,
                Review = r.Review,
                UserName = r.User.Name,
                RatedAt = r.RatedAt
            })
            .ToListAsync();

        return Ok(ratings);
    }

    // GET average rating for an article
    [HttpGet("average/{articleId}")]
    public async Task<ActionResult<double>> GetAverage(int articleId)
    {
        var avg = await _context.Ratings
            .Where(r => r.ArticleId == articleId)
            .AverageAsync(r => (double?)r.RatingNumber) ?? 0.0;

        return Ok(avg);
    }

    // POST: api/ratings
    [HttpPost]
    public async Task<ActionResult<RatingDto>> Create([FromBody] RatingCreateDto dto)
    {
        var rating = new Rating
        {
            RatingNumber = dto.RatingNumber,
            Review = dto.Review,
            UserId = dto.UserId,
            ArticleId = dto.ArticleId,
            RatedAt = DateTime.UtcNow
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(rating.UserId);

        var result = new RatingDto
        {
            Id = rating.Id,
            RatingNumber = rating.RatingNumber,
            Review = rating.Review,
            UserName = user?.Name ?? "Unknown",
            RatedAt = rating.RatedAt
        };

        return CreatedAtAction(nameof(GetByArticle), new { articleId = rating.ArticleId }, result);
    }

}
