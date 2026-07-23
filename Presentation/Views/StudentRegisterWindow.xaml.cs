using System.Windows;
using System.Windows.Controls;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class StudentRegisterWindow : Window
{
    public StudentRegisterWindow()
    {
        InitializeComponent();
        var vm = new StudentRegisterViewModel();
        vm.Registered += (_, _) =>
        {
            MessageBox.Show("Đăng ký thành công. Vui lòng đăng nhập lại.", "Đăng ký thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        };
        DataContext = vm;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is StudentRegisterViewModel vm) vm.Password = ((PasswordBox)sender).Password;
    }
}
