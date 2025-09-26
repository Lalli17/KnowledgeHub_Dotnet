using System.ComponentModel.DataAnnotations;

namespace HarmanKnowledgeHubPortal.Domain.DTO
{
    public class SubmitUrlDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public List<int> TagIds { get; set; } = new();
    }
}
