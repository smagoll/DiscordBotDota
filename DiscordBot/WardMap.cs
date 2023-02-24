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


    internal class WardMap
    {

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

        private static JObject GetWardMap(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}/wardmap";
            var response = OpenDotaAPI.SendGetRequestOpenDota(URL);
            JObject jsonResponse = JObject.Parse(response);
            return jsonResponse;
        }

        public static System.Drawing.Image GetWardMapObs(string link)
        {
            var steamID32 = OpenDotaAPI.GetSteamID32(link);
            var wardMap = GetWardMap(steamID32);
            var image = CreateWardMap((JObject)wardMap["obs"]);
            return image;

        }
        
        public static System.Drawing.Image GetWardMapSen(string link)
        {
            var steamID32 = OpenDotaAPI.GetSteamID32(link);
            var wardMap = GetWardMap(steamID32);
            var image = CreateWardMap((JObject)wardMap["sen"]);
            return image;

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
    }
}
