# Reddit Push Dispatcher
Push new posts from your favorite subreddit right to your phone!

## Pushover
You will need an account at [Pushover](https://pushover.net/) to be able to run this. Be sure to also create an application and get the associated `API Token/Key` as well.

## Settings
Once you create an account and have both a `User Key` and an `API Token/Key`, you need to set them here.

You can do it one of two ways. You can set them as environment variables:

```powershell
$Env:Settings__PushoverUserKey = ''
$Env:Settings__PushoverAppKey = ''
$Env:Settings__RedditFeedUrl = 'https://old.reddit.com/r/GamingLeaksAndRumours/new/'
$Env:Settings__PollRateInMinutes = 60
```

Or you can set them in the `appsettings.json` in the codebase:

```json
"Settings": {
  "PushoverUserKey": "",
  "PushoverAppKey": "",
  "RedditFeedUrl": "https://old.reddit.com/r/GamingLeaksAndRumours/new/",
  "PollRateInMinutes": 60
}
```

Feel free to also set your preferred subreddit and the rate at which the services will poll and scrape Reddit.

## Deploying

Simply build your image and run your container. Projects in .NET have an unusual folder structure, so we must both specify the context at the solution file, but also specify the location of the `dockerfile`. See below.

```bash
docker build -t redditpushdispatcher -f .\RedditPushDispatcher\Dockerfile .
docker run -it redditpushdispatcher
```
