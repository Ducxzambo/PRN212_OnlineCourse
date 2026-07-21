using System.Windows;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public partial class AccountEditWindow : Window
    {
        public AccountEditWindow(AccountEditViewModel viewModel)
        {
            InitializeComponent();

            viewModel.SaveSucceeded += (s, e) => { DialogResult = true; };
            DataContext = viewModel;
        }
    }
}
