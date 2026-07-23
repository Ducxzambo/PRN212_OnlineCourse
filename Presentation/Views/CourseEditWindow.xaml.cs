using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class CourseEditWindow : Window
    {
        public CourseEditWindow(CourseEditViewModel viewModel)
        {
            InitializeComponent();

            viewModel.SaveSucceeded += (s, e) => { DialogResult = true; };
            DataContext = viewModel;
        }
    }
}

