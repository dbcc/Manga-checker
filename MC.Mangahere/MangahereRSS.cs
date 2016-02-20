using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MC.Database;
using MC.Models;

namespace MC.Mangahere {
    public static class MangahereRSS {
        private static void Get_feed_titles(string url, MangaModel manga, string openLinks) {
            var chPlus = int.Parse(manga.Chapter);
            chPlus++;
            var feed = RSSReader.Read(url);
            if (feed == null) return;
            foreach (var mangs in feed.Items) {
                //ParseFile.setManga("mangahere", name, chapter);
                if (mangs.Title.Text.ToLower().Contains(chPlus.ToString().ToLower())) {
                    if (openLinks == "1") {
                        Process.Start(mangs.Links[0].Uri.AbsoluteUri);
                        Sqlite.UpdateManga("mangahere", manga.Name, chPlus.ToString(), mangs.Links[0].Uri.AbsoluteUri,
                            DateTime.Now);
                    }

                    manga.Chapter = chPlus.ToString();
                    DebugText.Write($"[Mangahere] {manga.Name} {chPlus} Found new Chapter");
                }
            }
        }

        public static void Check(MangaModel manga, string openLinks) {
            var name = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "_").Trim('_').ToLower();
            //DebugText.Write(Regex.Replace("tes__ygr___rhut_","[^0-9a-zA-Z]+","_").Trim('_')); //test regex output
            try {
                Get_feed_titles("http://mangahere.co/rss/" + name + ".xml",
                    manga, openLinks);
            } catch (Exception e) {
                //Main.DebugTextBox.Text += string.Format("[MangahereRSS] Error {0} {1}", manga, e);
                DebugText.Write($"[Mangahere] Error {manga.Name} {e.Message} {name}.");
            }
        }
    }
}
