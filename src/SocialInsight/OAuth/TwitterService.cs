using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace SocialInsight
{
    public class TwitterService
    {
        private OAuthClient _client;

        private const string _baseurl = "http://api.twitter.com/1/";

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
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "friends/ids.xml", parameters);

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
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "followers/ids.xml", parameters);

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
            string encodedids = UrlEx.UrlEncode(userIds);
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "users/lookup.xml?user_id=" + encodedids, null);

            var users = xml.Root.Elements("user")
                           .Select(x => XmlToUser(x))
                           .ToList();
            return users;
        }

        public void VerifyLogin()
        {
            _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "account/verify_credentials.xml", null);
        }

        public void Retweet(long tweet, bool trimUser = true)
        {
            string url = _baseurl + "statuses/retweet/{0}.xml".Fmt(tweet);
            if (trimUser)
                url += "?trim_user=true";

            _client.APIWebRequestXml(HttpMethod.POST, url, null);
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
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "statuses/home_timeline.xml", null);

            var tweets = new List<Tweet>();
            foreach (var xmlElement in xml.Root.Elements("status"))
            {
                var tweet = XmlToTweet(xmlElement);
                tweets.Add(tweet);
            }

            return tweets;
        }

        public Tweet GetTweetById(long id)
        {
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "statuses/show/{0}.xml".Fmt(id), null);

            var tweet = XmlToTweet(xml.Root);
            return tweet;
        }

        public void UpdateStatus(string statusMessage)
        {
            string encodedMessage = UrlEx.UrlEncode(statusMessage);
            var parameters = new Dictionary<string, string> { { "status", encodedMessage } };
            var xml = _client.APIWebRequestXml(HttpMethod.POST, _baseurl + "statuses/update.xml", parameters);
        }

        public void SetAccessToken(string accessToken, string secretToken)
        {
            _client.Token = accessToken;
            _client.TokenSecret = secretToken;
        }

        public TwitterUser GetUserById(int userId)
        {
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "users/show.xml?user_id=" + userId, null);

            return XmlToUser(xml.Root);
        }

        public TwitterUser GetuserByName(string name)
        {
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "users/show.xml?screen_name=" + name, null);

            return XmlToUser(xml.Root);
        }

        public List<Tweet> GetUserTimelineByName(string username, int page = 1, int count = 20)
        {
            Arg.ThrowIfNonPositive(page, "page");
            Arg.ThrowIfOutOfRange(count, "count", 1, 200);

            var xml = _client.APIWebRequestXml(HttpMethod.GET,
                _baseurl + "statuses/user_timeline.xml?screen_name={0}&page={1}&count={2}".Fmt(username, page, count), null);

            var tweets = xml.Root.Elements("status")
                                 .Select(s => XmlToTweet(s))
                                 .ToList();
            return tweets;
        }

        public List<Tweet> GetMentions(int page = 1, int count = 20)
        {
            Arg.ThrowIfNonPositive(page, "page");
            Arg.ThrowIfOutOfRange(count, "count", 1, 200);

            var xml = _client.APIWebRequestXml(HttpMethod.GET,
                _baseurl + "statuses/mentions.xml?page={0}&count={1}".Fmt(page, count), null);

            var tweets = xml.Root.Elements("status")
                                 .Select(s => XmlToTweet(s))
                                 .ToList();
            return tweets;
        }

        public List<TwitterUser> SearchForUsers(string searchQuery, int page = 1, int count = 20)
        {
            Arg.ThrowIfNonPositive(page, "page");
            Arg.ThrowIfOutOfRange(count, "count", 1, 20);

            searchQuery = UrlEx.UrlEncode(searchQuery);
            var xml = _client.APIWebRequestXml(HttpMethod.GET, _baseurl + "users/search.xml?q=" + searchQuery, null);

            var users = xml.Root.Elements("user")
                                .Select(u => XmlToUser(u))
                                .ToList();
            return users;
        }

        public List<TwitterTrend> GetCurrentTrends()
        {
            string json = _client.APIWebRequestJson(HttpMethod.GET, _baseurl + "trends.json", null);
            File.WriteAllText(@"D:\do.test.xml", json);
            var mainContainer = JObject.Parse(json);

            var trends = new List<TwitterTrend>();
            foreach (var jsonItem in mainContainer["trends"].Children())
            {
                var trend = new TwitterTrend();

                string name = jsonItem["name"].ToString();
                string url = jsonItem["url"].ToString();
                name = name.Substring(1, name.Length - 2);
                url = url.Substring(1, url.Length - 2);
                url = HttpUtility2.UrlDecode(url);

                trend.Name = name;
                trend.Url = new Uri(url);

                trends.Add(trend);
            }

            return trends;
        }

        public List<Tweet> Search(string searchQuery, int page = 1, int count = 20)
        {
            var json = _client.APIWebRequestJson(HttpMethod.GET, "http://search.twitter.com/search.json?q={0}&page={1}&rpp={2}".Fmt(searchQuery, page, count), null);

            var root = JObject.Parse(json);
            var tweets = new List<Tweet>();
            foreach (var result in root["results"].Children())
            {
                var tweet = new Tweet();
                tweet.Id = result["id"].Value<long>();
                tweet.Date = DateTime.ParseExact(result["created_at"].Value<string>(), "ddd, dd MMM yyyy HH:mm:ss zzzz", CultureEx.Invariant);
                tweet.Source = result["source"].Value<string>();
                tweet.Text = result["text"].Value<string>();
                tweet.Language = result["iso_language_code"].Value<string>();
                tweet.UserName = result["from_user"].Value<string>();
                tweet.UserImage = new Uri(result["profile_image_url"].Value<string>());
                tweets.Add(tweet);
            }

            return tweets;
        }

        private static TwitterUser XmlToUser(XElement xml)
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

        private static Tweet XmlToTweet(XElement xml)
        {
            var tweet = new Tweet();
            tweet.Date = DateTime.ParseExact(xml.GetSubElementValue("created_at"), "ddd MMM dd HH:mm:ss zzzz yyyy", CultureEx.Invariant);
            tweet.Id = long.Parse(xml.GetSubElementValue("id"));
            tweet.Text = xml.GetSubElementValue("text");
            tweet.Source = xml.GetSubElementValue("source");

            return tweet;
        }
    }
}