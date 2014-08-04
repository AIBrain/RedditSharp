namespace RedditSharp {

    internal class LinkData : SubmitData {

        internal LinkData() {
            Extension = "json";
            Kind = "link";
        }

        [RedditAPIName( "extension" )]
        internal string Extension { get; set; }

        [RedditAPIName( "url" )]
        internal string URL { get; set; }
    }
}