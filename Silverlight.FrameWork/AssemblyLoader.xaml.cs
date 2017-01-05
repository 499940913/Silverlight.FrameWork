using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Silverlight.FrameWork
{
    public partial class AssemblyLoader
    {
        private readonly object _lock=new object();

        private readonly Dictionary<int, string> _message;

        private int _displayindex;

        private readonly DispatcherTimer _timer;

        public AssemblyLoader()
        {
            InitializeComponent();
            Visibility=Visibility.Collapsed;
            Grid.SetColumnSpan(this,5);
            Grid.SetRowSpan(this, 5);
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };//延时显示消息体现项目的动态加载过程，噱头
            _timer.Tick += (s, e) =>
            {
                if (_displayindex >= _message.Count)
                {
                    SetVisible(false);
                    _timer.Stop();
                    return;
                }
                Display();
            };
            _message=new Dictionary<int, string>();
        }

        private void SetVisible(bool visible)
        {
            Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Display()
        {  
            SetVisible(true);
            Message.Text = _message[_displayindex];
            _displayindex++;
        }

        public void SetMessage(string message)
        {
            //延时显示结果不然一闪而过
            lock (_lock)
            {
                var i = _message.Count;
                _message.Add(i,message);
                if (_timer.IsEnabled) return;
                 _timer.Start();
            }
        }
    }
}
