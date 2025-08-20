using TwitchSharp;
using TwitchSharp.Events;
using TwitchSharp.Items;

public class Program
{
    public static async Task Main(string[] args)
    {
        ClientConfig Config = new()
        {
            ClientID = Variables.ClientID,
            ClientSecret = Variables.ClientSecret,
            Redirect_uri = "https://localhost:3000",
            Username = Variables.Username, // Username = Username of the acting user (bot)
            Scopes = [
                "user:bot",
                "user:read:chat",
                "user:write:chat",
                "chat:edit",
                "chat:read",
                "moderator:read:followers"
            ]
        };
        TwitchClient Client = new(Config);
        TwitchUser channel = new TwitchUser(Client, Variables.ChannelName); // ChannelName = username of twitch channel, where the bot is supposed to join


        var msg = new TwitchMessageBuilder(Client);
        msg.WithContent("Test");
        await msg.SendAsync(channel);

        Client.EventEngine.OnMessageReceived += async (s, e) =>
        {
            Console.WriteLine($"Received message > {e.Message.Channel} - {e.Message.Author}: {e.Message.Content} ({e.Message.ID})");
            if (e.Message.Content == "Ping") await e.Message.RespondAsync("Pong!");
        };
        await Client.EventEngine.SubscribeToEventAsnyc(channel, new(EventType.MessageReceived));

        Client.EventEngine.OnFollowReceived += async (s, e) =>
        {
            Console.Write("Follow Received: ");
            Console.WriteLine($"{e.Follower.DisplayName}");
            await e.Broadcaster.SendMessageAsync($"{e.Follower.DisplayName} just followed!");
        };
        await Client.EventEngine.SubscribeToEventAsnyc(channel, new(EventType.FollowReceived));

        while (true) ;
    }
}