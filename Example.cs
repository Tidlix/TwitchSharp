using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwitchSharp;
using TwitchSharp.Entitys;
using TwitchSharp.Events.Types;

public class Example
{
    /*
    # # # # # # # # # # # 
     TwitchSharp Example
    # # # # # # # # # # # 

    This class is an basic example, how you could create your own Twitch Bot
    */
    public static async Task Main(string[] args)
    {
        // You can configure a few things on how TwitchSharp works, by modifying the default values of the TwitchSharpEngine
        TwitchSharpEngine.ModifyEngine(
            consoleLevel: TwitchSharpEngine.ConsoleLevel.Information,
            showTime: false,
            showConsoleLevel: false,
            dateTimeFormat: "dd/MM/yyyy - HH:mm:ss"
        );
        // note: in the example above, I've configured everything to the default value. If you want to keep a value default, you can do that by leaving it null

        // OPTIONAL - TwitchSharp method to ask for authorization - Preferred method is storing refresh token and insert without manual registration 
        string refreshToken = await TwitchSharpEngine.GenerateRefreshTokenAsync(
                new TwitchRefreshTokenConfig()
                {
                    ClientID = Variables.ClientID,
                    ClientSecret = Variables.ClientSecret,
                    RedirectUri = "https://localhost:3000",
                    Scopes = [
                        "user:bot",
                        "user:read:chat",
                        "user:read:whispers",
                        "user:write:chat",
                        "user:manage:whispers"
                    ]
                }
            );

        TwitchClientConfig conf = new()
        {
            ClientID = Variables.ClientID,
            ClientSecret = Variables.ClientSecret,
            RefreshToken = refreshToken
        };

        TwitchClient Client = new(conf);
        Console.WriteLine($"Client is running on {Client.CurrentUser.DisplayName}");

        TwitchUser me = await Client.GetUserByLoginAsync("tidlix");
        await me.SendChatMessageAsync("Hello World!");
        await me.SendWhisperAsync("Just joined your channel!");

        var EventHandler = Client.UseEvents();
        // MUST BE DONE BEFORE SUBSCRIBING TO THE EVENT!!!
        EventHandler.OnChannelChatMessageReceived += async (s, e) =>
        {
            Console.WriteLine("msg received: " + e.MessageContent);
            if (e.MessageContent == "!ping") await e.Broadcaster.SendChatMessageAsync("!pong", e.MessageID);
        };
    
        await EventHandler.SubscribeToEventAsync(new ChannelChatMessageReceivedEvent(me));

        while (true) ; // Keep program running
    }
}