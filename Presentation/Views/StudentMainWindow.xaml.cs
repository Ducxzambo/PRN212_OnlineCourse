using System.Windows;
using Presentation.Helpers;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class StudentMainWindow : Window
{
    private StudentMainViewModel _viewModel;

    public StudentMainWindow()
    {
        InitializeComponent();
        _viewModel = new StudentMainViewModel();
        DataContext = _viewModel;

        _viewModel.LogoutRequested += (s, e) =>
        {
            StudentSession.Current = null;
            StudentSession.CurrentAccount = null;
            Close();
        };
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        await _viewModel.LoadAsync();
    }

 
}
