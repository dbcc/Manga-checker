using System;
using System.Diagnostics;
using MC.Database;
using MC.Models;

namespace MC.Webtoons {
    public class WebtoonsRSS {
        // soon...
        public static void Check(MangaModel manga, string openLinks) {
            try {
                var Name = manga.Name;
                var Chapter = int.Parse(manga.Chapter);
                Chapter++;
                var Url = manga.RssLink;
                var rssitems = RSSReader.Read(Url);
                if (rssitems == null) return;
                foreach (var rssitem in rssitems.Items) {
                    if (rssitem.Title.Text.Contains(Chapter.ToString())) {
                        if (openLinks.Equals("1")) {
                            Process.Start(rssitem.Links[0].Uri.AbsoluteUri);
                            ParseFile.SetManga("webtoons", Name, Chapter.ToString());
                            Sqlite.UpdateManga(
                                "webtoons",
                                Name,
                                Chapter.ToString(),
                                rssitem.Links[0].Uri.AbsoluteUri,
                                DateTime.Now);
                            DebugText.Write($"[Webtoons] Found new Chapter {Name} {rssitem.Title.Text}.");
                        }
                    }
                }
            }
            catch (Exception ex) {
                DebugText.Write($"[Webtoons] Error {ex.Message}.");
            }
        }
    }
}