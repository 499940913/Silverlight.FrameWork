using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using BaseTool;

namespace Silverlight.FrameWork.ViewModel
{ 
    public class AccordionItemViewModel:IHeaderContentViewModel,INotifyPropertyChanged,IViewModel,ITextViewMolde
    {
        private string _header;

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


        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                if (PropertyChanged == null) return;
                PropertyChanged(this, new PropertyChangedEventArgs("Header"));
            }
        }

        public string Name { get; set;}

        public void InitiViewModel(XElement element, IAppFrame mFrame)
        {
            ViewModelHelper.ChangeViewModel(element,this);
            _commands.Clear();
            var items = XmlHelper.GetXElements(element, "AccordionCommandViewModel");
            foreach (var item in items)
            {
                var accordionCommandViewModel = new AccordionCommandViewModel();
                accordionCommandViewModel.InitiViewModel(item, mFrame);
                _commands.Add(accordionCommandViewModel);
            }
        }


        public AccordionItemViewModel()
        {
            _commands=new List<AccordionCommandViewModel>();
        }

        //存储commands

        private readonly List<AccordionCommandViewModel> _commands;
        public List<AccordionCommandViewModel> Commands {
            get { return _commands; } 
        }

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
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
