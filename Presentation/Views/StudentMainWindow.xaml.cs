using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class StudentMainWindow : Window
{
    public StudentMainWindow()
    {
        InitializeComponent();
        var vm = new StudentMainViewModel();
        vm.LogoutRequested += (_, _) => { new LoginWindow().Show(); Close(); };
        vm.ProfileRequested += (_, _) =>
        {
            if (Presentation.Helpers.StudentSession.Current?.Account != null)
                new ProfileWindow(new ProfileViewModel(Presentation.Helpers.StudentSession.Current.Account)).ShowDialog();
        };
        DataContext = vm;
        vm.Load();
    }
}
