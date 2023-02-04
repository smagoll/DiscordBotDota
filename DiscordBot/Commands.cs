﻿using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class Commands : InteractionModuleBase<SocketInteractionContext>
    {
        
        public static List<ApplicationCommandProperties> GetAllCommands()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
            applicationCommandProperties.Add(GetWardmap().Build());
            return applicationCommandProperties;
        }

        private static SlashCommandBuilder GetWardmap()
        {
            //wardmap
            var globalCommandWardmapObs = new SlashCommandBuilder();
            globalCommandWardmapObs.WithName("wardmap")
                                   .WithDescription("Create wardmap observer wards by the given link to the player")
                                   .AddOption(new SlashCommandOptionBuilder()
                                                    .WithName("obs")
                                                    .WithDescription("observer")
                                                    .WithType(ApplicationCommandOptionType.SubCommand)
                                                    .AddOption(new SlashCommandOptionBuilder()
                                                                    .WithName("link")
                                                                    .WithDescription("steam link user")
                                                                    .WithRequired(true)
                                                                    .WithType(ApplicationCommandOptionType.String)))             
                                   .AddOption(new SlashCommandOptionBuilder()
                                                    .WithName("sen")
                                                    .WithDescription("sentry")
                                                    .WithType(ApplicationCommandOptionType.SubCommand)
                                                    .AddOption(new SlashCommandOptionBuilder()
                                                                    .WithName("link")
                                                                    .WithDescription("steam link user")
                                                                    .WithRequired(true)
                                                                    .WithType(ApplicationCommandOptionType.String)));
            return globalCommandWardmapObs;
        }
    }
}
