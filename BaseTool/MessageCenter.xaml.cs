using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BaseTool
{
    public partial class MessageCenter : IDisposable
    {
        public event EventHandler PageIndexChanged;
        public MessageCenter()
        {
            InitializeComponent();
            Loaded += MessageCenter_Loaded;
            CloseButton.Click+=CloseButton_Click;
            ExpandButton.SelectedChanged += ExpandButton_SelectedChanged;
            DataPage.PageIndexChanged += DataPage_PageIndexChanged;
        }

        void MessageCenter_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandButton.Selected = false;
        }

        private int _totalCount;

        private int _rowsize = 10;

        public int RowSize {
            get { return _rowsize; }
            set { _rowsize = value; }
        }

        public int SetTotalCount
        {
            set
            {
                _totalCount = value;
                var itemCount = new byte[_totalCount];
                var pcv = new PagedCollectionView(itemCount);
                DataPage.Source = pcv;
                DataPage.PageSize = _rowsize;
            }
        }

        public int CurrentPageIndex
        {
            get { return DataPage.PageIndex; }
        }

        private void DataPage_PageIndexChanged(object sender, EventArgs e)
    {
            if (Dispatcher.CheckAccess())
            {
                ClearChildren();
            }
            else
            {
                Dispatcher.BeginInvoke(ClearChildren);
            }
            if (PageIndexChanged != null)
            {
                PageIndexChanged(sender, e);
            }
        }

        ~MessageCenter()
        {
            Dispose(false);
        }
       
        void ExpandButton_SelectedChanged(object sender, SelectedChangedEventArgs e)
        {
            var imagename = "/BaseTool;component/images/down.png";
            var b = (ImageButton3)sender;
            if (b.Selected) imagename = "/BaseTool;component/images/up.png";
            b.Source= new BitmapImage(new Uri(imagename, 0));
            double height =25;
            if (b.Selected)
            {
                height += 25*(Container.Children.Count + 1)+5;
            }
            var doubleAnimationUsingKeyFrames = Expand.Children[0] as DoubleAnimationUsingKeyFrames;
            if (doubleAnimationUsingKeyFrames != null)
            {
                doubleAnimationUsingKeyFrames.KeyFrames[0].Value =25;
                doubleAnimationUsingKeyFrames.KeyFrames[1].Value = height;
            }
              
            var animationUsingKeyFrames = Shrink.Children[0] as DoubleAnimationUsingKeyFrames;
            if (animationUsingKeyFrames != null)
            {
                animationUsingKeyFrames.KeyFrames[0].Value = ActualHeight;
                animationUsingKeyFrames.KeyFrames[1].Value = height;
            }   
            if (b.Selected)
            {
                Shrink.Stop();
                Expand.Begin();
            }
            else
            {
                Expand.Stop();
                Shrink.Begin();
            }
        }

        public void AddButton(ImageButton3 button)
        {
          Container.Children.Add(button);
        }

        public void ClearChildren()
        {
            var children = Container.Children.Where(p=>p.GetType()==typeof(ImageButton3));
            var uiElements = children as UIElement[] ?? children.ToArray();
            if (uiElements.Any())
            {
                foreach (var obj in uiElements)
                {
                    var btn = (ImageButton3) obj;
                    btn.Dispose();
                }
            }
             Container.Children.Clear();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
           Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _mdispose;
        protected void Dispose(bool disposing)
        {
            if (_mdispose) return;
            ExpandButton.Dispose();
            CloseButton.Dispose();
            if (Dispatcher.CheckAccess())
            {
                 ClearChildren();
                 Loaded -= MessageCenter_Loaded;
                 CloseButton.Click -= CloseButton_Click;
                 ExpandButton.SelectedChanged -= ExpandButton_SelectedChanged;
                 DataPage.PageIndexChanged -= DataPage_PageIndexChanged;
            }
            else
            {
                Dispatcher.BeginInvoke(ClearChildren);
                Dispatcher.BeginInvoke(() => { ClearChildren(); Loaded -= MessageCenter_Loaded; CloseButton.Click -= CloseButton_Click; ExpandButton.SelectedChanged -= ExpandButton_SelectedChanged; DataPage.PageIndexChanged -= DataPage_PageIndexChanged; });
            }
            if (disposing)
            {
                if (PageIndexChanged != null)
                {
                    foreach (var del in PageIndexChanged.GetInvocationList())
                        PageIndexChanged -= del as EventHandler;
                    PageIndexChanged = null;
                }
            }
            _mdispose = true;
        }

        public void Close()
        {
             Visibility = Visibility.Collapsed;
             Dispose(true);
        }
    }
}
