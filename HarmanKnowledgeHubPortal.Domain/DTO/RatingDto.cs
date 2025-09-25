namespace HarmanKnowledgeHubPortal.Domain.DTOs
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int RatingNumber { get; set; }
        public string Review { get; set; }
        public string UserName { get; set; }
        public DateTime RatedAt { get; set; }
    }
}
