using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Abilitics.SearchPoint.Engine.Infrastructure;
using Abilitics.SearchPoint.Engine.OAuth;
using System.Net;
using System.IO;
using SocialNetworkAPIs;
using SocialNetworkAPIs.OAuth;

namespace Abilitics.SearchPoint.Engine.LinkedIn
{
    public class TwitterService
    {
        private OAuthClient _client;

        public TwitterService(string consumerKey, string consumerSecret)
        {
            _client = new OAuthClient(consumerKey, consumerSecret);

            _client.RequestTokenServiceUrl = "https://api.twitter.com/oauth/request_token";
            _client.AuthorizeServiceUrl = "https://api.twitter.com/oauth/authorize";
            _client.AccessTokenServiceUrl = "https://api.twitter.com/oauth/access_token";
            _client.Realm = "";
            _client.CallbackUrl = "http://jivkopetiov.com";
        }

        public List<TwitterUser> GetFriends(string username)
        {
            var parameters = new Dictionary<string, string> {
                { "screen_name" , username }
            };
            var xml = _client.APIWebRequest(HttpMethod.GET, "http://api.twitter.com/1/friends/ids.xml", parameters);

            var ids = new List<int>();
            foreach (var xmlElement in xml.Root.Elements("id"))
            {
                int id = int.Parse(xmlElement.Value);
                ids.Add(id);
            }

            var users = GetUsersByIds(ids);
            return users;
        }

        public List<TwitterUser> GetFollowers(string username)
        {
            var parameters = new Dictionary<string, string> {
                { "screen_name" , username }
            };
            var xml = _client.APIWebRequest(HttpMethod.GET, "http://api.twitter.com/1/followers/ids.xml", parameters);

            var ids = new List<int>();
            foreach (var xmlElement in xml.Root.Elements("id"))
            {
                int id = int.Parse(xmlElement.Value);
                ids.Add(id);
            }

            if (ids.Count == 0)
                return new List<TwitterUser>();

            var users = GetUsersByIds(ids);
            return users;
        }

        public List<TwitterUser> GetUsersByIds(List<int> ids)
        {
            string userIds = ids.JoinStrings(",");

            var xml = _client.APIWebRequest(HttpMethod.GET, "http://api.twitter.com/1/users/lookup.xml?user_id=" + userIds, null);

            var users = new List<TwitterUser>();
            foreach (var userXml in xml.Root.Elements("user"))
                users.Add(BuildTwitterUserFromXml(userXml));

            return users;
        }

        private static TwitterUser BuildTwitterUserFromXml(XElement xml)
        {
            var user = new TwitterUser();
            user.Id = xml.GetSubElementNumber("id");
            user.ScreenName = xml.GetSubElementValue("screen_name");
            user.Location = xml.GetSubElementValue("location");
            user.Name = xml.GetSubElementValue("name");
            user.Description = xml.GetSubElementValue("description");
            user.ProfileImage = xml.GetSubElementUrl("profile_image_url");
            user.FollowersCount = xml.GetSubElementNumber("followers_count");
            user.FriendsCount = xml.GetSubElementNumber("friends_count");
            user.GeoEnabled = Convert.ToBoolean(xml.GetSubElementValue("geo_enabled"));
            user.Language = xml.GetSubElementValue("lang");

            return user;
        }

        public void VerifyLogin()
        {
            _client.APIWebRequest(HttpMethod.GET, "http://api.twitter.com/version/account/verify_credentials.xml", null);
        }

        public void Retweet(int tweet)
        {
            _client.APIWebRequest(HttpMethod.POST, "http://api.twitter.com/1/statuses/retweet/{0}.xml?trim_user=true".Fmt(tweet), null);
        }

        public string GetRequestToken()
        {
            return _client.AuthorizationLinkGet();
        }

        public Tuple<string, string> GetAccessToken(string requestToken)
        {
            _client.AccessTokenGet(requestToken);
            return Tuple.Create(_client.Token, _client.TokenSecret);
        }

        public void SetVerifier(string verifier)
        {
            _client.Verifier = verifier;
        }

        public List<Tweet> GetHomeTimeline()
        {
            var xml = _client.APIWebRequest(HttpMethod.GET, "http://api.twitter.com/1/statuses/home_timeline.xml", null);

            var tweets = new List<Tweet>();
            foreach (var xmlElement in xml.Root.Elements("status"))
            {
                var tweet = XmlToTweet(xmlElement);

                tweets.Add(tweet);
            }

            return tweets;
        }

        private static Tweet XmlToTweet(XElement xml)
        {
            var tweet = new Tweet();
            tweet.Date = DateTime.ParseExact(xml.GetSubElementValue("created_at"), "ddd MMM dd HH:mm:ss zzzz yyyy", CultureEx.Invariant);
            tweet.Id = xml.GetSubElementValue("id");
            tweet.Text = xml.GetSubElementValue("text");
            tweet.Source = xml.GetSubElementValue("source");


            return tweet;
        }

        public void UpdateStatus(string statusMessage)
        {
            string encodedMessage = HttpUtility2.UrlEncode(statusMessage);
            var parameters = new Dictionary<string, string> { { "status", encodedMessage } };
            var xml = _client.APIWebRequest(HttpMethod.POST, "http://api.twitter.com/1/statuses/update.xml", parameters);
            Console.WriteLine(xml.ToString());
        }

        public void SetAccessToken(string accessToken, string secretToken)
        {
            _client.Token = accessToken;
            _client.TokenSecret = secretToken;
        }
    }
}