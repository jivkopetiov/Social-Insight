﻿using System;
using System.Configuration;
using System.Linq;
using Abilitics.SearchPoint.Engine.LinkedIn;
using NUnit.Framework;

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
            string token = _twitter.GetRequestToken();
            Assert.IsNotNullOrEmpty(token);
        }

        [Test]
        public void GetUserById()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            int userId = 5637652;
            var user = _twitter.GetUserById(userId);
            Assert.AreEqual(userId, user.Id);
        }

        [Test]
        public void GetUserByName()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            string name = "shanselman";
            var user = _twitter.GetuserByName(name);
            Assert.AreEqual(name, user.ScreenName);
        }
        
        [Test]
        public void LookupUsers()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            var users = _twitter.GetUsersByIds(new[] { 5637652, 10413302 }.ToList());
            Assert.AreEqual(2, users.Count);
        }

        [Test]
        public void GetFriends()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            var friends = _twitter.GetFriends(_username);

            Assert.Greater(friends.Count, 0);
        }

        [Test]
        public void GetFollowers()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            var followers = _twitter.GetFollowers(_username);
            Assert.Greater(followers.Count, 0);
        }

        [Test]
        public void GetHomeTimeline()
        {
            _twitter.SetAccessToken(_lastAccessToken, _lastAccessTokenSecret);
            var timeline = _twitter.GetHomeTimeline();
            Assert.Greater(timeline.Count, 0);
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
