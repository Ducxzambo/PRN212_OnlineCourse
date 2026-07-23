using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;
using Services.Models;

namespace Presentation.ViewModels;

public class AccountListViewModel : ViewModelBase
{
    private List<Account> _allAccounts = new();
    private string _searchText = "";
    private Account? _selectedAccount;

    /// <summary>Accounts with Role = Instructor - shown in the "Instructor" tab.</summary>
    public ObservableCollection<Account> InstructorAccounts { get; } = new();

    /// <summary>Accounts with Role = Student - shown in the "Student" tab.</summary>
    public ObservableCollection<Account> StudentAccounts { get; } = new();

    /// <summary>Accounts with Role = Admin - read-only, cannot be edited/deleted from the UI.</summary>
    public ObservableCollection<Account> AdminAccounts { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
    }

    public Account? SelectedAccount
    {
        get => _selectedAccount;
        set => SetProperty(ref _selectedAccount, value);
    }

    /// <summary>True when the selected account can be edited/deleted (i.e. it is not an Admin account).</summary>
    private bool CanModifySelected => SelectedAccount != null && SelectedAccount.Role != (int)AccountRole.Admin;

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ViewDetailsCommand { get; }

    public AccountListViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, () => CanModifySelected);
        DeleteCommand = new RelayCommand(Delete, () => CanModifySelected);
        RefreshCommand = new RelayCommand(Load);
        ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedAccount != null);
    }

    public  void Load()
    {
        var previousSelectedId = SelectedAccount?.Id;

        _allAccounts = AppServices.AccountService.GetAllAccounts();
        ApplyFilter();

        if (previousSelectedId.HasValue)
            SelectedAccount =
                InstructorAccounts.FirstOrDefault(a => a.Id == previousSelectedId.Value) ??
                StudentAccounts.FirstOrDefault(a => a.Id == previousSelectedId.Value) ??
                AdminAccounts.FirstOrDefault(a => a.Id == previousSelectedId.Value);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allAccounts
            : _allAccounts.Where(a =>
                    a.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

        InstructorAccounts.Clear();
        StudentAccounts.Clear();
        AdminAccounts.Clear();

        foreach (var account in filtered)
        {
            switch (account.Role)
            {
                case (int)AccountRole.Instructor:
                    InstructorAccounts.Add(account);
                    break;
                case (int)AccountRole.Student:
                    StudentAccounts.Add(account);
                    break;
                default:
                    AdminAccounts.Add(account);
                    break;
            }
        }
    }

    private void Add()
    {
        var window = new AccountEditWindow(new AccountEditViewModel(null));
        if (window.ShowDialog() == true) Load();
    }

    private void Edit()
    {
        if (SelectedAccount == null) return;
        if (SelectedAccount.Role == (int)AccountRole.Admin)
        {
            MessageBox.Show("Không được phép sửa tài khoản Admin.", "Không cho phép", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var window = new AccountEditWindow(new AccountEditViewModel(SelectedAccount));
        if (window.ShowDialog() == true) Load();
    }

    private  void Delete()
    {
        if (SelectedAccount == null) return;
        if (SelectedAccount.Role == (int)AccountRole.Admin)
        {
            MessageBox.Show("Không được phép xóa tài khoản Admin.", "Không cho phép", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Xóa tài khoản '{SelectedAccount.FullName}'?", "Xác nhận xóa",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        var (success, error) = AppServices.AccountService.DeleteAccount(SelectedAccount.Id);
        if (!success)
        {
            MessageBox.Show(error, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Load();
    }

    private void ViewDetails()
    {
        if (SelectedAccount == null) return;

        if (SelectedAccount.Role == (int)AccountRole.Instructor && SelectedAccount.Instructor != null)
        {
            var window = new InstructorDetailWindow(new InstructorDetailViewModel(SelectedAccount.Instructor));
            window.ShowDialog();
        }
        else if (SelectedAccount.Role == (int)AccountRole.Student && SelectedAccount.Student != null)
        {
            var window = new StudentDetailWindow(new StudentDetailViewModel(SelectedAccount.Student));
            window.ShowDialog();
        }
        else
        {
            MessageBox.Show("Tài khoản này chưa liên kết với thông tin chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

