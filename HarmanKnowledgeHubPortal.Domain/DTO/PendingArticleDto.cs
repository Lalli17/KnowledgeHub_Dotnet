using System;
using System.Collections.Generic;

namespace HarmanKnowledgeHubPortal.Domain.DTO
{
    public class PendingArticleDto
    {
        public IEnumerable<int> ArticleIds { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string AuthorEmail { get; set; } // Added property
    }
}