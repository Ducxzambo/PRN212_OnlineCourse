using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class InstructorDetailWindow : Window
    {
        public InstructorDetailWindow(InstructorDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

