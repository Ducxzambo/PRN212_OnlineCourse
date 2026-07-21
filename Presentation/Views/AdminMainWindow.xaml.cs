using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();

            var viewModel = new AdminMainViewModel();
            viewModel.LogoutRequested += (s, e) =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            };

            DataContext = viewModel;
        }
    }
}
