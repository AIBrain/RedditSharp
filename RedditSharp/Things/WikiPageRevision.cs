using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Things {

    public class WikiPageRevision : Thing {

        protected internal WikiPageRevision() {
        }

        [JsonIgnore]
        public RedditUser Author { get; set; }

        [JsonProperty( "id" )]
        new public string Id { get; private set; }

        [JsonProperty( "page" )]
        public string Page { get; private set; }

        [JsonProperty( "reason" )]
        public string Reason { get; private set; }

        [JsonProperty( "timestamp" )]
        [JsonConverter( typeof( UnixTimestampConverter ) )]
        public DateTime? TimeStamp { get; set; }

        internal WikiPageRevision Init( Reddit reddit, JToken json, IWebAgent webAgent ) {
            CommonInit( reddit, json, webAgent );
            JsonConvert.PopulateObject( json.ToString(), this, reddit.JsonSerializerSettings );
            return this;
        }

        internal async Task<WikiPageRevision> InitAsync( Reddit reddit, JToken json, IWebAgent webAgent ) {
            CommonInit( reddit, json, webAgent );
            await JsonConvert.PopulateObjectAsync( json.ToString(), this, reddit.JsonSerializerSettings );
            return this;
        }

        private void CommonInit( Reddit reddit, JToken json, IWebAgent webAgent ) {
            base.Init( json );
            Author = new RedditUser().Init( reddit, json[ "author" ], webAgent );
        }
    }
}