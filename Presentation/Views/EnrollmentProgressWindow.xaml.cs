using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class EnrollmentProgressWindow : Window
{
    public EnrollmentProgressWindow(EnrollmentProgressViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
