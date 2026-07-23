using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class CategoryEditWindow : Window
    {
        public CategoryEditWindow(CategoryEditViewModel viewModel)
        {
            InitializeComponent();

            viewModel.SaveSucceeded += (s, e) => { DialogResult = true; };
            DataContext = viewModel;
        }
    }
}

