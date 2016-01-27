﻿using System;
using System.Diagnostics;
using Manga_checker.Database;
using Manga_checker.Handlers;

namespace Manga_checker.Sites {
    internal class YomangaRSS {
        public RSSReader RssReader = new RSSReader();

        public void Check() {
            try {
                var mangaList = ParseFile.GetManga("yomanga");
                var open = ParseFile.GetValueSettings("open links");
                var feed = RssReader.Read("http://yomanga.co/reader/feeds/rss");
                foreach (var manga in mangaList) {
                    var splitterino = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                    var newch = int.Parse(splitterino[1]) + 1;
                    var full = splitterino[0] + " chapter " + newch;
                    foreach (var item in feed.Items) {
                        var title = item.Title.Text;
                        if (Equals(full.ToLower(), title.ToLower())) {
                            if (open.Equals("1")) {
                                Process.Start(item.Links[0].Uri.AbsoluteUri);
                                ParseFile.SetManga("yomanga", splitterino[0], newch.ToString());
                                Sqlite.UpdateManga("yomanga", splitterino[0], newch.ToString(),
                                    item.Links[0].Uri.AbsoluteUri);
                                DebugText.Write($"[{DateTime.Now}][YoManga] Found new Chapter {splitterino[0]} {newch}.");
                                break;
                            }
                        }
                    }
                    //DebugText.Write(item.Title.Text);
                }
            }
            catch (Exception ex) {
                DebugText.Write($"[{DateTime.Now}][YoManga] Error {ex.Message}.");
            }
        }
    }
}