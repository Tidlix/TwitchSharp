using TwitchSharp;
using TwitchSharp.Items;

public class Program
{
    public static async Task Main(string[] args)
    {
        ClientConfig Config = new()
        {
            ClientID = "",
            ClientSecret = "",
            Redirect_uri = "https://localhost:3000",
            Username = "",
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
        TwitchUser channel = new TwitchUser(Client, "");


        var msg = new TwitchMessageBuilder(Client);
        msg.WithContent("Joined Channel as Test bot");
        await msg.SendAsync(channel);

        Client.EventEngine.OnMessageReceived += async (s, e) =>
        {
            Console.WriteLine($"Received message > {e.Message.Channel} - {e.Message.Author}: {e.Message.Content} ({e.Message.ID})");
            if (e.Message.Content == "Ping") await e.Message.RespondAsync("Pong!");
        };
        await Client.EventEngine.StartListeningForMessagesAsync(channel);

        while (true) ;
    }
}