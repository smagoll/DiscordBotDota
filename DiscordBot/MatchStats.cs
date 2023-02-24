using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DiscordBot
{
    internal class MatchStats
    {

        private static JArray GetMatches(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}/matches";
            var response = OpenDotaAPI.SendGetRequestOpenDota(URL);
            JArray jsonResponse = JArray.Parse(response);
            var arrayMatches = JArray.FromObject(jsonResponse.Take(100));
            return arrayMatches;
        }


        private static string? GetHero(int id)
        {
            //JObject hero = OpenDotaAPI.GetJsonBuildAsync("heroes.json").Result;
            string URL = "https://api.opendota.com/api/heroes";
            var response = OpenDotaAPI.SendGetRequestOpenDota(URL);
            JObject hero = JObject.Parse(response);
            string localized_name = (string)hero[$"{id}"]["localized_name"];
            return localized_name;
        }
        
        public static string? GetImageHero(int id)
        {
            JObject hero = OpenDotaAPI.GetJsonBuildAsync("heroes.json").Result;
            string linkImg = "https://api.opendota.com" + (string)hero[$"{id}"]["icon"];
            return linkImg;
        }

        private static List<Hero> AddHeroWL(JArray matches)
        {
            List<Hero> heroes = new();

            foreach (var match in matches)
            {

                int idHero = int.Parse((string)match["hero_id"]);
                var playerSlot = int.Parse((string)match["player_slot"]);
                var radiantWin = bool.Parse((string)match["radiant_win"]);
                bool result = false;
                if (playerSlot < 128)//0-127 radiant, 127-255 dire
                {
                    if (radiantWin)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false; ;
                    }
                }
                else
                {
                    if (radiantWin)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                if (heroes.Where(x => x.IdHero == idHero).Count() == 0)//проверка есть ли такой герой в списке
                {
                    Hero hero = new Hero();
                    hero.IdHero = idHero;
                    if (result)
                    {
                        hero.Win++;
                    }
                    else
                    {
                        hero.Lose++;
                    }
                    heroes.Add(hero);
                }
                else
                {
                    var hero = heroes.Where(x => x.IdHero == idHero).ToArray()[0];
                    if (result)
                    {
                        hero.Win++;
                    }
                    else
                    {
                        hero.Lose++;
                    }
                }
            }

            return heroes;
        }

        public static string HeroesToString(List<Hero> heroInfo)
        {

            string info = "```";
            foreach (var hero in heroInfo)
            {
                info += hero.CountMatches.ToString();
                info += " | " + hero.PercentWin + "%";
                info += " | " + GetHero(hero.IdHero) +"\n";
            }
            info += "```";
            return info;
        }

        public static string GetInfo(string link)
        {
            var steamID32 = OpenDotaAPI.GetSteamID32(link);
            if (steamID32 == 0)
            {
                return "Invalid URL";
            }
            var matches = GetMatches(steamID32);
            var heroes = AddHeroWL(matches);
            var heroesSorted = heroes.OrderByDescending(x => x.CountMatches).OrderByDescending(x => x.Win).Take(5).ToList();
            var info = HeroesToString(heroesSorted);
            return info;
        }
    }
}
