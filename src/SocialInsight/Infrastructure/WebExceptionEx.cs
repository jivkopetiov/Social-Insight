using System;
using System.Net;

namespace Abilitics.SearchPoint.Engine.Infrastructure
{
    internal static class WebExceptionEx
    {
        public static bool StatusCodeEquals(this WebException exception, HttpStatusCode statusCode)
        {
            var response = exception.Response as HttpWebResponse;

            if (response == null) return false;

            return response.StatusCode == statusCode;
        }
    }
}
