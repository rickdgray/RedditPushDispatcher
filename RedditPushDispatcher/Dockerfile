FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RedditPushDispatcher/RedditPushDispatcher.csproj", "RedditPushDispatcher/"]
RUN dotnet restore "RedditPushDispatcher/RedditPushDispatcher.csproj"
COPY . .
WORKDIR "/src/RedditPushDispatcher"
RUN dotnet build "RedditPushDispatcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedditPushDispatcher.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedditPushDispatcher.dll"]