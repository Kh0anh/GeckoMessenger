﻿#pragma checksum "..\..\..\..\Views\Settings\EditProfileUserControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "97EA740F89E84DE14DCA37DDB71297DF1443734419D03FA35C9E38218205EF1F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Expression.Media;
using HandyControl.Expression.Shapes;
using HandyControl.Interactivity;
using HandyControl.Media.Animation;
using HandyControl.Media.Effects;
using HandyControl.Properties.Langs;
using HandyControl.Themes;
using HandyControl.Tools;
using HandyControl.Tools.Converter;
using HandyControl.Tools.Extension;
using Material.Icons.WPF;
using Messenger.ViewModels;
using Messenger.Views.Settings;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Messenger.Views.Settings {
    
    
    /// <summary>
    /// EditProfileUserControl
    /// </summary>
    public partial class EditProfileUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 50 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenEditImageContext;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleUsernameEdit;
        
        #line default
        #line hidden
        
        
        #line 252 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleFullNameEdit;
        
        #line default
        #line hidden
        
        
        #line 364 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleEmailEdit;
        
        #line default
        #line hidden
        
        
        #line 459 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton TogglePhoneNumberEdit;
        
        #line default
        #line hidden
        
        
        #line 554 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleBirthdayEdit;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Messenger;component/views/settings/editprofileusercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.OpenEditImageContext = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\..\..\Views\Settings\EditProfileUserControl.xaml"
            this.OpenEditImageContext.Click += new System.Windows.RoutedEventHandler(this.OpenEditImageContext_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ToggleUsernameEdit = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 3:
            this.ToggleFullNameEdit = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 4:
            this.ToggleEmailEdit = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 5:
            this.TogglePhoneNumberEdit = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 6:
            this.ToggleBirthdayEdit = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

