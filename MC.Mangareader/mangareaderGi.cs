using System;
using System.Net;
using System.Text.RegularExpressions;
using MC.Models;

namespace MC.Mangareader {
    public class MangareaderGi {
        public static MangaModel GetInfo(string url) {
            var InfoViewModel = new MangaModel();
            try {
                var web = new WebClient();
                var html = web.DownloadString(url);
                var name = Regex.Match(html, "<h2 class=\"aname\">(.+)</h2>", RegexOptions.IgnoreCase);
                if (name.Success) {
                    var chapter = Regex.Match(html, "<a href=\"(.+)\">(.+) (\\d+)</a>", RegexOptions.IgnoreCase);
                    InfoViewModel.Name = name.Groups[2].Value.Trim();
                    if (chapter.Success && chapter.Groups[2].Value == name.Groups[1].Value) {
                        InfoViewModel.Name = name.Groups[1].Value;
                        InfoViewModel.Chapter = chapter.Groups[3].Value.Trim();
                        InfoViewModel.Site = "mangareader.net";
                        InfoViewModel.Error = "null";
                        InfoViewModel.Link = "http://" + InfoViewModel.Site + chapter.Groups[1].Value.Trim();
                        return InfoViewModel;
                    }
                }
            }
            catch (Exception e) {
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
                // do stuff here
            }
            return InfoViewModel;
        }
    }
}