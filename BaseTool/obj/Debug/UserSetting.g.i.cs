﻿#pragma checksum "F:\2015\瑞安危房\系统\BaseTool\UserSetting.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E7D7F6824C6DCB9E794BF52343402A58"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using BaseTool;
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


namespace BaseTool {
    
    
    public partial class UserSetting : BaseTool.BaseChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox UserName;
        
        internal System.Windows.Controls.TextBox PassWord;
        
        internal System.Windows.Controls.ComboBox DeptCombobox;
        
        internal System.Windows.Controls.ComboBox RoleCombobox;
        
        internal System.Windows.Controls.Button OKButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/BaseTool;component/UserSetting.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.UserName = ((System.Windows.Controls.TextBox)(this.FindName("UserName")));
            this.PassWord = ((System.Windows.Controls.TextBox)(this.FindName("PassWord")));
            this.DeptCombobox = ((System.Windows.Controls.ComboBox)(this.FindName("DeptCombobox")));
            this.RoleCombobox = ((System.Windows.Controls.ComboBox)(this.FindName("RoleCombobox")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
        }
    }
}

