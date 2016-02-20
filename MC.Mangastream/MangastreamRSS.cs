using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using MC.Database;
using MC.Models;

namespace MC.Mangastream {
    public static class MangastreamRSS {
        public static void Check(MangaModel manga, IEnumerable<List<object>> mslist, string openLinks) {
            mslist = mslist.Reverse();
            foreach (var m in mslist) {
                if (!m[0].ToString().ToLower().Contains(manga.Name.ToLower())) {
                    continue;
                }

                var t1 = (DateTime) m[2];
                var diff = DateTime.Compare(t1.ToUniversalTime(), manga.Date.ToUniversalTime());
                if (diff <= 0) {
                    continue;
                }

                var mangaTitle = m[0].ToString()
                    .Substring(0, m[0].ToString().LastIndexOf(" "));
                var link = m[1].ToString();

                var ch = Regex.Match(m[0].ToString(), $@"{mangaTitle} (.+)", RegexOptions.IgnoreCase);
                var chapter = ch.Groups[1].Value.Trim();

                if (chapter == manga.Chapter) {
                    continue;
                }

                if (openLinks == "1") {
                    Process.Start(link);
                    Sqlite.UpdateManga("mangastream", manga.Name, chapter, link, t1);
                    manga.Chapter = chapter;
                    manga.Date = t1;
                }
                DebugText.Write($"[Mangastream] {manga.Name} {chapter} Found new Chapter");
            }
        }

        public static List<List<object>> Get_feed_titles() {
            const string url = "http://Mangastream.com/rss";
            var mngstr = new List<List<object>>();

            try {
                string xml;
                using (var webClient = new WebClient()) {
                    xml = webClient.DownloadString(url);
                }

                xml = Regex.Replace(xml, @"&.+;", "A");

                // SyndicationFeed can't parse the pubDate so we need to do a workarond
                xml = xml.Replace("pubDate", "pubDateBroke");
                var rawTimes =
                    Regex.Matches(xml, "<pubDateBroke>(.+)</pubDateBroke>")
                        .OfType<Match>()
                        .Select(m => m.Groups[1].Value)
                        .ToArray();

                var reader = new XmlTextReader(new StringReader(xml));
                var feed = SyndicationFeed.Load(reader);
                if (feed == null) {
                    return mngstr;
                }

                var count = 0;
                foreach (var mangs in feed.Items) {
                    var pubDate = DateTime.ParseExact(
                        rawTimes[count],
                        "ddd, dd MMM yyyy h:mm:ss zz00",
                        CultureInfo.GetCultureInfoByIetfLanguageTag("en-us"));

                    mngstr.Add(new List<object> {mangs.Title.Text, mangs.Id, pubDate});
                    count++;
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mngstr;
        }
    }
}