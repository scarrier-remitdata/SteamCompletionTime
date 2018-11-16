using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using RestSharp;

namespace SteamCompletionTime
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GetListButton_Click(object sender, EventArgs e)
        {
            var steamId = steamIdTextBox.Text;
            if (string.IsNullOrEmpty(steamId))
            {
                resultsTextBox.Text = "Enter a steam id";
                return;
            }
            var client = new RestClient("http://steamcommunity.com");
            var response = client.Get(new RestRequest(string.Format("/id/{0}/games/?tab=all&xml=1",steamId)));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                resultsTextBox.Text = response.Content;
                return;
            }

            var games = new Dictionary<string, GameInfo>();
            using (XmlReader reader = XmlReader.Create(new StringReader(response.Content)))
            {
                while (reader.ReadToFollowing("name"))
                {
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.CDATA)
                    {
                        var gameName = reader.Value;
                        gameName = Regex.Replace(gameName, @" - ", " ");//replace - with space
                        //get more results back if we leave the non-ascii in
                        //gameName = Regex.Replace(gameName, @"[^a-zA-Z0-9 ,.-]", string.Empty); //strip non-ascii
                        var excludeWord = @"(Enhanced Edition|Game of the Year.*|GOTY.*|Collector's Edition|\([^)]*\)|∞*∞|DLC|Director's Cut|
                                            Map Pack|Unit Pack|Maximum Edition|Original Soundtrack|Public Test|Steam Edition|Multiplayer|Single Player|
                                            Digital Deluxe|Digital Deluxe Bundle|Pre-Order|Edition Remastered|:|™|®)";
                        gameName = Regex.Replace(gameName, excludeWord, "", RegexOptions.IgnoreCase);
                        if (!games.ContainsKey(gameName))
                            games.Add(gameName, new GameInfo{Name = gameName});
                    }
                }
            }
            resultsTextBox.Text = string.Format("Loading {0} games", games.Keys.Count);
            resultsTextBox.Refresh();
            client = new RestClient("https://howlongtobeat.com");

            foreach (var name in games.Keys.ToList())
            {
                var request = new RestRequest("/search_main.php?t=games&page=1&sorthead=&sortd=Normal&plat=&detail=0");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("queryString", name, ParameterType.GetOrPost);
                client.PostAsync(request, (asyncResponse, handle) => HandleResponse(games, name, asyncResponse));
                Thread.Sleep(100);
            }
            while (games.Values.Any(g => !g.Retrieved))
            {
                Thread.Sleep(500);
            }
            var sortedList = games.Values.ToList().OrderByDescending(t => t.Hours).Select(t => t.Name + " - " + t.Hours);

            resultsTextBox.Text = string.Join("\r\n", sortedList.ToArray());
        }

        private void HandleResponse(Dictionary<string, GameInfo> games, string name, IRestResponse response)
        {
            games[name].Retrieved = true;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            var node = doc.DocumentNode.SelectNodes("//div//*[contains(@class, 'search_list_tidbit center time_') and not(contains(text(), '--')) or contains(@class, 'search_list_tidbit_long center time_') and not(contains(text(), '--'))]");
            if (node == null)
                return;
            var hours = node[0].InnerText;
            if (hours.Contains("Mins"))
            {
                hours = Regex.Replace(hours, "Mins", "");
                hours.Trim();
                float.TryParse(hours, out float minsToHours);
                minsToHours /= 60f;
                games[name].Hours = minsToHours;
            }
            else
            {
                hours = Regex.Replace(hours, "Hours", "");
                hours.Trim();
                hours = Regex.Replace(hours, "&#189;", ".5");
                float.TryParse(hours, out float hoursFloat);
                games[name].Hours = hoursFloat;
            }
        }

    }

    internal class GameInfo
    {
        public string Name { get; set; }
        public float Hours { get; set; }
        public bool Retrieved { get; set; }
    }
}
