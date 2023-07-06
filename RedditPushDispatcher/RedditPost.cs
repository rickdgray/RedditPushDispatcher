namespace RedditPushDispatcher
{
    internal class RedditPost
    {
        public string Flair { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset PostTime { get; set; }
        public string Url { get; set; } = string.Empty;
        public int CommentCount { get; set; }
    }
}
