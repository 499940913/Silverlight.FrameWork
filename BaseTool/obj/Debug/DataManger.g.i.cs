﻿#pragma checksum "F:\2016\危房推广\系统\BaseTool\DataManger.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EB1D89DF3DD7FFC9281F38D0C0399773"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using BaseTool;
using Liquid;
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
    
    
    public partial class DataManger : BaseTool.BaseChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Shapes.Rectangle HeardBK;
        
        internal System.Windows.Controls.Grid Header;
        
        internal System.Windows.Shapes.Rectangle HeaderMid;
        
        internal System.Windows.Controls.Button startUpload;
        
        internal System.Windows.Controls.Button Downloadfile;
        
        internal System.Windows.Controls.Button ViewOnline;
        
        internal System.Windows.Controls.Button deleteFile;
        
        internal System.Windows.Controls.TextBlock TitleText;
        
        internal System.Windows.Controls.ColumnDefinition leftColumn;
        
        internal Liquid.Tree testTree;
        
        internal System.Windows.Controls.GridSplitter grsplSplitter;
        
        internal Liquid.ItemViewer items;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/BaseTool;component/DataManger.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.HeardBK = ((System.Windows.Shapes.Rectangle)(this.FindName("HeardBK")));
            this.Header = ((System.Windows.Controls.Grid)(this.FindName("Header")));
            this.HeaderMid = ((System.Windows.Shapes.Rectangle)(this.FindName("HeaderMid")));
            this.startUpload = ((System.Windows.Controls.Button)(this.FindName("startUpload")));
            this.Downloadfile = ((System.Windows.Controls.Button)(this.FindName("Downloadfile")));
            this.ViewOnline = ((System.Windows.Controls.Button)(this.FindName("ViewOnline")));
            this.deleteFile = ((System.Windows.Controls.Button)(this.FindName("deleteFile")));
            this.TitleText = ((System.Windows.Controls.TextBlock)(this.FindName("TitleText")));
            this.leftColumn = ((System.Windows.Controls.ColumnDefinition)(this.FindName("leftColumn")));
            this.testTree = ((Liquid.Tree)(this.FindName("testTree")));
            this.grsplSplitter = ((System.Windows.Controls.GridSplitter)(this.FindName("grsplSplitter")));
            this.items = ((Liquid.ItemViewer)(this.FindName("items")));
        }
    }
}

