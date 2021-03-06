﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Things {

    public class PrivateMessage : Thing {
        private const string CommentUrl = "/api/comment";
        private const string SetAsReadUrl = "/api/read_message";

        [JsonProperty( "author" )]
        public string Author { get; set; }

        [JsonProperty( "body" )]
        public string Body { get; set; }

        [JsonProperty( "body_html" )]
        public string BodyHtml { get; set; }

        [JsonProperty( "dest" )]
        public string Destination { get; set; }

        [JsonProperty( "first_message_name" )]
        public string FirstMessageName { get; set; }

        [JsonProperty( "was_comment" )]
        public bool IsComment { get; set; }

        [JsonIgnore]
        public PrivateMessage Parent {
            get {
                if ( string.IsNullOrEmpty( ParentID ) )
                    return null;
                var id = ParentID.Remove( 0, 3 );
                var listing = new Listing<PrivateMessage>( Reddit, "/message/messages/" + id + ".json", WebAgent );
                var firstMessage = listing.First();
                if ( firstMessage.FullName == ParentID )
                    return listing.First();
                return firstMessage.Replies.First( x => x.FullName == this.ParentID );
            }
        }

        [JsonProperty( "parent_id" )]
        public string ParentID { get; set; }

        [JsonIgnore]
        public PrivateMessage[] Replies { get; set; }

        [JsonProperty( "created" )]
        [JsonConverter( typeof( UnixTimestampConverter ) )]
        public DateTime Sent { get; set; }

        [JsonProperty( "created_utc" )]
        [JsonConverter( typeof( UnixTimestampConverter ) )]
        public DateTime SentUTC { get; set; }

        [JsonProperty( "subject" )]
        public string Subject { get; set; }

        [JsonProperty( "subreddit" )]
        public string Subreddit { get; set; }

        public Listing<PrivateMessage> Thread {
            get {
                if ( string.IsNullOrEmpty( ParentID ) )
                    return null;
                var id = ParentID.Remove( 0, 3 );
                return new Listing<PrivateMessage>( Reddit, "/message/messages/" + id + ".json", WebAgent );
            }
        }

        [JsonProperty( "new" )]
        public bool Unread { get; set; }

        private Reddit Reddit { get; set; }

        private IWebAgent WebAgent { get; set; }

        public PrivateMessage Init( Reddit reddit, JToken json, IWebAgent webAgent ) {
            CommonInit( reddit, json, webAgent );
            JsonConvert.PopulateObject( json[ "data" ].ToString(), this, reddit.JsonSerializerSettings );
            return this;
        }

        public async Task<PrivateMessage> InitAsync( Reddit reddit, JToken json, IWebAgent webAgent ) {
            CommonInit( reddit, json, webAgent );
            await JsonConvert.PopulateObjectAsync( json[ "data" ].ToString(), this, reddit.JsonSerializerSettings );
            return this;
        }

        public void Reply( string message ) {
            if ( Reddit.User == null )
                throw new AuthenticationException( "No user logged in." );
            var request = WebAgent.CreatePost( CommentUrl );
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody( stream, new {
                text = message,
                thing_id = FullName,
                uh = Reddit.User.Modhash
            } );
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString( response.GetResponseStream() );
            var json = JObject.Parse( data );
        }

        public void SetAsRead() {
            var request = WebAgent.CreatePost( SetAsReadUrl );
            WebAgent.WritePostBody( request.GetRequestStream(), new {
                id = FullName,
                uh = Reddit.User.Modhash,
                api_type = "json"
            } );
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString( response.GetResponseStream() );
        }

        private void CommonInit( Reddit reddit, JToken json, IWebAgent webAgent ) {
            base.Init( json );
            Reddit = reddit;
            WebAgent = webAgent;
            var data = json[ "data" ];
            if ( data[ "replies" ] != null && data[ "replies" ].Any() ) {
                if ( data[ "replies" ][ "data" ] != null ) {
                    if ( data[ "replies" ][ "data" ][ "children" ] != null ) {
                        var replies = new List<PrivateMessage>();
                        foreach ( var reply in data[ "replies" ][ "data" ][ "children" ] )
                            replies.Add( new PrivateMessage().Init( reddit, reply, webAgent ) );
                        Replies = replies.ToArray();
                    }
                }
            }
        }

        #region Obsolete Getter Methods

        [Obsolete( "Use Thread property instead" )]
        public Listing<PrivateMessage> GetThread() {
            return Thread;
        }

        #endregion Obsolete Getter Methods
    }
}