using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class AccountEditViewModel : ViewModelBase
{
    private readonly int? _accountId;
    private string _fullName = "";
    private string _email = "";
    private string? _phone;
    private bool _isActive = true;
    private AccountRole _role = AccountRole.Instructor;
    private string? _errorMessage;

    public bool IsEdit => _accountId.HasValue;
    public string WindowTitle => IsEdit ? "Sửa tài khoản" : "Thêm tài khoản mới";

    /// <summary>Role can only be chosen when creating; it is fixed afterwards.</summary>
    public bool CanChangeRole => !IsEdit;

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string? Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsInstructorRole
    {
        get => _role == AccountRole.Instructor;
        set { if (value) Role = AccountRole.Instructor; }
    }

    public bool IsStudentRole
    {
        get => _role == AccountRole.Student;
        set { if (value) Role = AccountRole.Student; }
    }

    public bool IsAdminRole
    {
        get => _role == AccountRole.Admin;
        set { if (value) Role = AccountRole.Admin; }
    }

    private AccountRole Role
    {
        get => _role;
        set
        {
            if (_role == value) return;
            _role = value;
            OnPropertyChanged(nameof(IsInstructorRole));
            OnPropertyChanged(nameof(IsStudentRole));
            OnPropertyChanged(nameof(IsAdminRole));
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }

    public event EventHandler? SaveSucceeded;

    public AccountEditViewModel(Account? account)
    {
        if (account != null)
        {
            _accountId = account.Id;
            FullName = account.FullName;
            Email = account.Email;
            Phone = account.Instructor?.Phone ?? account.Student?.Phone;
            IsActive = account.IsActive;
            _role = (AccountRole)account.Role;
        }

        SaveCommand = new RelayCommand(Save);
    }

    private  void Save()
    {
        ErrorMessage = null;

        var (success, error) = IsEdit
            ? AppServices.AccountService.UpdateAccount(_accountId!.Value, FullName, Email, Phone, IsActive)
            : AppServices.AccountService.CreateAccount(FullName, Email, Phone, Role);

        if (!success)
        {
            ErrorMessage = error;
            return;
        }

        SaveSucceeded?.Invoke(this, EventArgs.Empty);
    }
}

