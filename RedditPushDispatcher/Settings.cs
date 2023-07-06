namespace RedditPushDispatcher
{
    internal class Settings
    {
        public string PushoverUserKey { get; set; } = string.Empty;
        public string PushoverAppKey { get; set; } = string.Empty;
        public string RedditFeedUrl { get; set; } = string.Empty;
        public int PollRateInMinutes { get; set; }
    }
}
