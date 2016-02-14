using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MC.Database;
using MC.Models;

namespace MC.Mangareader {
    public class MangareaderHTML {
        public static string Check(MangaModel manga, string openLinks) {
            var name = Regex.Replace(manga.Name, "[^0-9a-zA-Z]+", "-").Trim('-').ToLower();
            //DebugText.Write(name);
            var ch = manga.Chapter.Trim(' ');
            MatchCollection m1;
            var FullName = "";
            var mangaa = "";
            var ch_plus = 0;
            if (ch.Contains(" ")) {
                var chsp = manga.Chapter.Split(new[] {" "}, StringSplitOptions.None);
                ch_plus = int.Parse(chsp[0]);
                ch_plus++;
                FullName = manga.Name + " " + ch_plus;

                var url_2 = "http://www.mangareader.net/" + chsp[1] + "/" + name;
                var htmltxt2 = GetSource.Get(url_2 + ".html") ?? GetSource.Get(url_2);
                m1 = Regex.Matches(htmltxt2, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match mangamatch in m1) {
                    mangaa = mangamatch.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName)) {
                        var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
                        if (openLinks == "1") {
                            Process.Start(link);
                            ParseFile.SetManga("mangareader", manga.Name, ch_plus + " " + chsp[1]);
                            Sqlite.UpdateManga("mangareader", manga.Name, ch_plus + " " + chsp[1], link, DateTime.Now);
                            manga.Chapter = ch_plus + " " + chsp[1];
                            manga.Link = link;
                        }
                        DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
                        return FullName;
                    }
                    FullName = manga.Name + " " + chsp[0];
                }
            }
            else {
                var chsp = ch;
                ch_plus = int.Parse(chsp);
                ch_plus++;
                FullName = manga.Name + " " + ch_plus;
                var url_1 = "http://www.mangareader.net/" + name;
                var htmltext1 = GetSource.Get(url_1) ?? GetSource.Get(url_1 + ".html");
                m1 = Regex.Matches(htmltext1, @"<a href=.+>(.+)</a>.+</li>", RegexOptions.IgnoreCase);
                foreach (Match mangamatch in m1) {
                    mangaa = mangamatch.Groups[1].Value;
                    if (mangaa.ToLower().Contains(FullName)) {
                        var link = "http://www.mangareader.net/" + name + "/" + ch_plus;
                        if (openLinks == "1") {
                            Process.Start(link);
                            ParseFile.SetManga("mangareader", manga.Name, ch_plus.ToString());
                            Sqlite.UpdateManga("mangareader", manga.Name, ch_plus.ToString(), link, DateTime.Now);
                            manga.Chapter = ch_plus.ToString();
                            manga.Link = link;
                        }
                        DebugText.Write($"[Mangareader] {manga.Name} {ch_plus} Found new Chapter");
                        return FullName;
                    }
                    FullName = manga.Name + " " + chsp;
                }
            }
            return FullName;
        }
    }
}