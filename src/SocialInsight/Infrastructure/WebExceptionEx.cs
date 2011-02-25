using System;
using System.Net;
using System.IO;

namespace SocialInsight
{
    internal static class WebExceptionEx
    {
        public static bool StatusCodeEquals(this WebException exception, HttpStatusCode statusCode)
        {
            var response = exception.Response as HttpWebResponse;

            if (response == null) return false;

            return response.StatusCode == statusCode;
        }

        public static string GetErrorMessage(WebException ex)
        {
            var response = ex.Response as HttpWebResponse;
            if (response == null)
                return null;

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    string message = reader.ReadToEnd();
                    return message;
                }
            }
        }
    }
}
