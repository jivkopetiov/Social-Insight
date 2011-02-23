using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocialNetworkAPIs
{
    public class TwitterUser
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string ScreenName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri ProfileImage { get; set; }

        public bool GeoEnabled { get; set; }

        public int FriendsCount { get; set; }

        public int FollowersCount { get; set; }

        public string Language { get; set; }
    }
}
