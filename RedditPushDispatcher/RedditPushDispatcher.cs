using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Web;

namespace RedditPushDispatcher
{
    internal class RedditPushDispatcher : BackgroundService
    {
        private readonly Settings _settings;
        private readonly ILogger<RedditPushDispatcher> _logger;

        public RedditPushDispatcher(IOptions<Settings> settings,
            ILogger<RedditPushDispatcher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting application.");

            if (string.IsNullOrWhiteSpace(_settings.PushoverUserKey))
            {
                throw new ArgumentNullException("PushoverUserKey", "The user key was not specified!");
            }

            if (string.IsNullOrWhiteSpace(_settings.PushoverAppKey))
            {
                throw new ArgumentNullException("PushoverAppKey", "The app key was not specified!");
            }

            using var client = new HttpClient();

            var lastPoll = DateTimeOffset.Now;

            lastPoll = lastPoll.AddHours(-4);

            _logger.LogDebug("Poll time: {lastPoll}", lastPoll);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Pulling Feed.");

                var doc = await new HtmlWeb().LoadFromWebAsync(_settings.RedditFeedUrl, stoppingToken);
                var nodes = doc.DocumentNode
                    .SelectSingleNode("//body/div[@class='content' and @role='main']/div[@id='siteTable']")
                    ?.ChildNodes
                    ?? throw new Exception("Couldn't load posts.");

                _logger.LogInformation("Parsing data...");

                var posts = new List<RedditPost>();
                foreach (var node in nodes)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    // not a post
                    if (!node.HasClass("thing"))
                    {
                        continue;
                    }

                    // ignore spacers and ads
                    if (node.HasClass("clearleft") || node.HasClass("promoted"))
                    {
                        continue;
                    }

                    if (!long.TryParse(node.GetDataAttribute("timestamp").Value, out long unixTimestamp))
                    {
                        continue;
                    }

                    var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).ToLocalTime();

                    _logger.LogDebug("Timestamp: {timestamp}", timestamp);

                    if (timestamp < lastPoll)
                    {
                        continue;
                    }

                    var relativePermalink = node.GetDataAttribute("permalink")?.Value ?? string.Empty;

                    var postNode = node.SelectSingleNode(".//div[@class='top-matter']");

                    var titleNode = postNode.SelectSingleNode("./p[@class='title']");
                    var title = titleNode.InnerText.Trim().Split("&#32;").FirstOrDefault() ?? string.Empty;

                    var flair = titleNode.FirstChild.GetAttributeValue("title", string.Empty);

                    if (title.StartsWith(flair))
                    {
                        title = title[flair.Length..];
                    }

                    var commentsString = postNode.ChildNodes[3].InnerText ?? string.Empty;
                    if (!string.IsNullOrEmpty(commentsString))
                    {
                        commentsString = commentsString.Trim().Split(" comment").FirstOrDefault();
                    }

                    if (!int.TryParse(commentsString, out int commentCount))
                    {
                        continue;
                    }

                    posts.Add(new RedditPost
                    {
                        Flair = flair,
                        Title = HttpUtility.HtmlDecode(title),
                        PostTime = timestamp,
                        Url = $"https://reddit.com{relativePermalink}",
                        CommentCount = commentCount
                    });
                }

                _logger.LogInformation("Found {count} new posts.", posts.Count);

                var notifications = new List<Dictionary<string, string>>();

                foreach (var post in posts)
                {
                    if (post == null)
                    {
                        continue;
                    }

                    var data = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey }
                    };

                    if (string.IsNullOrWhiteSpace(post.Title))
                    {
                        continue;
                    }

                    data.Add("message", $"{post.Title}. {post.CommentCount} comments.");

                    if (string.IsNullOrWhiteSpace(post.Flair))
                    {
                        data.Add("title", "Gaming Leaks and Rumors");
                    }
                    else
                    {
                        data.Add("title", $"Gaming Leaks and Rumors: {post.Flair}");
                    }

                    if (!string.IsNullOrWhiteSpace(post.Url))
                    {
                        data.Add("url", post.Url);
                        data.Add("url_title", "Read more");
                    }

                    notifications.Add(data);
                    _logger.LogDebug("Queued notification. Count: {count}", notifications.Count);
                }

                _logger.LogInformation("Queued all notifications. Count: {count}", notifications.Count);

                foreach (var notification in notifications)
                {
                    await client.PostAsync(
                        "https://api.pushover.net/1/messages.json",
                        new FormUrlEncodedContent(notification),
                        stoppingToken);

                    _logger.LogDebug("Pushed notification; delaying for 3 seconds...");

                    await Task.Delay(3000, stoppingToken);
                }

                _logger.LogInformation(
                    "Pushed all notifications. Waiting until next poll time: {pollTime}",
                    DateTime.Now.AddMinutes(_settings.PollRateInMinutes));

                await Task.Delay(_settings.PollRateInMinutes * 60000, stoppingToken);
            }
        }
    }
}
