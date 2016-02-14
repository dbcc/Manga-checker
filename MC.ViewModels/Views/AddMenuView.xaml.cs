using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MC.ViewModels.Views {
    /// <summary>
    /// Interaktionslogik für AddMenuView.xaml
    /// </summary>
    public partial class AddMenuView : UserControl {
        public AddMenuView() {
            InitializeComponent();
            DataContext = new AddMenuViewModel();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
