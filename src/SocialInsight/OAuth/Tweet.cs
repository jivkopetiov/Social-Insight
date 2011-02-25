using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialInsight
{
    public class Tweet
    {
        public DateTime Date { get; set; }

        public long Id { get; set; }

        public string Text { get; set; }

        public string Source { get; set; }
    }
}
