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

            viewModel.InstructorLoginSucceeded += (s, e) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            };

            viewModel.AdminLoginSucceeded += (s, e) =>
            {
                var adminWindow = new AdminMainWindow();
                adminWindow.Show();
                Close();
            };

            DataContext = viewModel;
        }
    }
}
