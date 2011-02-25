using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SocialInsight.Facebook
{
    public class FacebookService
    {
        private string _accessToken;

        public FacebookService(string accessToken)
        {
            _accessToken = accessToken;
        }

        public List<FacebookUser> GetFriends()
        {
            string url = "https://graph.facebook.com/me/friends?access_token={0}".Fmt(_accessToken);

            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(response))
            {
                string json = reader.ReadToEnd();
                return ParseJsonForListOfPeople(json);
            }
        }

        public List<FacebookUser> SearchPeople(string query)
        {
            string url = "https://graph.facebook.com/search?q={0}&type=user&access_token={1}".Fmt(query, _accessToken);
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(response))
            {
                string json = reader.ReadToEnd();
                return ParseJsonForListOfPeople(json);
            }
        }

        public void PostToFacebookWall(long id, string message, Uri picture = null)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://graph.facebook.com/{0}/feed".Fmt(id));
            string parameters = "access_token={0}&message={1}".Fmt(_accessToken, message);
            if (picture != null)
                parameters += "&picture=" + picture.ToString();

            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(parameters);
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = request.GetResponse().GetResponseStream())
                using (var reader = new StreamReader(response))
                {
                    string textResponse = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                string error = WebExceptionEx.GetErrorMessage(ex);
                if (error != null)
                    throw new InvalidOperationException(message, ex);
                else
                    throw;
            }
        }

        private static List<FacebookUser> ParseJsonForListOfPeople(string json)
        {
            var doc = JObject.Parse(json);
            var friends = new List<FacebookUser>();
            foreach (var item in doc["data"].Children())
            {
                var user = new FacebookUser();
                user.Id = item["id"].Value<long>();
                user.Name = item["name"].Value<string>();
                friends.Add(user);
            }

            return friends;
        }
    }
}
