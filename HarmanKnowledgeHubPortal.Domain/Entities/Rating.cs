using System;
using System.ComponentModel.DataAnnotations;

namespace HarmanKnowledgeHubPortal.Domain.Entities
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int RatingNumber { get; set; }

        [MaxLength(500)]
        public string? Review { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
    }
}
