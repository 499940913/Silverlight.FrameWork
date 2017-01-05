using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BaseTool
{
    public class ImageButton3:Button,IDisposable
    {
        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;
        public new event EventHandler Click;
        private bool _mdispose;
        private System.Windows.Input.MouseButtonEventHandler _clickHandler;
        public ImageButton3()
        {
            DefaultStyleKey = typeof(ImageButton3);
            _clickHandler = ImageButton3_MouseLeftButtonUp;
           AddHandler(MouseLeftButtonUpEvent, _clickHandler, true);
        }

        void ImageButton3_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Click!=null)
            {
                Click(sender, e);
            }
        }

        ~ImageButton3()
        {
            Dispose(false);
        }

        protected override void OnClick()
        {
            Selected = !Selected;
            base.OnClick();
        }

        public void Dispose()
        {
            Dispose(true);
           GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_mdispose) return;
            if (_clickHandler != null)
            {
                if (Dispatcher.CheckAccess())
                {
                    RemoveHandler(MouseLeftButtonUpEvent, _clickHandler);
                }
                else
                {
                    if (_clickHandler != null)
                    {
                        Dispatcher.BeginInvoke(() => { RemoveHandler(MouseLeftButtonUpEvent, _clickHandler); });
                    }
                }
                _clickHandler = null;
            }
            if (disposing)
            {
                if (Dispatcher.CheckAccess())
                {
                    ClearValue(SourceProperty);
                    ClearValue(TextProperty);
                    ClearValue(ImageSizeProperty);
                }
                else
                {
                    Dispatcher.BeginInvoke(() => { ClearValue(SourceProperty); ClearValue(TextProperty); ClearValue(ImageSizeProperty); });
                }
                if (SelectedChanged != null)
                {
                    foreach (var del in SelectedChanged.GetInvocationList())
                        SelectedChanged -= del as EventHandler<SelectedChangedEventArgs>;
                    SelectedChanged = null;
                }
                if (Click != null)
                {
                    foreach (var del in Click.GetInvocationList())
                        Click -= del as EventHandler;
                    Click = null;
                }
            }
            _mdispose = true;
        }

        public static readonly DependencyProperty SourceProperty =
     DependencyProperty.Register("Source", typeof(BitmapImage), typeof(ImageButton3), null);
        public BitmapImage Source
        {

            get { return (BitmapImage)GetValue(SourceProperty); }

            set
            {
                SetValue(SourceProperty, value);
            }

        }

        private bool _isSelected;

        public bool Selected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    var e = new SelectedChangedEventArgs("SelectedProperty", _isSelected, value);
                    _isSelected = value;
                    if (SelectedChanged != null)
                    {
                        SelectedChanged(this, e);
                    }
                }
            }
        }

        public static DependencyProperty TextProperty =
    DependencyProperty.Register("Text", typeof(string), typeof(ImageButton3), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static DependencyProperty ImageSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(double), typeof(ImageButton3), null);

        public double ImageSize
        {
            get
            {
                return (double)GetValue(ImageSizeProperty);
            }
            set
            {
                SetValue(ImageSizeProperty, value);
            }
        }

    }
}
