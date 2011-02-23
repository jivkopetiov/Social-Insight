using System.Configuration;
using Abilitics.SearchPoint.Engine.LinkedIn;
using NUnit.Framework;
using System;

namespace SocialNetworkAPIs.UnitTests
{
    [TestFixture]
    public class TwitterServiceTests
    {
        private TwitterService _twitter;

        private static string _lastAccessToken = "255203773-DdDAX66l4Pyrm81XoDWsWhvvBbb94udo0eTtjmWb";

        private static string _lastAccessTokenSecret = "dIgoOsxVwSmpK0Ku9PJSPU6GZusml1nD8Wbo1Z5AZfM";

        private string _username = "jivkopetiov2";

        [SetUp]
        public void SetUp()
        {
            _twitter = new TwitterService(
                ConfigurationManager.AppSettings["TwitterConsumerKey"],
                ConfigurationManager.AppSettings["TwitterConsumerSecret"]);
        }

        [Test]
        public void GetRequestTokenTest()
        {
            _twitter.GetRequestToken();
        }

        [Test]
        public void GetFriends()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            _twitter.GetFriends(_username);
        }

        [Test]
        public void GetFollowers()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            _twitter.GetFollowers(_username);
        }

        [Test]
        public void GetHomeTimeline()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            _twitter.GetHomeTimeline();
        }

        [Test]
        public void UpdateStatus()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            string status = Guid.NewGuid().ToString() + @"тест &!@#`~_+=-,.<>?;'\/|$%^&*() " + Guid.NewGuid().ToString();
            _twitter.UpdateStatus(status);
        }
    }
}
