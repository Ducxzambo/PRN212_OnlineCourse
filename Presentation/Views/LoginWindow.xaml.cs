using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var viewModel = new LoginViewModel();
            viewModel.LoginSucceeded += (s, e) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            };

            DataContext = viewModel;
        }
    }
}
