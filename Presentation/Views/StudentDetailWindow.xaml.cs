using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class StudentDetailWindow : Window
    {
        public StudentDetailWindow(StudentDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
