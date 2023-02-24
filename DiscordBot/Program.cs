using DiscordBot;
using System;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Bot.Main(args);
        //var a = OpenDotaAPI.GetSteamID32("https://steamcommunity.com/profiles/76561198199492125");
        //var q = MatchStats.GetInfo("https://steamcommunity.com/profiles/76561198199492125");
        //var q = MatchStats.GetInfo("qwe");
        //var img = MatchStats.GetImageHero(2);
    }

}
