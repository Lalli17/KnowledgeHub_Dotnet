using System.ComponentModel.DataAnnotations;

namespace HarmanKnowledgeHubPortal.Domain.DTOs
{
    public class RatingCreateDto
    {
        [Range(1, 5)]
        public int RatingNumber { get; set; }

        [MaxLength(500)]
        public string Review { get; set; }

        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
}
