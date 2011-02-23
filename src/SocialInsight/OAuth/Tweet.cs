using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialNetworkAPIs.OAuth
{
    public class Tweet
    {
        public DateTime Date { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public string Source { get; set; }
    }
}
