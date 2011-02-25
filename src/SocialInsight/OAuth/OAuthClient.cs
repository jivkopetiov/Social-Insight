using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SocialInsight
{
    public class OAuthClient
    {
        private enum RequestType
        {
            RequestToken,
            AccessToken,
            ApiRequest
        }

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private const string _apiVersion = "1.0";
        private const string _oauthPrefix = "oauth_";
        private const string _consumerKeyIdentifier = "oauth_consumer_key";
        private const string _callbackIdentifier = "oauth_callback";
        private const string _versionIdentifier = "oauth_version";
        private const string _signatureMethodIdentifier = "oauth_signature_method";
        private const string _signatureIdentifier = "oauth_signature";
        private const string _timestampIdentifier = "oauth_timestamp";
        private const string _nonceIdentifier = "oauth_nonce";
        private const string _tokenIdentifier = "oauth_token";
        private const string _verifierIdentifier = "oauth_verifier";
        private const string _tokenSecretIdentifier = "oauth_token_secret";

        private const string HMACSHA1SignatureType = "HMAC-SHA1";
        private const string PlainTextSignatureType = "PLAINTEXT";
        private const string RSASHA1SignatureType = "RSA-SHA1";

        private Random _random = new Random();

        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public string Realm { get; set; }
        public string RequestTokenServiceUrl { get; set; }
        public string AuthorizeServiceUrl { get; set; }
        public string AccessTokenServiceUrl { get; set; }
        public string CallbackUrl { get; set; }

        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string Verifier { get; set; }

        private const string _unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public OAuthClient(string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public XDocument APIWebRequest(HttpMethod method, string url, Dictionary<string, string> parameters)
        {
            Uri uri = new Uri(url);
            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();

            string outUrl, querystring;

            string signature = GenerateSignature(uri,
                _consumerKey,
                _consumerSecret,
                Token,
                TokenSecret,
                method,
                timeStamp,
                nonce,
                SignatureTypes.HMACSHA1,
                RequestType.ApiRequest,
                out outUrl,
                parameters,
                out querystring);


            var webRequest = BuildRequest(method, url);

            string authHeader = string.Format(@"OAuth oauth_nonce=""{0}"", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""{1}"", oauth_consumer_key=""{2}"", oauth_token=""{3}"",oauth_signature=""{4}"", oauth_version=""1.0""",
                nonce, timeStamp, _consumerKey, Token, UrlEx.UrlEncode(signature));

            webRequest.Headers.Add("Authorization", authHeader);

            if (method == HttpMethod.POST)
                AppendPostDataToBody(parameters, webRequest);

            try
            {
                using (var response = webRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                    return XDocument.Load(stream);
            }
            catch (WebException ex)
            {
                string message = GetErrorMessage(ex);
                if (message != null)
                    throw new InvalidOperationException(message, ex);
                else
                    throw;
            }
        }

        private static void AppendPostDataToBody(Dictionary<string, string> parameters, HttpWebRequest webRequest)
        {
            string postData = GetPostData(parameters);

            if (string.IsNullOrEmpty(postData))
                return;

            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            webRequest.ContentLength = postBytes.Length;

            using (var reqStream = webRequest.GetRequestStream())
                reqStream.Write(postBytes, 0, postBytes.Length);
        }

        private static string GetPostData(Dictionary<string, string> parameters)
        {
            string postData = null;
            if (parameters != null && parameters.Count > 0)
            {
                postData = "";
                foreach (var pair in parameters)
                    postData += pair.Key + "=" + pair.Value + "&";

                postData = postData.Substring(0, postData.Length - 1);
            }
            return postData;
        }

        public string AuthorizationLinkGet()
        {
            string result = null;

            string response = PerformWebRequest(HttpMethod.POST, RequestTokenServiceUrl, String.Empty, RequestType.RequestToken);
            if (response.Length > 0)
            {
                var queryString = HttpUtility2.ParseQueryString(response, Encoding.UTF8);
                if (queryString[_tokenIdentifier] != null)
                {
                    Token = queryString[_tokenIdentifier];
                    TokenSecret = queryString[_tokenSecretIdentifier];
                    result = AuthorizeServiceUrl + string.Format("?{0}={1}", _tokenIdentifier, Token);
                }
            }

            return result;
        }

        public void AccessTokenGet(string authToken)
        {
            Token = authToken;

            string response = PerformWebRequest(HttpMethod.POST, AccessTokenServiceUrl, string.Empty, RequestType.AccessToken);

            if (response.Length > 0)
            {
                var queryString = HttpUtility2.ParseQueryString(response, Encoding.UTF8);

                if (queryString[_tokenIdentifier] != null)
                    Token = queryString[_tokenIdentifier];

                if (queryString[_tokenSecretIdentifier] != null)
                    TokenSecret = queryString[_tokenSecretIdentifier];
            }
        }

        private string PerformWebRequest(HttpMethod method, string url, string postData, RequestType type)
        {
            string outUrl = "";
            string querystring = "";

            if (method == HttpMethod.POST)
            {
                if (postData.Length > 0)
                {
                    NameValueCollection qs = HttpUtility2.ParseQueryString(postData, Encoding.UTF8);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility2.UrlDecode(qs[key]);
                        qs[key] = UrlEx.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);

            string sig = GenerateSignature(uri,
                _consumerKey,
                _consumerSecret,
                Token,
                TokenSecret,
                method,
                GenerateTimeStamp(),
                GenerateNonce(),
                SignatureTypes.HMACSHA1,
                type,
                out outUrl,
                null,
                out querystring);

            querystring += string.Format("&{0}={1}", _signatureIdentifier, UrlEx.UrlEncode(sig));

            if (method == HttpMethod.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            return WebRequestCore(method, outUrl + querystring, postData);
        }

        private string WebRequestCore(HttpMethod method, string url, string postData)
        {
            var webRequest = BuildRequest(method, url);

            if (method == HttpMethod.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (var stream = webRequest.GetRequestStream())
                using (var writer = new StreamWriter(stream))
                    writer.Write(postData);
            }

            using (var response = webRequest.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private static HttpWebRequest BuildRequest(HttpMethod method, string url)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method.ToString();
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.ServicePoint.Expect100Continue = false;
            return webRequest;
        }

        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException("hashAlgorithm");

            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(_oauthPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        private string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var builder = new StringBuilder();
            for (int counter = 0; counter < parameters.Count; counter++)
            {
                var parameter = parameters[counter];
                builder.AppendFormat("{0}={1}", parameter.Name, parameter.Value);

                if (counter < parameters.Count - 1)
                {
                    builder.Append("&");
                }
            }

            return builder.ToString();
        }

        private string GenerateSignatureBase(
            Uri url, string consumerKey, string token, string tokenSecret, HttpMethod httpMethod,
            string timeStamp, string nonce, string signatureType, RequestType type, out string normalizedUrl, Dictionary<string, string> additionalParameters, out string normalizedRequestParameters)
        {
            if (token == null)
                token = "";

            if (tokenSecret == null)
                tokenSecret = "";

            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentNullException("consumerKey");

            if (string.IsNullOrEmpty(signatureType))
                throw new ArgumentNullException("signatureType");

            normalizedUrl = null;
            normalizedRequestParameters = null;

            var parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(_versionIdentifier, _apiVersion));
            parameters.Add(new QueryParameter(_nonceIdentifier, nonce));
            parameters.Add(new QueryParameter(_timestampIdentifier, timeStamp));
            parameters.Add(new QueryParameter(_signatureMethodIdentifier, signatureType));
            parameters.Add(new QueryParameter(_consumerKeyIdentifier, consumerKey));
            if (additionalParameters != null)
            {
                foreach (var pair in additionalParameters)
                    parameters.Add(new QueryParameter(pair.Key, pair.Value));
            }

            if (type == RequestType.RequestToken && CallbackUrl != null)
                parameters.Add(new QueryParameter(_callbackIdentifier, UrlEx.UrlEncode(CallbackUrl)));

            if (!string.IsNullOrEmpty(token))
                parameters.Add(new QueryParameter(_tokenIdentifier, token));

            if (type == RequestType.AccessToken && !string.IsNullOrEmpty(Verifier))
                parameters.Add(new QueryParameter(_verifierIdentifier, Verifier));

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            string result = string.Format("{0}&{1}&{2}", httpMethod.ToString(), UrlEx.UrlEncode(normalizedUrl), UrlEx.UrlEncode(normalizedRequestParameters));
            return result;
        }

        private string GenerateSignature(
            Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret,
            HttpMethod httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, RequestType type,
            out string normalizedUrl, Dictionary<string, string> additionalParameters, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(
                        url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, type, out normalizedUrl, additionalParameters, out normalizedRequestParameters);
                    var hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEx.UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEx.UrlEncode(tokenSecret)));
                    return ComputeHash(hmacsha1, signatureBase);
                case SignatureTypes.PLAINTEXT:
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", signatureType.ToString());
            }
        }

        private string GenerateTimeStamp()
        {
            var timespan = DateTime.UtcNow - _unixEpoch;
            string result = Convert.ToInt64(timespan.TotalSeconds).ToString();
            return result;
        }

        private string GenerateNonce()
        {
            return _random.Next(123400, 9999999).ToString();
        }

        private class QueryParameter
        {
            private string _name = null;
            private string _value = null;

            public QueryParameter(string name, string value)
            {
                _name = name;
                _value = value;
            }

            public string Name
            {
                get { return _name; }
            }

            public string Value
            {
                get { return _value; }
            }
        }

        private class QueryParameterComparer : IComparer<QueryParameter>
        {
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }
        }

        public XDocument PublicAPIWebRequest(HttpMethod method, string url, Dictionary<string, string> parameters)
        {
            var uri = new Uri(url);

            if (method == HttpMethod.GET)
                AppendQueryParameters(uri, parameters);

            var request = BuildRequest(method, url);

            if (method == HttpMethod.POST)
                AppendPostDataToBody(parameters, request);

            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    return XDocument.Load(stream);
            }
            catch (WebException ex)
            {
                string message = GetErrorMessage(ex);
                if (message != null)
                    throw new InvalidOperationException(message, ex);
                else
                    throw;
            }
        }

        public static Uri AppendQueryParameters(Uri uri, IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return uri;

            var builder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                builder.AppendFormat("{0}={1}&", parameter.Key, parameter.Value);
            }
            string result = builder.ToString();
            result = result.Substring(0, result.Length - 1);

            string queryStringSeparator = null;
            if (uri.AbsoluteUri.Contains("?"))
                queryStringSeparator = "&";
            else
                queryStringSeparator = "?";

            Uri resultUri = new Uri(uri.AbsoluteUri + queryStringSeparator + result);
            return resultUri;
        }

        private string GetErrorMessage(WebException ex)
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