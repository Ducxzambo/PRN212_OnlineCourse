using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class LessonEditWindow : Window
    {
        public LessonEditWindow(LessonEditViewModel viewModel)
        {
            InitializeComponent();

            viewModel.SaveSucceeded += (s, e) => { DialogResult = true; };
            DataContext = viewModel;
        }
    }
}

