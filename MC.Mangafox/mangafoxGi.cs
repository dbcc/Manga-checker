using System;
using System.Net;
using System.Text.RegularExpressions;
using MC.Database;
using MC.Models;

namespace MC.Mangafox {
    public class MangafoxGi {
        public static MangaModel GeInfo(string url) {
            var InfoViewModel = new MangaModel();
            var web = new WebClient();
            try {
                //title="RSS" href="/rss/one_piece.xml"/><link
                var source = web.DownloadString(url);
                var rsslink = Regex.Match(source, "title=\"RSS\" href=\"(.+)\"/>", RegexOptions.IgnoreCase);
                var rss = RSSReader.Read("http://mangafox.me" + rsslink.Groups[1].Value);

                if (rss.Equals(null)) {
                    InfoViewModel.Error = "null";
                    return InfoViewModel;
                }

                foreach (var item in rss.Items) {
                    if (!item.Title.Text.ToLower().Contains("vol")) {
                        var title = Regex.Match(item.Title.Text, "(.+) Ch (.+)");
                        InfoViewModel.Name = title.Groups[1].Value;
                        InfoViewModel.Chapter = title.Groups[2].Value;
                        InfoViewModel.Link = item.Links[0].Uri.AbsoluteUri;
                    }
                    else {
                        var title = Regex.Match(item.Title.Text, "(.+) Vol.+ Ch (.+)");
                        InfoViewModel.Name = title.Groups[1].Value.Trim();
                        InfoViewModel.Chapter = title.Groups[2].Value.Trim();
                        InfoViewModel.Link = item.Links[0].Uri.AbsoluteUri;
                    }
                    InfoViewModel.Site = "mangafox.me";
                    InfoViewModel.Error = "null";
                    break;
                }
            }
            catch (Exception e) {
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
            return InfoViewModel;
        }
    }
}