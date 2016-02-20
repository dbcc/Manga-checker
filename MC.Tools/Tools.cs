using System.Threading.Tasks;
using MC.Database;
using MC.Mangafox;
using MC.Mangahere;
using MC.Mangareader;
using MC.Mangastream;
using MC.Models;
using MC.Webtoons;
using MC.Yomanga;

namespace MC.Tools {
    public class Tools {

        public static void ChangeChaperNum(MangaModel item, string op) {
            if (!item.Chapter.Contains(" ")) {
                var chapter = int.Parse(item.Chapter);
                if (op.Equals("-")) {
                    chapter--;
                    var newDate = item.Date.AddDays(-1);
                    item.Date = newDate;
                } else {
                    chapter++;
                    var newDate = item.Date.AddDays(1);
                    item.Date = newDate;
                }

                item.Chapter = chapter.ToString();
                Sqlite.UpdateManga(item.Site, item.Name, item.Chapter, item.Link, item.Date, false);
            }
        }
    

        public static void RefreshManga(MangaModel manga) {
            var setting = Sqlite.GetSettings();
            switch (manga.Site) {
                case "mangareader": {
                    MangareaderHTML.Check(manga, setting["open links"]);
                    break;
                }
                case "mangastream": {
                    var feed = MangastreamRSS.Get_feed_titles();
                    MangastreamRSS.Check(manga, feed, setting["open links"]);
                    break;
                }
                case "mangafox": {
                    MangafoxRSS.Check(manga, setting["open links"]);
                    break;
                }
                case "mangahere": {
                    MangahereRSS.Check(manga, setting["open links"]);
                    break;
                }
                case "batoto": {
                    var feed = BatotoRSS.BatotoRSS.Get_feed_titles();
                    BatotoRSS.BatotoRSS.Check(feed, manga, setting["open links"]);
                    break;
                }
                case "yomanga": {
                    var feed = RSSReader.Read("http://yomanga.co/reader/feeds/rss") ??
                               RSSReader.Read("http://46.4.102.16/reader/feeds/rss");
                    YomangaRSS.Check(manga, feed, setting["open links"]);
                    break;
                }
                case "webtoons": {
                    WebtoonsRSS.Check(manga, setting["open links"]);
                    break;
                }
            }
        }
    }
}