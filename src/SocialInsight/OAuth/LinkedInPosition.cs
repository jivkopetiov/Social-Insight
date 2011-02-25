using System;

namespace SocialInsight
{
    public class LinkedInPosition
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public bool IsCurrent { get; set; }

        public string CompanyName { get; set; }

        public string Industry { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
