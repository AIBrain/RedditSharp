using System;

namespace RedditSharp {

    public class RateLimitException : Exception {

        public RateLimitException( TimeSpan timeToReset ) {
            TimeToReset = timeToReset;
        }

        public TimeSpan TimeToReset { get; set; }
    }
}