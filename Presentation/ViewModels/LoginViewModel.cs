using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _username = "";
    private string _password = "";
    private string? _errorMessage;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoginCommand { get; }

    /// <summary>Raised when an Instructor account logs in successfully.</summary>
    public event EventHandler? InstructorLoginSucceeded;

    /// <summary>Raised when an Admin account logs in successfully.</summary>
    public event EventHandler? AdminLoginSucceeded;

    /// <summary>Raised when a Student account logs in successfully.</summary>
    public event EventHandler? StudentLoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Vui lòng nhập tên tài khoản hoặc email.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập mật khẩu.";
            return;
        }

        var account = await AppServices.AccountService.LoginAsync(Username, Password);
        if (account == null)
        {
            ErrorMessage = "Tên tài khoản/email hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa.";
            return;
        }

        if (account.Role == (int)AccountRole.Admin)
        {
            AdminSession.Current = account;
            AdminLoginSucceeded?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Check if it's a Student account
        if (account.Role == (int)AccountRole.Student)
        {
            if (account.Student == null)
            {
                ErrorMessage = "Tài khoản Student này chưa được liên kết đúng. Vui lòng liên hệ Admin.";
                return;
            }

            StudentSession.Current = account.Student;
            StudentSession.CurrentAccount = account;
            StudentLoginSucceeded?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Instructor role: keep using the existing InstructorSession/Instructors flow.
        Instructor? instructor = account.Instructor ?? 
            (account.Email != null ? await AppServices.InstructorService.LoginAsync(account.Email) : null);

        if (instructor == null)
        {
            ErrorMessage = "Tài khoản Instructor này chưa được liên kết đúng. Vui lòng liên hệ Admin.";
            return;
        }

        InstructorSession.Current = instructor;
        InstructorLoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}

