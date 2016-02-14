using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MC.Database;
using MC.Mangafox;
using MC.Mangareader;
using MC.Models;
using PropertyChanged;
using MC.Mangastream;
using MC.Webtoons;

namespace MC.ViewModels {
    [ImplementPropertyChanged]
    public class NormalAddViewModel {
        public NormalAddViewModel() {
            SearchCommand = new Models.ActionCommand(Search);
            AddCommand = new Models.ActionCommand(Add);
            Progressbar = Visibility.Collapsed;
            AddButtonVisibility = Visibility.Collapsed;
        }

        public string Link { get; set; }

        public Visibility Progressbar { get; set; }

        public Visibility InfoVisi { get; set; }

        public Visibility AddButtonVisibility { get; set; }

        private MangaModel manga { get; set; }

        public string InfoLabel { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }

        public void Search() {
            //search stuff
            InfoLabel = "";
            Progressbar = Visibility.Visible;
            var t = new Thread(new ThreadStart(delegate {
                try {
                    //search manga here
                    if (Link.ToLower().Contains("mangareader.net")) {
                        manga = MangareaderGi.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Contains("mangafox.me")) {
                        manga = MangafoxGi.GeInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Contains("readms.com") || Link.ToLower().Contains("mangastream.com")) {
                        manga = MangastreamGi.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else if (Link.ToLower().Equals(string.Empty)) {
                        manga.Error = "Link empty";
                    }
                    else if (Link.ToLower().Contains("webtoons")) {
                        manga = WebtoonsGi.GetInfo(Link);
                        InfoLabel = $"{manga.Name}\n{manga.Chapter}\n{manga.Site}";
                    }
                    else {
                        InfoLabel = "Link not recognized :/";
                    }
                }
                catch (Exception error) {
                    InfoLabel = error.Message;
                    AddButtonVisibility = Visibility.Collapsed;
                }
                Progressbar = Visibility.Collapsed;
                AddButtonVisibility = Visibility.Visible;
            })) {IsBackground = true};
            t.Start();
        }

        public void Add() {
            var name = manga.Name;
            var chapter = manga.Chapter;
            if (manga.Site.ToLower().Contains("mangareader")) {
                if (name != "ERROR" || name != "None" && chapter != "None" || chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangareader", name.ToLower(), chapter, "");
                    Sqlite.AddManga("mangareader", name, chapter, "placeholder", DateTime.Now, manga.Link);
                    InfoLabel += Sqlite.AddManga("mangareader", name, chapter, "placeholder", DateTime.Now, manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangafox")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangafox", name.ToLower(), chapter, "");
                    InfoLabel += Sqlite.AddManga("mangafox", name, chapter, "placeholder", DateTime.Now, manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            if (manga.Site.ToLower().Contains("mangastream")) {
                if (!name.Equals("ERROR") && name != "None" && chapter != "None" && chapter != "ERROR") {
                    DebugText.Write($"[Debug] Trying to add {name} {chapter}");
                    ParseFile.AddManga("mangastream", name.ToLower(), chapter, "");
                    InfoLabel += Sqlite.AddManga("mangastream", name, chapter, "placeholder", manga.Date,
                        manga.Link)
                        ? "\nSuccess!"
                        : "\nAlready in list!";
                    return;
                }
            }
            InfoLabel = "failed";
            AddButtonVisibility = Visibility.Collapsed;
        }
    }
}