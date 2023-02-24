using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class ResponseCommands
    {
        public static async Task HandleWardmapCommand(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.First();
            if (subCommand.Name == "obs")
            {
                var link = subCommand.Options.First().Value.ToString();
                await command.RespondAsync($"1 second...");
                await Bot.SendWardMap((ulong)command.ChannelId, link, true);
            }
            else
            {
                var link = subCommand.Options.First().Value.ToString();
                await command.RespondAsync($"1 second...");
                await Bot.SendWardMap((ulong)command.ChannelId, link, false);
            }
        }

        public static async Task HandleStatsCommand(SocketSlashCommand command)
        {
            var link = command.Data.Options.First().Value.ToString();
            //await Bot.SendStats((ulong)command.ChannelId, link);
            var channel = Bot._client.GetChannel((ulong)command.ChannelId) as IMessageChannel;
            var msg = await channel.SendMessageAsync(" 1 second...");
            var info = MatchStats.GetInfo(link);
            await msg.ModifyAsync(msg => msg.Content = info);


        }
    }
}
