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
                        gameName = Regex.Replace(gameName, @"[^a-zA-Z0-9 ,.-]", string.Empty); //strip non-ascii
                        gameName = Regex.Replace(gameName, "Enhanced Edition", "", RegexOptions.IgnoreCase);
                        gameName = Regex.Replace(gameName, "Game of the Year.*", "", RegexOptions.IgnoreCase);
                        gameName = Regex.Replace(gameName, "GOTY.*", "", RegexOptions.IgnoreCase);
                        if (!games.ContainsKey(gameName))
                            games.Add(gameName, new GameInfo{Name = gameName});
                    }
                }
            }
            resultsTextBox.Text = string.Format("Loading {0} games", games.Keys.Count);
            resultsTextBox.Refresh();
            client = new RestClient("http://www.howlongtobeat.com");

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

            resultsTextBox.Text = string.Join("\r\n", sortedList);
        }

        private void HandleResponse(Dictionary<string, GameInfo> games, string name, IRestResponse response)
        {
            games[name].Retrieved = true;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'gamelist_tidbit time_')]");
            if (node == null)
                return;
            var hours = node.InnerText;
            hours = Regex.Replace(hours, " Hours", "");
            hours = Regex.Replace(hours, "&#189;", ".5");
            var hoursFloat = 0.0f;
            float.TryParse(hours, out hoursFloat);
            games[name].Hours = hoursFloat;
        }

    }

    internal class GameInfo
    {
        public string Name { get; set; }
        public float Hours { get; set; }
        public bool Retrieved { get; set; }
    }
}
