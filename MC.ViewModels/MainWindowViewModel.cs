using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MC.Database;
using MC.Models;
using MC.Threads;
using MC.Tools;
using MC.ViewModels.Dialogs;
using MC.ViewModels.Properties;
using PropertyChanged;
using MC.ViewModels.Views;

namespace MC.ViewModels {
    [ImplementPropertyChanged]
    public class MainWindowViewModel  {
        public static readonly ObservableCollection<MangaModel> MangasInternal =
            new ObservableCollection<MangaModel>();
        

        private ThreadStart Childref;
        private Thread ChildThread;
        private HistoryWindow History;

        public PackIconKind PausePlayButtonIcon { get; set; } = PackIconKind.Pause;

        private readonly List<string> _sites = new List<string> {
            "Mangafox",
            "Mangahere",
            "Mangareader",
            "Mangastream",
            "Batoto",
            "Webtoons",
            "YoManga",
            "Kissmanga"
        };
        
        public MainWindowViewModel() {
            Mangas = new ReadOnlyObservableCollection<MangaModel>(MangasInternal);

            RefreshCommand = new ActionCommand(RunRefresh);
            FillMangastreamCommand = new ActionCommand(FillMangastream);
            FillYoMangaCommand = new ActionCommand(Fillyomanga);
            FillMangafoxCommand = new ActionCommand(FillMangafox);
            FillMangahereCommand = new ActionCommand(FillMangahere);
            FillMangareaderCommand = new ActionCommand(FillMangareader);
            FillWebtoonsCommand = new ActionCommand(FillWebtoons);
            FillBatotoCommand = new ActionCommand(Fillbatoto);
            FillListCommand = new ActionCommand(Fill_list);
            FillBacklogCommand = new ActionCommand(FillBacklog);
            FillKissmangaCommand = new ActionCommand(FillKissmanga);
            StartStopCommand = new ActionCommand(Startstop);
            DebugCommand = new ActionCommand(DebugClick);
            SettingsCommand = new ActionCommand(SettingClick);
            AddMangaCommand = new ActionCommand(AddMangaClick);
            HistoryCommand = new ActionCommand(ShowHistory);
            DeleteMangaCommand = new ActionCommand(Delete);

            MinusChapterCommand = new ActionCommand(ChapterMinus);
            PlusChapterCommand = new ActionCommand(ChapterPlus);
            RefreshMangaCommand = new ActionCommand(Refresh);

            DebugVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;

            ThreadStatus = "[Running]";
            Fill_list();

            Childref = MainThread.CheckNow;
            ChildThread = new Thread(Childref) {IsBackground = true};
            ChildThread.SetApartmentState(ApartmentState.STA);
            ChildThread.Start();
        }


        public ICommand MinusChapterCommand { get; }
        public ICommand PlusChapterCommand { get; }
        public ICommand RefreshMangaCommand { get; }

        public MangaModel SelectedItem { get; set; }

        public bool MenuToggleButton { get; set; }

        public ReadOnlyObservableCollection<MangaModel> Mangas { get; }

        public ICommand RefreshCommand { get; }
        public ICommand FillMangastreamCommand { get; }
        public ICommand FillMangareaderCommand { get; }
        public ICommand FillYoMangaCommand { get; }
        public ICommand FillMangafoxCommand { get; }
        public ICommand FillMangahereCommand { get; }
        public ICommand FillWebtoonsCommand { get; }
        public ICommand FillBacklogCommand { get; }
        public ICommand FillListCommand { get; }
        public ICommand FillBatotoCommand { get; }
        public ICommand FillKissmangaCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand DebugCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AddMangaCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand DeleteMangaCommand { get; }

        public string CurrentSite { get; set; }

        public string ThreadStatus { get; set; }

        public Visibility DataGridVisibility { get; set; }

        public Visibility DebugVisibility { get; set; }

        public Visibility SettingsVisibility { get; set; }

        public Visibility AddVisibility { get; set; }

        private void ShowHistory() {
            if (History != null) {
                History.Show();
            }
            else {
                History = new HistoryWindow {
                    DataContext = new HistoryViewModel(),
                    ShowActivated = false,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                History.Show();
            }
        }

        private void RunRefresh() {
            Threads.Properties.Settings.Default.ForceCheck = "force";
        }

        private void Startstop() {
            switch (ThreadStatus) {
                case "[Running]": {
                    ChildThread.Abort();
                    ThreadStatus = "[Stopped]";
                    PausePlayButtonIcon = PackIconKind.Play;
                    break;
                }
                case "[Stopped]": {
                    Childref = MainThread.CheckNow;
                    ChildThread = new Thread(Childref) {IsBackground = true};
                    ChildThread.Start();
                    ThreadStatus = "[Running]";
                    PausePlayButtonIcon = PackIconKind.Pause;
                    break;
                }
            }
        }

        public bool FillingList { get; set; } = false;

        private async Task GetMangas(string site) {
            if (FillingList) return ;
            FillingList = true;
            CurrentSite = site;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
            foreach (var manga in await Sqlite.GetMangasAsync(site.ToLower())) {
                if (manga.Link.Equals("placeholder")) {
                    manga.Link = "";
                }
                MangasInternal.Add(manga);
            }
            FillingList = false;
        }

        private async void FillMangastream() {
            MangasInternal.Clear();
            await GetMangas("Mangastream");
        }

        private async void FillMangareader() {
            MangasInternal.Clear();
            await GetMangas("Mangareader");
        }

        private async void Fillbatoto() {
            MangasInternal.Clear();
            await GetMangas("Batoto");
        }

        private async void FillMangafox() {
            MangasInternal.Clear();
            await GetMangas("Mangafox");
        }

        private async void FillMangahere() {
            MangasInternal.Clear();
            await GetMangas("Mangahere");
        }

        private async void FillBacklog() {
            MangasInternal.Clear();
            await GetMangas("Backlog");
        }

        private async void FillWebtoons() {
            MangasInternal.Clear();
            await GetMangas("Webtoons");
        }

        private async void Fillyomanga() {
            MangasInternal.Clear();
            await GetMangas("YoManga");
        }

        private async void FillKissmanga() {
            MangasInternal.Clear();
            await GetMangas("Kissmanga");
        }

        private async void Fill_list() {
            MenuToggleButton = false;
            MangasInternal.Clear();
            foreach (var site in _sites) {
                await GetMangas(site);
            }
            CurrentSite = "All";
        }

        private void DebugClick() {
            CurrentSite = "Debug";
            DebugVisibility = Visibility.Visible;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Collapsed;
        }

        private void SettingClick() {
            MenuToggleButton = false;
            CurrentSite = "Settings";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Visible;
            AddVisibility = Visibility.Collapsed;
        }

        private void AddMangaClick() {
            MenuToggleButton = false;
            CurrentSite = "Add Manga";
            DebugVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            SettingsVisibility = Visibility.Collapsed;
            AddVisibility = Visibility.Visible;
        }

        private static async Task<bool> Delete(MangaModel mangaItem) {
            var dialog = new ConfirmDeleteDialog {
                MessageTextBlock = {
                    Text = "Deleting\n" + mangaItem.Name
                },
                SiteName = {
                    Text = mangaItem.Site
                },
                item = mangaItem
            };
            var x = await DialogHost.Show(dialog);
            return (string)x == "1";
        }

        private async void Delete() {
            var su = await Delete(SelectedItem);
            if (su)
                MangasInternal.Remove(SelectedItem);
        }

        public void ChapterMinus() {
            Tools.Tools.ChangeChaperNum(SelectedItem, "-");
        }

        public void ChapterPlus() {
            Tools.Tools.ChangeChaperNum(SelectedItem, "+");
        }

        public void Refresh() {
            try {
                Tools.Tools.RefreshManga(SelectedItem);
            } catch {
                //ignored
            }
        }
    }
}