using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SocialInsight
{
    public class LinkedInService
    {
        private OAuthClient _client;

        public LinkedInService(string consumerKey, string consumerSecret)
        {
            _client = new OAuthClient(consumerKey, consumerSecret);

            _client.RequestTokenServiceUrl = "https://api.linkedin.com/uas/oauth/requestToken";
            _client.AuthorizeServiceUrl = "https://api.linkedin.com/uas/oauth/authorize";
            _client.AccessTokenServiceUrl = "https://api.linkedin.com/uas/oauth/accessToken";
            _client.Realm = "http://api.linkedin.com/";
            _client.CallbackUrl = "http://jivkopetiov.com.s3.amazonaws.com/linkedInLoggedIn.htm";
        }

        public void SetAccessToken(string accessToken, string accessTokenSecret, string verifier)
        {
            _client.Token = accessToken;
            _client.TokenSecret = accessTokenSecret;
            _client.Verifier = verifier;
        }

        public void SetVerifier(string verifier)
        {
            _client.Verifier = verifier;
        }

        public string GetRequestToken()
        {
            return _client.AuthorizationLinkGet();
        }

        public void GetAccessToken(string requestToken)
        {
            _client.AccessTokenGet(requestToken);
        }

        public List<LinkedInProfile> SearchFor(string keyword, int page, int count)
        {
            page--;
            int start = page * count;

            string fields = "id,first-name,last-name,picture-url,headline,location,industry,distance";
            var xml = _client.APIWebRequest(HttpMethod.GET,
                string.Format("http://api.linkedin.com/v1/people-search:(people:({0}),num-results)?keywords={1}&start={2}&count={3}",
                fields, keyword, start, count), null);

            var people = xml.Root.Element("people")
                                 .Elements("person")
                                 .Select(e => LinkedInProfileFromXml(e))
                                 .ToList();
            return people;
        }

        public LinkedInProfile GetProfile(string id)
        {
            var xml = _client.APIWebRequest(HttpMethod.GET,
                string.Format("http://api.linkedin.com/v1/people/id={0}:({1})", id, ProfileFields()), null);

            var person = LinkedInProfileFromXml(xml.Root);
            return person;
        }

        private static string ProfileFields()
        {
            string fields = "id,first-name,last-name,picture-url,headline,location,industry,distance,positions,current-share,num-connections,summary,specialties";
            return fields;
        }

        public LinkedInProfile GetMyProfile()
        {
            var xml = _client.APIWebRequest(HttpMethod.GET,
                string.Format("http://api.linkedin.com/v1/people/~:({0})", ProfileFields()), null);

            var person = LinkedInProfileFromXml(xml.Root);
            return person;
        }

        /// <summary>
        /// Page is 1-based, e.g. the first page is 1;
        /// </summary>
        public List<LinkedInProfile> GetMyConnections(int page, int count)
        {
            string fields = "id,first-name,last-name,picture-url,headline,location,industry,distance,positions";

            if (page == 0)
                page = 1;

            int start = page - 1;

            var xml = _client.APIWebRequest(HttpMethod.GET,
                string.Format("http://api.linkedin.com/v1/people/~/connections:({0})?start={1}&count={2}",
                fields, start, count), null);

            var people = xml.Root.Elements("person")
                                 .Select(e => LinkedInProfileFromXml(e))
                                 .ToList();

            return people;
        }

        private static LinkedInProfile LinkedInProfileFromXml(XElement xmlElement)
        {
            var person = new LinkedInProfile();
            person.Id = xmlElement.GetSubElementValue("id");
            person.Name = xmlElement.GetSubElementValue("first-name") + " " +
                          xmlElement.GetSubElementValue("last-name");
            person.Picture = xmlElement.GetSubElementUrlNoThrow("picture-url");
            person.Headline = xmlElement.GetSubElementValueNoThrow("headline");
            person.Industry = xmlElement.GetSubElementValueNoThrow("industry");
            person.CurrentStatus = xmlElement.GetSubElementValueNoThrow("current-status");
            person.CurrentStatusDate = xmlElement.GetSubElementValueNoThrow("current-status-timestamp");
            person.CurrentShare = xmlElement.GetSubElementValueNoThrow("current-share");
            person.NumberOfConnections = xmlElement.GetSubElementNumberNoThrow("num-connections");
            person.Distance = xmlElement.GetSubElementNumber("distance");
            person.Summary = xmlElement.GetSubElementValueNoThrow("summary");
            person.Specialties = xmlElement.GetSubElementValueNoThrow("specialties");
            person.Interests = xmlElement.GetSubElementValueNoThrow("interests");

            var locationXml = xmlElement.Element("location");
            person.Location = locationXml.GetSubElementValue("name");
            person.Country = locationXml.GetSubSubElementValue("country", "code");

            var positionsXml = xmlElement.Element("positions");
            if (positionsXml != null)
            {
                person.Positions = positionsXml.Elements("position")
                    .Select(xml => LinkedInPositionFromXml(xml))
                    .ToList();
            }

            return person;
        }

        private static LinkedInPosition LinkedInPositionFromXml(XElement xml)
        {
            var position = new LinkedInPosition();
            position.Id = xml.GetSubElementValue("id");
            position.Title = xml.GetSubElementValue("title");
            position.Summary = xml.GetSubElementValueNoThrow("summary");
            position.IsCurrent = bool.Parse(xml.GetSubElementValue("is-current").ToLowerInvariant());
            position.CompanyName = xml.GetSubSubElementValueNoThrow("company", "name");
            position.Industry = xml.GetSubSubElementValueNoThrow("company", "industry");

            var startDateXml = xml.Element("start-date");
            if (startDateXml != null)
            {
                int startYear = startDateXml.GetSubElementNumber("year");
                int startMonth = startDateXml.GetSubElementNumberNoThrow("month");
                if (startMonth == 0)
                    startMonth = 1;

                position.StartDate = new DateTime(startYear, startMonth, 1);
            }

            var endDateXml = xml.Element("end-date");
            if (endDateXml != null)
            {
                int endYear = endDateXml.GetSubElementNumber("year");
                int endMonth = endDateXml.GetSubElementNumberNoThrow("month");
                if (endMonth == 0)
                    endMonth = 1;

                position.EndDate = new DateTime(endYear, endMonth, 1);
            }

            return position;
        }
    }

}