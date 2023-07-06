namespace RedditPushDispatcher
{
    internal class RedditPost
    {
        public string Flair { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PostTime { get; set; }
        public string Url { get; set; }
        public int CommentCount { get; set; }
    }
}
