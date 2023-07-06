using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditPushDispatcher;

var builder = Host.CreateApplicationBuilder();
builder.Logging.AddConsole();
builder.Services.AddHostedService<RedditPushDispatcher.RedditPushDispatcher>();
builder.Services.Configure<Settings>(
    builder.Configuration.GetSection(nameof(Settings)));

builder.Build().Run();