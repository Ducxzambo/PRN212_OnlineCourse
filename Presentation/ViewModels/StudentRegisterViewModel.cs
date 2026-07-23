using System.Windows.Input;
using Presentation.Helpers;
namespace Presentation.ViewModels;

public class StudentRegisterViewModel : ViewModelBase
{
    private string _fullName = "", _email = "", _password = ""; private string? _error;
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
    public string Password 
    { 
        get => _password; 
        set => SetProperty(ref _password, value); 
    }
    public string? Error 
    { 
        get => _error; 
        set => SetProperty(ref _error, value); 
    }
    public ICommand RegisterCommand { get; }
    public event EventHandler? Registered;
    public StudentRegisterViewModel() => RegisterCommand = new RelayCommand(Register);
    private void Register()
    {
        var r = AppServices.AccountService.RegisterStudent(FullName, Email, Password, null); 
        if (!r.Success) { 
            Error = r.Error; 
            return; 
        }
        Registered?.Invoke(this, EventArgs.Empty);
    }
}

