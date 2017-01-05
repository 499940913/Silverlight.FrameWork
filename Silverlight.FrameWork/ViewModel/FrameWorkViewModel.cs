using System.ComponentModel;
using System.Xml.Linq;

namespace Silverlight.FrameWork.ViewModel
{

    public class FrameWorkViewModel:INotifyPropertyChanged,IViewModel,IFrameworkElementViewModel,IAssemblyViewModel
    {
        private string _headerHeight;
        public string HeaderHeight {
            get { return _headerHeight; }
            set
            {
                _headerHeight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HeaderHeight"));
            }
        }

        private string _visibility;

        public string Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
            }
        }

        public int? UiIndex { get; set; }

        public void InitiViewModel(XElement element, IAppFrame mFrame)
        {
            var assemblyLoader = mFrame.GetObject<AssemblyLoader>("AssemblyLoader");
          //  assemblyLoader.SetVisible(true);
            assemblyLoader.SetMessage(string.Format("正在初始化{0}对象...", element.Name));
            ViewModelHelper.ChangeViewModel(element,this);
        }
         
        public string Name { get; set;}

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private string _brush;

        public string FrameWorkBrush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FrameWorkBrush"));
            }
        }

        private string _foreground;

        public string Foreground {
            get { return _foreground; }
            set
            {
                _foreground= value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Foreground"));
            }
        }

        public string PackageName { get; set; }

        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        #region IFrameworkElementViewModel
        private bool _isenabled = true;

        public bool IsEnabled
        {
            get { return _isenabled; }
            set
            {
                _isenabled = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled"));
            }
        }

        private string _opacity;

        public string Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Opacity"));
            }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Content"));
            }
        }
        private string _height;

        public string Height
        {
            get { return _height; }
            set
            {
                _height = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Height"));
            }
        }

        private string _width;

        public string Width
        {
            get { return _width; }
            set
            {
                _width = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Width"));
            }
        }

        private string _margin;

        public string Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Margin"));
            }
        }

        private string _verticalAlignment;

        public string VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                _verticalAlignment = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("VerticalAlignment"));
            }
        }

        private string _horizontalAlignment;

        public string HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                _horizontalAlignment = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HorizontalAlignment"));
            }
        }

        public int? Row { get; set; }

        public int? RowSpan { get; set; }

        public int? Column { get; set; }

        public int? ColumnSpan { get; set; }

        public string Tip { get; set; }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
