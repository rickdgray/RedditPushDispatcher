# Reddit Push Dispatcher
Push new posts from your favorite subreddit right to your phone!

## Pushover
You will need an account at [Pushover](https://pushover.net/) to be able to run this. Be sure to also create an application and get the associated `API Token/Key` as well.

## Settings
Once you create an account and have both a `User Key` and an `API Token/Key`, you need to pass them in at runtime. Feel free to also set your preferred subreddit and the rate at which the services will poll and scrape Reddit. URLs **must** be to `old.reddit.com`. The optional settings are:

* PushoverUserKey
* PushoverAppKey
* RedditFeedUrl (optional, defaults to [r/GamingLeaksAndRumours/new/](old.reddit.com/r/GamingLeaksAndRumours/new/))
* PollRateInMinutes (optional, defaults to 60)

## Deploying
Simply build your image and run your container. Projects in .NET have an unusual folder structure, so we must both specify the context at the solution file, but also specify the location of the `dockerfile`. See below.

```bash
docker build -t redditpushdispatcher -f .\RedditPushDispatcher\Dockerfile .
docker run --env Settings__PushoverUserKey='abcd1234' --env Settings__PushoverAppKey='abcd1234' redditpushdispatcher
```
