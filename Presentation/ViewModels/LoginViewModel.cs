using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _email = "";
    private string _password = "";
    private string? _errorMessage;
    public string Email 
    { 
        get => _email; 
        set => SetProperty(ref _email, value); 
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
    public event EventHandler? InstructorLoginSucceeded;
    public event EventHandler? AdminLoginSucceeded;
    public event EventHandler? StudentLoginSucceeded;

    public LoginViewModel() => LoginCommand = new RelayCommand(Login);

    private void Login()
    {
        ErrorMessage = null;
        var account = AppServices.AccountService.Login(Email, Password);
        if (account == null) 
        { 
            ErrorMessage = "Email, mật khẩu không đúng hoặc tài khoản đã bị khóa."; 
            return; 
        }

        if (account.Role == (int)AccountRole.Admin) 
        { 
            AdminSession.Current = account; 
            AdminLoginSucceeded?.Invoke(this, EventArgs.Empty); 
            return; 
        }

        if (account.Role == (int)AccountRole.Student)
        {
            if (account.Student == null) 
            { 
                ErrorMessage = "Tài khoản Student chưa có hồ sơ."; 
                return; 
            }
            StudentSession.Current = account.Student; 
            StudentLoginSucceeded?.Invoke(this, EventArgs.Empty); 
            return;
        }

        Instructor? instructor = account.Instructor;
        if (instructor == null) 
        { 
            ErrorMessage = "Tài khoản Instructor chưa có hồ sơ."; 
            return; 
        }
        InstructorSession.Current = instructor;
        InstructorLoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}

