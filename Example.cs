using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwitchSharp;
using TwitchSharp.Entitys;
using TwitchSharp.Events;

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
        #region Logging
        /*
        # # # # #  
         LOGGING
        # # # # # 
        
        TwitchSharp has its own Logging System implemented.
        Because the Logging System is used in the entire TwitchSharp Library, it should be the first thing to be configured

        You can access the System with TwitchEngine.Logs.

        .ConfigureLogging() allows you to configure what will be logged, and how the prefix will look like.
        -> In the example below, I'll configure everything to the default configuration - Normally not needed if you want to leave it like that!

        .OnLog() is an event that is thrown everytime a log is created. it contains the string that is supposed to log, the log level and an exception (if given)
        Note: .OnLog() also logs the messages, below the minimal log level

        Technically you also could Log your own messages with .Log()
        */
        TwitchSharpLogs logs = TwitchEngine.Logs;
        logs.ConfigureLogging(new LogConfig()
        {
            /* 
            The Prefix will be shown at the start of every log.
            The following Variables are availible:
            - "{DateTime}" - Shows the current Date and Time (Further configuration with DateTimeFormat)
            - "{LogLevel}" - Shows the LogLevel, the current log has (Further configuration with MinimalLogLevel)
            */
            Prefix = "[{DateTime} TwitchSharp - {LogLevel}] ",
            /*
            DateTime uses the orinary DateTime variable. 
            The format options can be found at https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
            */
            DateTimeFormat = "dd.MM.yyyy HH:mm:ss",
            /*
            Changes the Color of the Prefix
            Note: The LogLevel will be shown in a different Color, which cannot be changed!
            */
            PrefixColor = ConsoleColor.Magenta,
            /*
            Configures the minimal log level, that is getting logged. The following log levels are availible:
            - LogLevel.Trace        - This will show every incoming and outcoming api and websocked traffic
            - LogLevel.Debug        - This will show information that might be needed for debugging options
            - LogLevel.Information  - This will show basic information that might be interesting at normal behaviour
            - LogLevel.Warning      - This will show any warnings, that might occure
            - LogLevel.Error        - This will show any Errors that might occure
            - LogLevel.Critical     - This will show any Critical Errors that probably needs you to restart your application
            */
            MinimalLogLevel = LogLevel.Information
        });

        logs.OnLog += (message, logLevel, Exception) =>
        {
            if (logLevel < LogLevel.Information) return; // This would prevent spam from .Trace or .Debug logs
            /*
            probably a system to store your log in a file and/or database
            */
        };

        logs.Log("This is a custom log by me!", LogLevel.Information);
        #endregion

        #region TwitchClient
        /*
        # # # # #
         Client
        # # # # #

        The Client can be created using the following options:
        - Option 1 (Automatic)
            This Option will use the AutomaticTwitchClientConfig() which requires the OAuthCode.
            This method is the easiest verison, if you want to "automate" the OAuthCode generation (https://dev.twitch.tv/docs/authentication/getting-tokens-oauth/#authorization-code-grant-flow)

        - Option 2 (Manual)
            This Option will use the ManualTwitchClientConfig() which requires an RedirectUri and a list of Scopes
            When using this method TwitchSharp will send an information log with an authorization link. 
            You then need to Authorize your twitch account, and wait until twitch redirects you to your given redirect uri.

            The new url you got redirected should look like this:
            https://localhost:3000/?code=b95189bekh8swuexmsfisxrdhxk8sp&scope=user%3Abot+user%3Aread%3Achat+user%3Aread%3Awhispers+user%3Awrite%3Achat+user%3Amanage%3Awhispers
            The important part is the following:
            code=b95189bekh8swuexmsfisxrdhxk8sp

            Copy that code and insert it into the console
        
        - Option 3 (Refresh)
            This Option will use the RefreshTwitchClientConfig() which requires an refresh token

            To gather an refresh token, you'll need to authorize your application and get an user access token. 
            When sending the api request to gather the user access token, you also get an refresh token, which you can insert to your config.

            Note: The Refresh token won't be valid forever and you might need to insert a new token after restarting your application 

        Note: In all Options will also need an ClientID and ClientSecret, which you'll can get from the twitch dev console
        */

        // OPTION 1 (Disabled in example)
        /*
        AutomaticTwitchClientConfig clientConfig = new AutomaticTwitchClientConfig()
        {
            ClientID = Variables.ClientID,
            ClientSecret = Variables.ClientSecret,
            Username = "xsophbot", // Name of the account the bot will use (has to match the account you authorize)
            OAuthCode = "InsertCodeHere"
        };
        TwitchEngine.CreateTwitchClient(clientConfig);
        */

        // OPTION 2
        ManualTwitchClientConfig clientConfig = new ManualTwitchClientConfig()
        {
            ClientID = Variables.ClientID,
            ClientSecret = Variables.ClientSecret,
            Username = "xsophbot", // Name of the account the bot will use (has to match the account you authorize)
            RedirectUri = "https://localhost:3000",
            Scopes = [
                // Full list of scopes at https://dev.twitch.tv/docs/authentication/scopes/#twitch-access-token-scopes 
                "user:bot",             // Required to use the authorized account as bot account
                "user:read:chat",       // Required to read messages in public chat
                "user:read:whispers",   // Required to read messages in private chat
                "user:write:chat",      // Required to write messages in public chat
                "user:manage:whispers", // Requried to write messages in private chat
            ]
        };
        TwitchEngine.CreateTwitchClient(clientConfig);

        // OPTION 3 (Disabled in example)
        /*
        RefreshTwitchClientConfig clientConfig = new RefreshTwitchClientConfig()
        {
            ClientID = Variables.ClientID,
            ClientSecret = Variables.ClientSecret,
            Username = "xsophbot", // Name of the account the bot will use (has to match the account you authorize)
            RefreshToken = "InsertTokenHere"
        };
        TwitchEngine.CreateTwitchClient(clientConfig);
        */

        TwitchClient Client = TwitchEngine.GetTwitchClient();
        Client.OnReady += (s) =>
        {
            Console.Clear();
            logs.Log("Client is now ready and running!", LogLevel.Information);
        };
        await Client.StartAsync();

        #endregion

        #region TwitchUser
        /*
        The base TwitchUser contains the default information and methods, which don't need further interaction from 
        the given user - like Authorization / sending messages / etc.
        */
        TwitchUser User = new TwitchUser("xsophbot"); 
        await User.SendMessageAsync("Hello World!");

        #endregion

        #region Events
        /*
        Before you can subscribe events, you'll have to configure the event engine
        You'll have to configure:
        - The Client, which is supposed to listen to events
        - A list of Events, that is supposed to be subscribed  
        */
        var EventConfig = new EventConfig()
        {
            Client = Client,
            /* 
            Note: Client.CurrentUser is the user, where the library is supposed to listen to events - Some Events will not need it!
             -> For an real enviroment, you probably want to change Client.CurrentUser with a real user (new TwitchUser("userNameOfStreamer"))
            */
            Subscriptions = [
                new EventSubscription(EventType.ChannelMessageReceived, Client.CurrentUser),
                new EventSubscription(EventType.ChannelFollowReceived, Client.CurrentUser),
                new EventSubscription(EventType.PrivateMessageReceived, Client.CurrentUser),
            ]
        };

        var EventEngine = TwitchEngine.UseEvents(EventConfig);
        EventEngine.OnChannelMessageReceived += async (s, e) =>
        {
            if (e.MessageContent.Contains("!ping")) await e.Broadcaster.SendMessageAsync("Pong!", e.MessageID);
        };
        EventEngine.OnPrivateMessageReceived += async (s, e) =>
        {
            await e.Sender.SendWhisperAsync("Hello!");
        };
        await EventEngine.StartListeningAsync();
        
        #endregion

        while (true) ; // Keep program running
    }
}