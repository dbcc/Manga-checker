using System.Windows.Controls;

namespace MC.ViewModels.Views {
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