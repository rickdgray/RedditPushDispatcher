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
Simply run with the necessary environment variables set and the image specified.
```bash
docker run -itd --restart unless-stopped \
--env Settings__PushoverUserKey='abcd1234' \
--env Settings__PushoverAppKey='abcd1234' \
ghcr.io/rickdgray/redditpushdispatcher:main
```

## Updating
First kill any currently running containers. Then update your local docker image with the following command.
```bash
docker pull ghcr.io/rickdgray/redditpushdispatcher:main
```
Then you can run the deployment command above to start anew.

## Building
Projects in .NET have an unusual folder structure, so when building we must both specify the context to be at the root of the solution, but also specify the location of the `dockerfile`.
```bash
docker build -f .\RedditPushDispatcher\Dockerfile .
```

## Debugging
You can edit the `launchSettings.json` file with your secrets. Then set the startup dropdown to "Docker" so that Visual Studio will create a container. This will allow you to debug with a local container.
```json
{
  "profiles": {
    "Docker": {
      "commandName": "Docker",
      "environmentVariables": {
        "Settings__PushoverUserKey": "abcd1234",
        "Settings__PushoverAppKey": "abcd1234",
      }
    }
  }
}
```
