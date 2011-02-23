using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SocialNetworkAPIs.UnitTests
{
    public class FacebookServiceTests
    {
        public void PostToFacebookWall()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://graph.facebook.com/123687284366775/feed");
            string parameters = "access_token=123687284366775|981f1ab1d1babc6f9c052280-643618867|-w-DOrM-d5yHDAHRqt9EoVymYzY&message=Testing picture&picture=http://www.google.bg/logos/classicplus.png";
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(parameters);
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = request.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(response))
                Console.WriteLine(reader.ReadToEnd());
        }
    }
}
