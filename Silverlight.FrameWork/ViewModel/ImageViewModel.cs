using System.ComponentModel;
using System.Xml.Linq;

namespace Silverlight.FrameWork.ViewModel
{
    public class ImageViewModel : INotifyPropertyChanged, IViewModel, IFrameworkElementViewModel, ISourceViewModel, IAssemblyViewModel
    {
        #region IFrameworkElementViewModel

        private string _stretch;
        public string Stretch {
            get { return _stretch; }
            set
            {
                _stretch = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Stretch"));
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

        public int? UiIndex { get; set; }

        public void InitiViewModel(XElement element, IAppFrame mFrame)
        {
            var assemblyLoader = mFrame.GetObject<AssemblyLoader>("AssemblyLoader");
          //  assemblyLoader.SetVisible(true);
            assemblyLoader.SetMessage(string.Format("正在初始化{0}对象...", element.Name));
          ViewModelHelper.ChangeViewModel(element,this);
        }

        public string Name { get; set;}

        private string _source;
      
        public string Source {
            get { return _source;}
            set
            {
                _source = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Source"));
            }
        }

        public string PackageName { get; set; }

        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
