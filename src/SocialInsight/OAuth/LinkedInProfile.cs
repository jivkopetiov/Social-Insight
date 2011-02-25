using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialInsight
{
    public class LinkedInProfile
    {
        public LinkedInProfile()
        {
            Positions = new List<LinkedInPosition>();
        }

        public List<LinkedInPosition> Positions { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public Uri Picture { get; set; }

        public string Headline { get; set; }

        public string Country { get; set; }

        public string Industry { get; set; }

        public int Distance { get; set; }

        public string Location { get; set; }

        public string CurrentShare { get; set; }

        public string CurrentStatusDate { get; set; }

        public string CurrentStatus { get; set; }

        public int NumberOfConnections { get; set; }

        public string Summary { get; set; }

        public string Specialties { get; set; }

        public string Interests { get; set; }

        public string Company
        {
            get
            {
                if (Positions.Any())
                    return Positions.First().CompanyName;
                else
                    return "n/a";
            }
        }
    }
}
