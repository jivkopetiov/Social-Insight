using System;
using System.Text;
using System.Windows.Forms;
using SocialInsight;

namespace TwitterTester
{
    public partial class Form1 : Form
    {
        private TwitterService _twitter = new TwitterService("PVzbzoAMnH9mpjZ3NjhP8w", "apMe8pXhDhOoV6bRwtMgh3IDTmNlu0Ix1FKdAmovE");

        public Form1()
        {
            InitializeComponent();

            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            webBrowser.Navigated += new WebBrowserNavigatedEventHandler(webBrowser_Navigated);
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
        }

        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            txtLog.Text += "Navigating to " + e.Url + Environment.NewLine;
        }

        void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            txtLog.Text += "Navigated to " + e.Url + Environment.NewLine;
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            txtLog.Text += "document completed " + e.Url + Environment.NewLine;

            if (e.Url.AbsoluteUri.StartsWith("http://jivkopetiov.com"))
            {
                var query = HttpUtility2.ParseQueryString(e.Url.Query, Encoding.UTF8);
                string authorizedRequestToken = query["oauth_token"];
                string verifier = query["oauth_verifier"];

                _twitter.SetVerifier(verifier);
                
                var result  = _twitter.GetAccessToken(authorizedRequestToken);
                txtAccessToken.Text = result.Item1;
                txtAccessTokenSecret.Text = result.Item2;
            }
        }

        private void btnGetRequestToken_Click(object sender, EventArgs e)
        {
            string token = _twitter.GetRequestToken();
            txtRequestToken.Text = token;
            webBrowser.Url = new Uri(token);
        }

        private void btnGetAccessToken_Click(object sender, EventArgs e)
        {

        }
    }
}
