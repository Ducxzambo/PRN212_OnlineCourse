using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentProgressViewModel : ViewModelBase
{
    private List<Enrollment> _enrollments = new();
    private string? _errorMessage;
    private bool _isLoading;

    public List<Enrollment> Enrollments
    {
        get => _enrollments;
        set => SetProperty(ref _enrollments, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand UpdateProgressCommand { get; }

    public StudentProgressViewModel()
    {
        UpdateProgressCommand = new AsyncRelayCommand(UpdateProgressAsync);
    }

    public async Task LoadAsync()
    {
        if (StudentSession.Current == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var enrollments = await AppServices.StudentService.GetEnrollmentHistoryAsync(StudentSession.Current.Id);
            Enrollments = enrollments;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải tiến độ: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdateProgressAsync()
    {
        // This would typically be called with a selected enrollment
        // For now, it's a placeholder
        await LoadAsync();
    }
}
