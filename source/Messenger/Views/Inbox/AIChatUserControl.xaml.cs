﻿using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Messenger.Views.Inbox
{
    /// <summary>
    /// Interaction logic for AIChatUserControl.xaml
    /// </summary>
    public partial class AIChatUserControl : UserControl
    {
        public AIChatViewModel VM { get; set; } 
        public AIChatUserControl(AIChatViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            if (DataContext is AIChatViewModel vm)
            {
                VM = vm;
                vm.ScrollToEnd += () =>
                {
                    MessagesScrollViewer.ScrollToEnd();
                };
            }
        }
    }
}
