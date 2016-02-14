using System.Diagnostics;
using System.IO;
using System.Windows;
using Manga_checker.Handlers;
using MC.Database;
using MC.ViewModels;

namespace Manga_checker {
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args) {
            if (!Debugger.IsAttached)
                ExceptionHandler.AddGlobalHandlers();

            if (File.Exists("MangaDB.sqlite"))
                Sqlite.UpdateDatabase();
            var mainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };
            mainWindow.Show();
        }
    }
}