﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Manga_checker.Database;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using Manga_checker.Sites;

namespace Manga_checker.Threads {
    internal class MainThread {
        public static void CheckNow() {
            var i = 5;
            var count = 0;
            while (true) {
                if (Settings.Default.ForceCheck.Equals("force")) {
                    i = 3;
                    Settings.Default.ForceCheck = "";
                }
                if (i >= 1) {
                    Settings.Default.StatusLabel = "Status: Checking in " + i + " seconds.";
                    Thread.Sleep(1000);
                    i--;
                    Settings.Default.CounterLabel = count.ToString();
                }
                else {
                    if (ParseFile.GetValueSettings("mangastream") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangastream";
                        var mslist = MangastreamRSS.Get_feed_titles();
                        foreach (var manga in Sqlite.GetMangas("mangastream")) {
                            try {
                                MangastreamRSS.Check(manga, mslist);
                            }
                            catch (Exception mst) {
                                DebugText.Write($"[Mangastream] Error {mst.Message} {mst.Data}");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("mangafox") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangafox";
                        foreach (var manga in Sqlite.GetMangas("mangafox")) {
                            //DebugText.Write(string.Format("[{0}][Mangafox] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                            try {
                                MangafoxRSS.Check(manga);
                            }
                            catch (Exception mst) {
                                DebugText.Write($"[mangafox] Error {mst.Message} {mst.Data}");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("mangareader") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Mangareader";
                        foreach (var manga in Sqlite.GetMangas("mangareader")) {
                            try {
                                //DebugText.Write(string.Format("[{0}][Mangareader] Checking {1}.", DateTime.Now,manga.Replace("[]", " ")));
                                MangareaderHTML.Check(manga);
                            }
                            catch (Exception mrd) {
                                DebugText.Write($"[Mangareader] Error {manga.Name} {mrd.Message}.");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("batoto") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Batoto";
                        var _mlist = BatotoRSS.Get_feed_titles();
                        foreach (var manga in Sqlite.GetMangas("batoto")) {
                            try {
                                BatotoRSS.Check(_mlist, manga);
                            }
                            catch (Exception bat) {
                                DebugText.Write($"[batoto] Error {bat.Message}.");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("kissmanga") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Kissmanga";
                        foreach (var manga in ParseFile.GetManga("kissmanga")) {
                            try {
                                //DebugText.Write(string.Format("[{0}][Kissmanga] Checking {1}.", DateTime.Now, manga.Replace("[]", " ")));
                                var man = manga.Split(new[] {"[]"}, StringSplitOptions.None);
                                KissmangaHTML.Check(man[0], man[1]);
                                Thread.Sleep(5000);
                            }
                            catch (Exception mrd) {
                                // lol
                                DebugText.Write($"[Kissmanga] Error {manga.Replace("[]", " ")} {mrd.Message}.");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("webtoons") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking Webtoons";
                        foreach (var manga in Sqlite.GetMangas("webtoons")) {
                            try {
                                WebtoonsRSS.Check(manga);
                            }
                            catch (Exception to) {
                                DebugText.Write($"[Webtoons] Error {to.Message}.");
                            }
                        }
                    }
                    if (ParseFile.GetValueSettings("yomanga") == "1") {
                        Settings.Default.StatusLabel = "Status: Checking YoManga";
                        var rss = RSSReader.Read("http://yomanga.co/reader/feeds/rss") ??
                                  RSSReader.Read("http://46.4.102.16/reader/feeds/rss");
                        if (rss != null) {
                            foreach (var manga in Sqlite.GetMangas("yomanga")) {
                                try {
                                    YomangaRSS.Check(manga, rss);
                                }
                                catch (Exception to) {
                                    DebugText.Write($"[YoManga] Error {to.Message}.");
                                }
                            }    
                        }
                    }
                    //timer2.Start();
                    var waittime = int.Parse(ParseFile.GetValueSettings("refresh time"));

                    Settings.Default.StatusLabel = @"Status: Checking in " + waittime + " seconds.";
                    count++;
                    i = waittime;
                }
            }
        }
    }
}