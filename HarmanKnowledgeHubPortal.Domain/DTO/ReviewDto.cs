using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.DTO
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int RatingNumber { get; set; }
        public string Review { get; set; }
        public string Name { get; set; }
        public DateTime RatedAt { get; set; }
    }

}
