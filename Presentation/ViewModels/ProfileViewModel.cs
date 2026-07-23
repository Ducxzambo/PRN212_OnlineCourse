using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    private readonly Account _account;
    private string _fullName;
    private string _email;
    private string _newPassword = "";
    private string? _phone;
    private string? _specialization;
    private DateTime? _dateOfBirth;
    private string? _errorMessage;

    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
    public string? Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string? Specialization { get => _specialization; set => SetProperty(ref _specialization, value); }
    public DateTime? DateOfBirth { get => _dateOfBirth; set => SetProperty(ref _dateOfBirth, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public bool IsInstructor => _account.Role == (int)AccountRole.Instructor;
    public bool IsStudent => _account.Role == (int)AccountRole.Student;
    public string RoleName => IsInstructor ? "Instructor" : "Student";
    public string CreatedDate => _account.CreatedDate.ToString("dd/MM/yyyy");
    public ICommand SaveCommand { get; }
    public event EventHandler? SaveSucceeded;

    public ProfileViewModel(Account account)
    {
        _account = account;
        _fullName = account.FullName;
        _email = account.Email;
        _phone = account.Instructor?.Phone ?? account.Student?.Phone;
        _specialization = account.Instructor?.Specialization;
        _dateOfBirth = account.Student?.DateOfBirth;
        SaveCommand = new RelayCommand(Save);
    }

    private void Save()
    {
        ErrorMessage = null;
        var result = AppServices.AccountService.UpdateOwnProfile(_account.Id, FullName, Email, NewPassword, Phone, Specialization, DateOfBirth);
        if (!result.Success) { ErrorMessage = result.Error; return; }

        _account.FullName = FullName.Trim();
        _account.Email = Email.Trim();
        if (!string.IsNullOrWhiteSpace(NewPassword)) _account.Password = NewPassword;
        if (_account.Instructor != null) { _account.Instructor.Phone = Phone; _account.Instructor.Specialization = Specialization; }
        if (_account.Student != null) { _account.Student.Phone = Phone; _account.Student.DateOfBirth = DateOfBirth; }
        SaveSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
