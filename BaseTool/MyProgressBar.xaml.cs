using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace BaseTool
{
    public partial class MyProgressBar : IDisposable
    {
       // private string _name;
        private string _des;
        private int _progressbarvalue;
        readonly List<string> _colors = new List<string>();
        public MyProgressBar()
        {
            InitializeComponent();
            _des = "";
            Description = _defaultdes;
            _colors.Add("Green");
            _colors.Add("Blue");
            _colors.Add("Yellow");
            _colors.Add("Orange");
            _colors.Add("Pink");
            _colors.Add("G1");
            _colors.Add("Y1");
            _colors.Add("G3");
            _colors.Add("B1");
        }
        private string _defaultdes="等待上传...";
        public string Description
        {
            get
            {
                return _des;
            }
            set
            {
                _des = value;
                DescriptionTxt.Text = _des;
                if (_colors.Count!=0)
                {
                    var ra = new Random();
                    var i = ra.Next(0, 8);
                    progress.Fill = (SolidColorBrush)LayoutRoot.Resources[_colors[i]];
                }
            }
        }
        public int ProgressBarValue
        {
            get 
            {
                return _progressbarvalue;
            }
            set
            {
                if (value < 0)
                {
                    _progressbarvalue = 0;
                }
                if (value >=100)
                {
                    _progressbarvalue = 100;
                    DescriptionTxt.Text = "完成！";
                }
                else
                {
                    _progressbarvalue = value;
                }
                var percent = Convert.ToDouble(_progressbarvalue) / 100;
                if (percent < 0) percent = 0;
                var width = LayoutRoot.ActualWidth * percent;
                progress.Width = width;
                progressTxt.Text = ProgressBarValue+"%";
            }
        }
        public void Rest()
        {
            Description = _defaultdes;
            ProgressBarValue = 0;
        }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
