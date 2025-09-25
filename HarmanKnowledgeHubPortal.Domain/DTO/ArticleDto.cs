using System;
using System.Collections.Generic;

namespace HarmanKnowledgeHubPortal.Domain.DTO
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }         // ✅ instead of Content
        public string Description { get; set; } // ✅ add description
        public string PostedBy { get; set; }    // ✅ who submitted the article
        public string CategoryName { get; set; }
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public List<ReviewDto> Reviews { get; set; }
    }
}
