using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace RedditSharp {

    public interface IWebAgent {

        string AccessToken { get; set; }

        string AuthCookie { get; set; }

        CookieContainer Cookies { get; set; }

        JToken CreateAndExecuteRequest( string url );

        HttpWebRequest CreateGet( string url );

        HttpWebRequest CreatePost( string url );

        HttpWebRequest CreateRequest( string url, string method );

        JToken ExecuteRequest( HttpWebRequest request );

        string GetResponseString( Stream stream );

        void WritePostBody( Stream stream, object data, params string[] additionalFields );
    }
}