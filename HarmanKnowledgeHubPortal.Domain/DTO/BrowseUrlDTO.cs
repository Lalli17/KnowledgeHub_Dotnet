using HarmanKnowledgeHubPortal.Domain.DTOs;

public class BrowseUrlDTO
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public string PostedBy { get; set; }
    public string CategoryName { get; set; }

    // New fields for ratings
    public double AverageRating { get; set; } = 0.0;
    public int RatingsCount { get; set; } = 0;
    public List<RatingDto> Reviews { get; set; } = new(); // optional, for full reviews
}
