using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MC.Database;

namespace MC.ViewModels.Dialogs {
    /// <summary>
    ///     Interaktionslogik für SetupDatabaseDialog.xaml
    /// </summary>
    public partial class SetupDatabaseDialog : UserControl {
        public SetupDatabaseDialog() {
            InitializeComponent();
            //Application.Current.Dispatcher.BeginInvoke(new Action(delegate {

            //}));
            start();
        }

        private void start() {
            new Thread(new ThreadStart(delegate {
                Thread.Sleep(3000);
                DebugText.Write("Creating Database");
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate { status.Content = "Creating Database"; }));
                Sqlite.SetupDatabase();
                if (File.Exists("manga.json")) {
                    DebugText.Write("Populating Database");
                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate { status.Content = "Populating Database"; }));
                    Sqlite.PopulateDb();
                }
                    status.Content = "FINISHED";
                    ProgressBar.Visibility = Visibility.Collapsed;
                    closeBtn.Visibility = Visibility.Visible;
            })).Start();
        }
    }
}