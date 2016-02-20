using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Manga_checker.ViewModels;
using MC.Models;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window {
        public HistoryWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Hide();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (DataGrid.SelectedIndex == -1) return;
            var item = (MangaModel) DataGrid.SelectedItem;
            Process.Start(item.Link);
        }
    }
}