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

            viewModel.StudentLoginSucceeded += (s, e) =>
            {
                var studentWindow = new StudentMainWindow();
                studentWindow.Show();
                Close();
            };

            DataContext = viewModel;

            // Bind PasswordBox to ViewModel (PasswordBox doesn't support direct binding for security reasons)
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (viewModel != null)
                {
                    viewModel.Password = PasswordBox.Password;
                }
            };
        }
    }
}
