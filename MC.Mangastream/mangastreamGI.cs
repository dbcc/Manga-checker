using System;
using System.Net;
using System.Text.RegularExpressions;
using MC.Models;

namespace MC.Mangastream {
    public class MangastreamGi {
        public static MangaModel GetInfo(string url) {
            var InfoViewModel = new MangaModel();

            try {
                var web = new WebClient();
                var source = web.DownloadString(url);
                //  </i> Smokin' Parade <strong>001</strong><em>
                var name = Regex.Match(source, "<h1>(.+)</h1>");
                if (name.Success) {
                    InfoViewModel.Name = name.Groups[1].Value.Trim();
                    var chapter = Regex.Match(source, "(http://readms.com/r/.+)\">(.{1,}?) - .[^<>]+");
                    if (chapter.Success) {
                        InfoViewModel.Chapter = chapter.Groups[2].Value.Trim();
                        InfoViewModel.Error = "null";
                        InfoViewModel.Site = "mangastream.com";
                        InfoViewModel.Link = chapter.Groups[1].Value.Trim();
                        return InfoViewModel;
                    }
                }
                InfoViewModel.Error = "No Match found";
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
            catch (Exception e) {
                InfoViewModel.Error = e.Message;
                InfoViewModel.Name = "ERROR";
                InfoViewModel.Chapter = "ERROR";
                return InfoViewModel;
            }
        }
    }
}