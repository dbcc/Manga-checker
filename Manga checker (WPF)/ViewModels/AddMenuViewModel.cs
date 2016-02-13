﻿using System;
using System.Windows.Input;
using Manga_checker.Adding;
using Manga_checker.Database;
using Manga_checker.Handlers;
using MaterialDesignThemes.Wpf;

namespace Manga_checker.ViewModels {
    public class AddMenuViewModel : ViewModelBase {
        private string _chapter;
        private string _name;

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Chapter {
            get { return _chapter; }
            set {
                _chapter = value;
                OnPropertyChanged();
            }
        }


        public AddMenuViewModel() {
            AddBacklogCommand = new ActionCommand(AddToBacklog);
            AddNormalCommand = new ActionCommand(NormalClick);
        }

        public ICommand AddBacklogCommand { get; }
        public ICommand AddNormalCommand { get; }
        //public ICommand AddAdvancedCommand { get; }

        private static async void NormalClick() {
            var d = new NormalAddDialog { DataContext = new NormalAddViewModel() };
            await DialogHost.Show(d);
        }

        private void AddToBacklog() {
            ParseFile.AddMangatoBacklog("backlog", Name, Chapter);
            if (Sqlite.GetMangaNameList("backlog").Contains(Name)) {
                Sqlite.UpdateManga(
                    "backlog",
                    Name,
                    Chapter,
                    "placeholder",
                    DateTime.Now);
            } else {
                Sqlite.AddManga("backlog", Name, Chapter, "placeholder", DateTime.Now);
            }

            Name = string.Empty;
            Chapter = string.Empty;
        }

    }
}
