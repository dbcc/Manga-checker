using System.Collections.ObjectModel;
using System.Windows.Input;
using MC.Database;
using MC.Models;
using PropertyChanged;

namespace MC.ViewModels {
    [ImplementPropertyChanged]
    public class HistoryViewModel {
        private readonly ObservableCollection<MangaModel> _linksCollection =
            new ObservableCollection<MangaModel>();

        public HistoryViewModel() {
            LinkCollection = new ReadOnlyObservableCollection<MangaModel>(_linksCollection);
            RefreshCommand = new Models.ActionCommand(FillCollection);
            FillCollection();
        }

        public MangaModel SelectedItem { get; set; }

        public ICommand RefreshCommand { get; }
        public ReadOnlyObservableCollection<MangaModel> LinkCollection { get; }

        public void FillCollection() {
            _linksCollection.Clear();
            foreach (var m in Sqlite.GetHistory()) {
                _linksCollection.Add(m);
            }
        }
    }
}