using System.Windows;
using System.Windows.Controls;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class ProfileWindow : Window
{
    public ProfileWindow(ProfileViewModel viewModel)
    {
        InitializeComponent();
        viewModel.SaveSucceeded += (_, _) => { MessageBox.Show("Đã cập nhật hồ sơ.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information); DialogResult = true; };
        DataContext = viewModel;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProfileViewModel vm) vm.NewPassword = ((PasswordBox)sender).Password;
    }
}
