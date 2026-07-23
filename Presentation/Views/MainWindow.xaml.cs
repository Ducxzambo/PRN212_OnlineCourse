using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            viewModel.LogoutRequested += (s, e) =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            };
            viewModel.ProfileRequested += (_, _) =>
            {
                if (Presentation.Helpers.InstructorSession.Current?.Account != null)
                    new ProfileWindow(new ProfileViewModel(Presentation.Helpers.InstructorSession.Current.Account)).ShowDialog();
            };

            DataContext = viewModel;
        }
    }
}

