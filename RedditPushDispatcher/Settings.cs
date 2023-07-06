namespace RedditPushDispatcher
{
    internal class Settings
    {
        public string PushoverUserKey { get; set; }
        public string PushoverAppKey { get; set; }
        public string RedditFeedUrl { get; set; }
        public int PollRateInMinutes { get; set; }
    }
}
