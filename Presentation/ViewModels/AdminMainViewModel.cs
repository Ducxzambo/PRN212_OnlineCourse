using System.Windows.Input;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class AdminMainViewModel : ViewModelBase
{
    private object? _currentViewModel;

    private readonly CategoryListViewModel _categoryListViewModel;
    private readonly AccountListViewModel _accountListViewModel;
    private readonly DashboardViewModel _dashboardViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string AdminName => AdminSession.Current?.FullName ?? "";

    public ICommand NavigateDashboardCommand { get; }
    public ICommand NavigateCategoriesCommand { get; }
    public ICommand NavigateAccountsCommand { get; }
    public ICommand LogoutCommand { get; }

    /// <summary>Raised when the admin logs out - the shell view closes and shows the login window again.</summary>
    public event EventHandler? LogoutRequested;

    public AdminMainViewModel()
    {
        _dashboardViewModel = new DashboardViewModel();
        _categoryListViewModel = new CategoryListViewModel();
        _accountListViewModel = new AccountListViewModel();

        NavigateDashboardCommand = new RelayCommand(() => CurrentViewModel = _dashboardViewModel);
        NavigateCategoriesCommand = new RelayCommand(() => CurrentViewModel = _categoryListViewModel);
        NavigateAccountsCommand = new RelayCommand(() => CurrentViewModel = _accountListViewModel);
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = _dashboardViewModel;

        _ = _dashboardViewModel.LoadAsync();
        _ = _categoryListViewModel.LoadAsync();
        _ = _accountListViewModel.LoadAsync();
    }

    private void Logout()
    {
        AdminSession.Current = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}
