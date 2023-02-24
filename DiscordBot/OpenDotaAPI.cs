using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml;

namespace DiscordBot
{
    internal class OpenDotaAPI
    {
        public static long GetSteamID32(string URL)
        {
            string steamID64 = "";
            long steamID32 = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load($"{URL}/?xml=1");
                XPathNavigator navigator = doc.CreateNavigator();
                XPathNodeIterator nodes = navigator.Select("/profile/steamID64");
                while (nodes.MoveNext())
                {
                    steamID64 = nodes.Current.ToString();
                }
                steamID32 = Convert.ToInt64(steamID64) - 76561197960265728;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return steamID32;
        }

        public static string SendGetRequestOpenDota(string URL)
        {
            System.Net.WebRequest reqGET = System.Net.WebRequest.Create(URL);
            System.Net.WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            return s;
        }
        
        public static string SendGetRequestOpenDotaIMG(string URL)
        {
            System.Net.WebRequest reqGET = System.Net.WebRequest.Create(URL);
            System.Net.WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            var s = sr.ReadToEnd();
            return s;
        }

        public static string? GetName(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}";
            var response = SendGetRequestOpenDota(URL);
            JObject jsonResponse = JObject.Parse(response);
            string name = (string)jsonResponse["profile"]["personaname"];
            return name;
        }

        public static async Task<JObject> GetJsonBuildAsync(string name)
        {
            string json = File.ReadAllText($@"..\..\..\build\{name}");
            var jsonResponse = JObject.Parse(json);
            return jsonResponse;
        }
    }
}
