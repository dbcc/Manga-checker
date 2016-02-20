using System.Windows.Controls;
using Manga_checker.ViewModels;

namespace Manga_checker.Views {
    /// <summary>
    ///     Interaktionslogik für SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl {
        public SettingView() {
            InitializeComponent();
            DataContext = new SettingViewModel();
        }
    }
}