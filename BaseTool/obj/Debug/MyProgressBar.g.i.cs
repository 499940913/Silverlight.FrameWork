﻿#pragma checksum "F:\2016\危房推广\系统\BaseTool\MyProgressBar.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "443CBA4D965C602126E3A83953CE2EFE"
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


namespace BaseTool {
    
    
    public partial class MyProgressBar : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.SolidColorBrush Green;
        
        internal System.Windows.Media.SolidColorBrush Blue;
        
        internal System.Windows.Media.SolidColorBrush Yellow;
        
        internal System.Windows.Media.SolidColorBrush Orange;
        
        internal System.Windows.Media.SolidColorBrush Pink;
        
        internal System.Windows.Media.SolidColorBrush G1;
        
        internal System.Windows.Media.SolidColorBrush Y1;
        
        internal System.Windows.Media.SolidColorBrush G3;
        
        internal System.Windows.Media.SolidColorBrush B1;
        
        internal System.Windows.Shapes.Rectangle progress;
        
        internal System.Windows.Controls.TextBlock DescriptionTxt;
        
        internal System.Windows.Controls.TextBlock progressTxt;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/BaseTool;component/MyProgressBar.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Green = ((System.Windows.Media.SolidColorBrush)(this.FindName("Green")));
            this.Blue = ((System.Windows.Media.SolidColorBrush)(this.FindName("Blue")));
            this.Yellow = ((System.Windows.Media.SolidColorBrush)(this.FindName("Yellow")));
            this.Orange = ((System.Windows.Media.SolidColorBrush)(this.FindName("Orange")));
            this.Pink = ((System.Windows.Media.SolidColorBrush)(this.FindName("Pink")));
            this.G1 = ((System.Windows.Media.SolidColorBrush)(this.FindName("G1")));
            this.Y1 = ((System.Windows.Media.SolidColorBrush)(this.FindName("Y1")));
            this.G3 = ((System.Windows.Media.SolidColorBrush)(this.FindName("G3")));
            this.B1 = ((System.Windows.Media.SolidColorBrush)(this.FindName("B1")));
            this.progress = ((System.Windows.Shapes.Rectangle)(this.FindName("progress")));
            this.DescriptionTxt = ((System.Windows.Controls.TextBlock)(this.FindName("DescriptionTxt")));
            this.progressTxt = ((System.Windows.Controls.TextBlock)(this.FindName("progressTxt")));
        }
    }
}
