namespace RedditSharp {

    public class CaptchaResponse {
        public readonly string Answer;

        public CaptchaResponse( string answer = null ) {
            Answer = answer;
        }

        public bool Cancel { get { return string.IsNullOrEmpty( Answer ); } }
    }
}