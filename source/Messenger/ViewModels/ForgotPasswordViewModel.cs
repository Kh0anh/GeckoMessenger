using Messenger.Services;
using Messenger.Utils;
using Messenger.Views;
using ServiceStack;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Messenger.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        public ICommand ForgotPasswordCommand { get; set; }
        public MainViewModel _MainViewModel { get; set; }
        public ForgotPasswordViewModel(MainViewModel mainViewModel)
        {
            _MainViewModel = mainViewModel;
            ForgotPasswordCommand = new RelayCommand(_ => Abc());
        }

        private void Abc()
        {
            throw new NotImplementedException();
        }
    }
}
