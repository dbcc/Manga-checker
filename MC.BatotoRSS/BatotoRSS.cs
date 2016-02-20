using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using MC.Database;
using MC.Models;

namespace MC.BatotoRSS {
    public static class BatotoRSS {
        public static List<List<object>> Get_feed_titles() {
            var settings = Sqlite.GetSettings();
            var url = settings["batoto_rss"];
            var mngstr = new List<List<object>>();
            if (url.Equals("")) {
                DebugText.Write($"[ERROR] batoto_rss is empty.");
                return mngstr;
            }
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            if (feed != null) {
                foreach (var mangs in feed.Items) {
                    var chapter = Regex.Match(mangs.Title.Text, @".+ ch\.(\d+).+", RegexOptions.IgnoreCase);

                    //Console.WriteLine(mangs.Title.Text);
                    mngstr.Add(new List<object> {
                        mangs.Title.Text,
                        chapter.Groups[1].Value,
                        mangs.Links[0].Uri.AbsoluteUri,
                        mangs.PublishDate.DateTime
                    });
                }
            }
            return mngstr;
        }

        public static void Check(IEnumerable<List<object>> feed, MangaModel manga, string openLinks) {
            var name = manga.Name;
            feed = feed.Reverse();
            foreach (var rssmanga in feed) {
                if (!rssmanga[0].ToString().ToLower().Contains(manga.Name.ToLower())) {
                    continue;
                }

                var t1 = (DateTime) rssmanga[3];
                var diff = DateTime.Compare(t1.ToUniversalTime(), manga.Date.ToUniversalTime());
                if (diff <= 0) {
                    continue;
                }
                string mangaTitle = (string) rssmanga[0];
                string link = (string) rssmanga[2];


                var ch = Regex.Match(mangaTitle, @".+ ch\.([\d\.]+):? r?", RegexOptions.IgnoreCase);
                var chapter = ch.Groups[1].Value.Trim();


                if (chapter == manga.Chapter) {
                    continue;
                }


                if (openLinks == "1") {
                    Process.Start(link);
                    Sqlite.UpdateManga("batoto", name, chapter, link, t1);
                    manga.Chapter = chapter;
                    manga.Date = t1;
                }
                DebugText.Write($"[Batoto] {mangaTitle} Found new Chapter");
            }
        }
    }
}