using System.ComponentModel;
using System.Xml.Linq;
using BaseTool;


namespace Silverlight.FrameWork.ViewModel
{
    public class CommandViewModel : INotifyPropertyChanged, IViewModel, IFrameworkElementViewModel, ISourceViewModel,ICommandViewModel,IAssemblyViewModel,ITextViewMolde
    {
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

        private string _fontFamily;
        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FontFamily"));
            }
        }

        private string _fontweight;
        public string FontWeight
        {
            get { return _fontweight; }
            set
            {
                _fontweight = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FontWeight"));
            }
        }


        private string _fontsource;
        public string FontSource
        {
            get { return _fontsource; }
            set
            {
                _fontsource = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FontSource"));
            }
        }


        private IAppFrame _frame;
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

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        private string _fontSize;

        public string FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FontSize"));
            }
        }

        private string _foreground;
        
        public string Foreground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Foreground"));
            }
        }

        public void InitiViewModel(XElement element,IAppFrame mFrame)
        {
            _frame = mFrame;
            var assemblyLoader = _frame.GetObject<AssemblyLoader>("AssemblyLoader"); 
          //  assemblyLoader.SetVisible(true);
            assemblyLoader.SetMessage(string.Format("正在初始化{0}对象...", element.Name));
            ViewModelHelper.ChangeViewModel(element, this);
            _frame = mFrame;
            if (_frame==null||!element.HasElements|| XmlHelper.GetXElements(element, "Command").Length==0) return;
            var commanddoc = XmlHelper.GetXElements(element, "Command")[0];
            var cmd = ViewModelHelper.CreateViewModel<CommandViewModel>(commanddoc);
            AssemblyHelper.CreateCommandFromViewModel(cmd,this,_frame);
        }

        public string Name { get; set; }

        private string _source;

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Source"));
            }
        }

        private Command _command;
       
        public Command Command
        {
            get { return _command;}
            set
            {
                _command = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Command"));
            }
        }

        public string PackageName { get; set;}

        public string AssemblyName { get; set;}

        public string ClassName { get; set;}

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
