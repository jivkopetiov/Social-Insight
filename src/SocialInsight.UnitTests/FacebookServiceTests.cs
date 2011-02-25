using System;
using NUnit.Framework;
using SocialInsight.Facebook;

namespace SocialInsight.UnitTests
{
    [TestFixture]
    public class FacebookServiceTests
    {
        private string _accessToken = "2227470867|2.YNURfN0E_SeJtinfUyk7nQ__.3600.1298678400-643618867|JxcVesWS-Sf0sPWTw9hvr2cO1dk";

        private FacebookService _facebook;

        [SetUp]
        public void Setup()
        {
            _facebook = new FacebookService(_accessToken);
        }

        [Test]
        public void GetFriends()
        {
            var friends = _facebook.GetFriends();
            Assert.Greater(friends.Count, 0);
        }

        [Test]
        public void PostToFacebookWall()
        {
            _facebook.PostToFacebookWall(123687284366775, Guid.NewGuid().ToString());
        }

        [Test]
        public void SearchPeople()
        {
            _facebook.SearchPeople("Ivan");
        }
    }
}
