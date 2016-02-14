using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MC.Database;
using MC.Models;
using MC.ViewModels.Dialogs;
using PropertyChanged;

namespace MC.ViewModels {
    [ImplementPropertyChanged]
    public class AddMenuViewModel {
        public string Name { get; set; }

        public string Chapter { get; set; }


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
