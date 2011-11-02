/**
 * MCrawl.CookieAwareWebClient
 * Extends the WebClient and allows for Login (Cookies)
 * Andre Brutus - ith help from lasseespeholt
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MCrawl
{
    /**
     * "Credit Where Credit is Due"
     * Cookie Aware web client courtesy of "lasseespeholt"
     * http://stackoverflow.com/questions/5869785/post-username-and-password-to-login-page-programmatically/5869858#5869858
     */
    internal class CookieAwareWebClient : WebClient
    {
        private CookieContainer cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = cookieContainer;
            }
            return request;
        }
    }
}
