﻿#pragma checksum "F:\开发\危房模块化改造\Dangerous\Silverlight.FrameWork\Login.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "009E1C26CD8A9DB0C2948D2C629F9C8E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Silverlight.FrameWork {
    
    
    public partial class Login : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image BackGround;
        
        internal System.Windows.Controls.TextBlock Title;
        
        internal System.Windows.Controls.Image UserLogo;
        
        internal System.Windows.Controls.Image PwdLogo;
        
        internal System.Windows.Controls.TextBox User;
        
        internal System.Windows.Controls.PasswordBox Password;
        
        internal System.Windows.Controls.Button LoginButton;
        
        internal System.Windows.Controls.TextBlock Copyright;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Silverlight.FrameWork;component/Login.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.BackGround = ((System.Windows.Controls.Image)(this.FindName("BackGround")));
            this.Title = ((System.Windows.Controls.TextBlock)(this.FindName("Title")));
            this.UserLogo = ((System.Windows.Controls.Image)(this.FindName("UserLogo")));
            this.PwdLogo = ((System.Windows.Controls.Image)(this.FindName("PwdLogo")));
            this.User = ((System.Windows.Controls.TextBox)(this.FindName("User")));
            this.Password = ((System.Windows.Controls.PasswordBox)(this.FindName("Password")));
            this.LoginButton = ((System.Windows.Controls.Button)(this.FindName("LoginButton")));
            this.Copyright = ((System.Windows.Controls.TextBlock)(this.FindName("Copyright")));
        }
    }
}

