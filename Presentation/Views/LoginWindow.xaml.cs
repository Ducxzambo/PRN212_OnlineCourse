using System.Windows;
using System.Windows.Controls;
using Presentation.ViewModels;

namespace Presentation.Views;
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        var vm = new LoginViewModel();
        vm.InstructorLoginSucceeded += (_, _) => { new MainWindow().Show(); Close(); };
        vm.AdminLoginSucceeded += (_, _) => { new AdminMainWindow().Show(); Close(); };
        vm.StudentLoginSucceeded += (_, _) => { new StudentMainWindow().Show(); Close(); };
        DataContext = vm;
    }
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) { if (DataContext is LoginViewModel vm) vm.Password = ((PasswordBox)sender).Password; }
    private void RegisterStudent_Click(object sender, RoutedEventArgs e) => new StudentRegisterWindow().ShowDialog();
}

