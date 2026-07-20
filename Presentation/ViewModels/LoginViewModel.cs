using System.Windows.Input;
using Presentation.Helpers;

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

    public event EventHandler? LoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = null;

        var instructor = await AppServices.InstructorService.LoginAsync(Email);
        if (instructor == null)
        {
            ErrorMessage = "Không tìm thấy Instructor với email này. Vui lòng kiểm tra lại.";
            return;
        }

        InstructorSession.Current = instructor;
        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
