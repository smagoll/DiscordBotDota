using System.Xml;
using System.Xml.XPath;
using System.Text.Json;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using HeatMap;
using static System.Collections.Specialized.BitVector32;

namespace DiscordBot
{
    class Hero
    {
        private int win = 0;
        private int lose = 0;
        public int IdHero { get; set; }
        public int Win
        {
            get { return win; }
            set { win = value; }
        }
        public int Lose
        {
            get { return lose; }
            set { lose = value; }
        }
        public int CountMatches
        {
            get { return win + lose; }
        }
        public int PercentWin
        {
            get { return (100 / CountMatches * win); }
        }
    }

    internal class WardMap
    {
        public static List<Hero> heroes = new();

        private static void Main(string[] args)
        {
        
            //var steamID32 = GetSteamID32("https://steamcommunity.com/profiles/76561198199492125/?xml=1");
            //string? name = GetName(steamID32);
            //var matchesReceived = GetMatches(steamID32);
            //AddHeroWL(matchesReceived);
            //var heroesSorted = heroes.OrderByDescending(x => x.CountMatches).OrderByDescending(x => x.Win).Take(5).ToList();
            //WriteInfoHero(heroesSorted);
            //var wardMap = GetWardMap(steamID32);
            //CreateObsWardMap(wardMap);
            //Console.WriteLine(steamID32);
        }

        

        public static long GetSteamID32(string URL)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(URL);
            string steamID64 = "";
            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator nodes = navigator.Select("/profile/steamID64");
            while (nodes.MoveNext())
            {
                steamID64 = nodes.Current.ToString();
            }

            var steamID32 = Convert.ToInt64(steamID64) - 76561197960265728;
            return steamID32;
        }

        private static string SendGetRequestOpenDota(string URL)
        {
            System.Net.WebRequest reqGET = System.Net.WebRequest.Create(URL);
            System.Net.WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            return s;
        }

        private static string? GetName(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}";
            var response = SendGetRequestOpenDota(URL);
            JObject jsonResponse = JObject.Parse(response);
            string name = (string)jsonResponse["profile"]["personaname"];
            return name;
        }

        private static JObject GetWardMap(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}/wardmap";
            var response = SendGetRequestOpenDota(URL);
            JObject jsonResponse = JObject.Parse(response);
            return jsonResponse;
        }

        public static System.Drawing.Image GetWardMapObs(string link)
        {
            var steamID32 = GetSteamID32($@"{link}/?xml=1");
            var wardMap = GetWardMap(steamID32);
            var image = CreateWardMap((JObject)wardMap["obs"]);
            return image;

        }
        
        public static System.Drawing.Image GetWardMapSen(string link)
        {
            var steamID32 = GetSteamID32($@"{link}/?xml=1");
            var wardMap = GetWardMap(steamID32);
            var image = CreateWardMap((JObject)wardMap["sen"]);
            return image;

        }

        private static JArray GetMatches(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}/matches";
            var response = SendGetRequestOpenDota(URL);
            JArray jsonResponse = JArray.Parse(response);
            var arrayMatches = JArray.FromObject(jsonResponse.Take(100));
            return arrayMatches;
        }

        private static async Task<JObject> GetJsonBuildAsync(string name)
        {
            string json = File.ReadAllText($@"..\..\..\build\{name}");
            var jsonResponse = JObject.Parse(json);
            return jsonResponse;
        }

        private static string? GetHero(int id)
        {
            JObject hero = GetJsonBuildAsync("heroes.json").Result;
            string localized_name = (string)hero[$"{id}"]["localized_name"];
            return localized_name;
        }

        private static Color HeatMapColor(decimal value, decimal min, decimal max)
        {
            decimal val = (value - min) / (max - min);
            int r = Convert.ToByte(255 * val);
            int g = Convert.ToByte(255 * (1 - val));
            int b = 0;

            return Color.FromArgb(255 / 100 * 100, r, g, b);
        }

        private static void DrawEllipse(System.Drawing.Image image, int x, int y, int freq, int max)
        {
            var color = HeatMapColor(freq, 0, max);
            Color brushColor = Color.FromArgb(250 / 100 * 10, 255, 255, 0);
            System.Drawing.SolidBrush brush = new SolidBrush(color);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.FillEllipse(brush, x, y, 17, 17);
                var a = "..\\..\\..\\resourses\\minimap__sen.jpg";

            }
        }

        private static System.Drawing.Image CreateWardMap(JObject wardMap)
        {
            var a = "..\\..\\..\\resourses\\minimap.jpg";
            System.Drawing.Image image = System.Drawing.Image.FromFile(a);

            int max = int.MinValue;
            foreach (var w in wardMap)//search maxElement
            {
                foreach (var t in (JObject)w.Value)
                {
                    if (int.Parse((string)t.Value) > max)
                    {
                        max = int.Parse((string)t.Value);
                    }
                }
            }
            List<DataType> datas = new List<DataType>();
            foreach (var ward in wardMap)
            {
                var x = (int.Parse(ward.Key) - 64) * 1024 / 128;
                var arrayY = ward.Value;
                foreach (var q in (JObject)arrayY)
                {
                    var y = 1024 - ((int.Parse(q.Key) - 64) * 1024 / 128);
                    for (int i = 0; i < int.Parse((string)q.Value); i++)
                    {
                        //DrawEllipse(image, x, y, int.Parse((string)q.Value), max);
                        DataType data = new DataType();
                        data.X = x;
                        data.Y = y;
                        data.Weight = int.Parse((string)q.Value);
                        datas.Add(data);
                    }
                }
            }
            HeatMapImage qwe = new HeatMapImage(1024, 1024, 150, 10);//создание HeatMap
            qwe.SetDatas(datas);//установка данных
            var e = qwe.GetHeatMap();//получение HeatMap
            System.Drawing.Image ee = (System.Drawing.Image)e;
            using (var graphics = Graphics.FromImage(image))//наложение HeatMap на карту
            {
                graphics.DrawImage(ee, 0, 0);
            }
            return image;
            //image.Save($"..\\..\\..\\resourses\\minimap_{wardMap.Path}.jpg");
        }

        private static void AddHeroWL(JArray matches)
        {
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
        }

        private static void WriteInfoHero(List<Hero> heroInfo)
        {
            foreach (var q in heroInfo)
            {
                Console.WriteLine("Герой: " + GetHero(q.IdHero));
                Console.WriteLine("Количество игр: " + q.CountMatches.ToString());
                Console.WriteLine("Процент побед: " + q.PercentWin.ToString() + "%");
                Console.WriteLine("Победы: " + q.Win.ToString());
                Console.WriteLine("Поражения: " + q.Lose.ToString());
                Console.WriteLine("---------");
            }
        }
    }
}
