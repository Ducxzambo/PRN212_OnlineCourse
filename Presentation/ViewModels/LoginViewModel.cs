using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _email = "";
    private string? _errorMessage;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
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

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = null;

        var account = await AppServices.AccountService.LoginAsync(Email);
        if (account == null)
        {
            ErrorMessage = "Không tìm thấy tài khoản với email này, hoặc tài khoản đã bị khóa.";
            return;
        }

        if (account.Role == (int)AccountRole.Admin)
        {
            AdminSession.Current = account;
            AdminLoginSucceeded?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Instructor role: keep using the existing InstructorSession/Instructors flow.
        Instructor? instructor = account.InstructorId.HasValue
            ? await AppServices.InstructorService.LoginAsync(account.Email)
            : null;

        if (instructor == null)
        {
            ErrorMessage = "Tài khoản Instructor này chưa được liên kết đúng. Vui lòng liên hệ Admin.";
            return;
        }

        InstructorSession.Current = instructor;
        InstructorLoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
