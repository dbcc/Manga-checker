using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PropertyChanged;

namespace MC.Models {
    [ImplementPropertyChanged]
    public class MangaModel {

        public MangaModel() {
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Chapter { get; set; }

        public string Site { get; set; }
        public string Error { get; set; }
        public string Link { get; set; }
        public string RssLink { get; set; }

        public DateTime Date { get; set; }

        public string FullName => $"{Name} {Chapter}";

        //public ObservableCollection<Button> _buttons = new ObservableCollection<Button>();

        public int New { get; set; }

        public List<Button> Buttons => PopulateButtons();
        public Visibility Separator { get; set; } = Visibility.Visible;

        
        public List<Button> PopulateButtons() {
            var gg = new List<string> {"mangafox", "mangareader", "mangahere"};
            var list = new List<Button>();
            if (gg.Contains(Site.ToLower())) {
                for (var i = 0; i < 3; i++) {
                    if (Chapter.Contains(" ")) {
                        Chapter = Chapter.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    var ch = int.Parse(Chapter);
                    ch = ch - i;
                    var button = new Button {
                        Content = $"{Name} {ch}",
                        Style = (Style) Application.Current.FindResource("MaterialDesignFlatButton"),
                        Height = 30
                    };
                    //button.Click +=
                    //    (sender, e) => OpenSite.Open(Site, Name, ch.ToString(), new List<string>());
                    list.Add(button);
                }
            } else {
                Separator = Visibility.Collapsed;
            }
            return list;
        }
    }
}