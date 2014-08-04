using System;

namespace RedditSharp {

    public struct Captcha {
        public readonly string Id;
        public readonly Uri Url;
        private const string UrlFormat = "http://www.reddit.com/captcha/{0}";

        internal Captcha( string id ) {
            Id = id;
            Url = new Uri( string.Format( UrlFormat, Id ), UriKind.Absolute );
        }
    }
}