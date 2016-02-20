using System;
using MC.Database;
using MC.Models;

namespace MC.Webtoons {
    public class WebtoonsGi {
        public static MangaModel GetInfo(string url) {
            var manga = new MangaModel();
            if (url.Contains("list?")) {
                url = url.Replace("list?", "rss?");
                try {
                    var rss = RSSReader.Read(url);
                    manga.Name = rss.Title.Text;
                    foreach (var item in rss.Items) {
                        DebugText.Write(item.Title.Text);
                        manga.Chapter = item.Title.Text.Replace("Ep. ", "");
                        manga.Link = item.Links[0].Uri.AbsoluteUri;
                        manga.RssLink = url;
                        manga.Site = "webtoons";
                        manga.Date = item.PublishDate.DateTime;
                        manga.Error = "null";
                        return manga;
                    }
                }
                catch (Exception e) {
                    DebugText.Write(e.Message);
                    return new MangaModel {
                        Error = "error"
                    };
                }
            }
            manga.Error = "error";
            return manga;
        }
    }
}