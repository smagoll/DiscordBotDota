using System.Xml;
using System.Xml.XPath;
using System.Text.Json;
using Newtonsoft.Json.Linq;

class InfoPlayer
{
    public string avatar { get; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var steamID32 = SendGetRequestSteamID("https://steamcommunity.com/profiles/76561198199492125/?xml=1");
        Console.WriteLine(steamID32);
        GetName(steamID32);
        GetMatches(steamID32);



        long SendGetRequestSteamID(string URL)
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

        string SendGetRequestOpenDota(string URL)
        {
            System.Net.WebRequest reqGET = System.Net.WebRequest.Create(URL);
            System.Net.WebResponse resp = reqGET.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            return s;
        }

        void GetName(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}";
            var response = SendGetRequestOpenDota(URL);
            JObject jsonResponse = JObject.Parse(response);
            string name = (string)jsonResponse["profile"]["personaname"];
            Console.WriteLine(name);
        }

        void GetMatches(long steamID32)
        {
            string URL = $@"https://api.opendota.com/api/players/{steamID32}/matches";
            var response = SendGetRequestOpenDota(URL);
            JArray jsonResponse = JArray.Parse(response);
            var qwe = JArray.FromObject(jsonResponse.Take(100));
            //JArray qwqwe = JArray.Parse(qwe);
            //IList<string> count = jsonResponse.Select(x => (string)x).ToList();
            Console.WriteLine(qwe);
        }
    }
}