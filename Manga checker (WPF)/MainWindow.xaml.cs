using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Manga_checker.Handlers;
using Manga_checker.Properties;
using MaterialDesignThemes.Wpf;
using MC.Models;
using MC.ViewModels.Dialogs;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // private WebClient web = new WebClient();
        public ThreadStart Childref;

        public Thread ChildThread;

        public Thread client;

        public bool clientStatus = false;

        // private DataGridMangasItem itm = new DataGridMangasItem();
        public List<string> mlist;

        public MainWindow() {
            InitializeComponent();
            Settings.Default.Debug = "Debug shit goes in here!\n";
            StartupInit.Setup();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DataGridMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            try {
                var itemselected = (MangaModel) DataGridMangas.SelectedItem;
                switch (itemselected.Site) {
                    case "mangafox": {
                        if (itemselected.Link != "placeholder") {
                            Process.Start(itemselected.Link);
                            itemselected.New = 0;
                        }
                        break;
                    }

                    case "mangareader": {
                        if (itemselected.Link != "placeholder") {
                            Process.Start(itemselected.Link);
                            itemselected.New = 0;
                        }
                        break;
                    }

                    case "batoto": {
                        if (itemselected.Link != "placeholder") {
                            Process.Start(itemselected.Link);
                            itemselected.New = 0;
                        }
                        break;
                    }
                    case "mangastream": {
                        if (itemselected.Link != "placeholder") {
                            Process.Start(itemselected.Link);
                            itemselected.New = 0;
                        }
                        break;
                    }
                }
            }
            catch (Exception g) {
                // do nothing
                DebugText.Write($"[Error] {g.Message} {g.TargetSite} ");
            }
        }

        private void DebugTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            DebugTextBox.ScrollToEnd();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            DebugText.Write(Settings.Default.ThreadStatus.ToString());

            // ButtonColorChange();
            if (!File.Exists("MangaDB.sqlite")) {
                DialogHost.Show(new SetupDatabaseDialog());
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
        
        private void TopMostBtn_Click(object sender, RoutedEventArgs e) {
            Topmost = Topmost == false;
        }
    }
}