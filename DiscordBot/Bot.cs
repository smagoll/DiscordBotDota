using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class Bot
    {
        public static DiscordSocketClient _client;

        public static Task Main(string[] args) => new Bot().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            var token = "your token";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task Client_Ready()
        {
            var commands = Commands.GetAllCommands();
            try
            {
                //await _client.CreateGlobalApplicationCommandAsync(globalCommandWardmap.Build());
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "wardmap":             
                    await ResponseCommands.HandleWardmapCommand(command);
                    break;
                case "stats":
                    await ResponseCommands.HandleStatsCommand(command);
                    break;
            }
        }

        public static async Task SendWardMap(ulong id, string link, bool isObs)
        {
            System.Drawing.Image wardMap;
            if (isObs)
            {
                wardMap = WardMap.GetWardMapObs(link);
            }
            else
            {
                wardMap = WardMap.GetWardMapSen(link);
            }

            wardMap.Save($@"..\..\..\resourses\{id}.jpg");
            var channel = _client.GetChannel(id) as IMessageChannel;
            await channel.SendFileAsync($@"..\..\..\resourses\{id}.jpg");
        }
        
        public static async Task SendStats(ulong id, string link)
        {
            var info = MatchStats.GetInfo(link);
            var channel = _client.GetChannel(id) as IMessageChannel;
            var msg = await channel.SendMessageAsync("1 second...");
            await msg.ModifyAsync(msg => msg.Content = info);
        }
    }
}
