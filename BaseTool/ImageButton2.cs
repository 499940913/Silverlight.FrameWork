using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BaseTool
{

    [TemplatePart(Name = "Container", Type = typeof(StackPanel))]
    [TemplatePart(Name = "ContainerShrink", Type = typeof(Storyboard))]
    [TemplatePart(Name = "ContainerExpand", Type = typeof(Storyboard))]
    [TemplatePart(Name = "Button", Type = typeof(ImageButton))]
    public class ImageButton2:ItemsControl,IDisposable
    {
        public event RoutedEventHandler Click;

        private string _displayname;
        private double _toheight;
        private StackPanel _container;
        private bool _isexpand;
        private Storyboard _shrink;
        private Storyboard _expand;
        private ImageButton _button;
        public ImageButton2()
        {
            DefaultStyleKey = typeof(ImageButton2);
            IsEnabledChanged+=ImageButton2_IsEnabledChanged;
            Loaded += (s, e) =>
            {
                _button.Click += (s1, e1) =>
                {
                    OnClick();
                };
                _displayname = Text;
                _toheight = 0;
                if (Items.Count > 0)
                {
                    foreach (var item in _container.Children)
                    {
                        var btn = item as ImageButton2;
                        if (btn != null)
                        {
                            _toheight += btn.ActualHeight;
                        }
                    }
                    Text = _displayname + "▲";
                    var doubleAnimationUsingKeyFrames = _expand.Children[0] as DoubleAnimationUsingKeyFrames;
                    if (doubleAnimationUsingKeyFrames != null)
                        doubleAnimationUsingKeyFrames.KeyFrames[1].Value = _toheight;
                    var animationUsingKeyFrames = _shrink.Children[0] as DoubleAnimationUsingKeyFrames;
                    if (animationUsingKeyFrames != null)
                        animationUsingKeyFrames.KeyFrames[0].Value = _toheight;
                }
            };
        }

        public static readonly DependencyProperty CommandProperty =

      DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageButton2), null);

        public ICommand Command {
            get
            {
                return (ICommand) GetValue(CommandProperty);
            }
            set
            { 
                SetValue(CommandProperty,value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _button=(ImageButton) GetTemplateChild("Button");
            if (Items.Count <= 0) return;
            _container = (StackPanel) GetTemplateChild("Container");
            _expand = (Storyboard)GetTemplateChild("ContainerExpand");
            _shrink = (Storyboard)GetTemplateChild("ContainerShrink");
            foreach (var item in Items)
            {
                var btn = item as ImageButton2;
                if (btn != null)
                {
                    _container.Children.Add(btn);
                }
            }
        }

        private bool _isSelected;

        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;

        public bool Selected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected == value) return;
                var e = new SelectedChangedEventArgs("SelectedProperty", _isSelected, value);
                _isSelected = value;
                if (SelectedChanged != null)
                {
                    SelectedChanged(this, e);
                }
            }
        }

        private static AccordionItem GetAccordionItem(FrameworkElement element)
        {
            var parent = element.Parent;
            if (parent == null) return null;
            var item = parent as AccordionItem;
            if (item != null)
            {
                return item;
            }
            var frameworkElement = parent as FrameworkElement;
            return frameworkElement != null ? GetAccordionItem(frameworkElement) : null;
        }

        protected virtual void OnClick()
        {
            Selected = !Selected;
            if (Click != null)
            {
                var e = new RoutedEventArgs();
                Click(this, e);
            }
            _isexpand = !_isexpand;
            double delta = 0;
            if (_container != null)
            {
                if (_isexpand)
                {
                    _shrink.Stop();
                    _expand.Begin();
                    Text = _displayname + "▼";
                    delta += _toheight;
                }
                else
                {
                    _expand.Stop();
                    _shrink.Begin();
                    Text = _displayname + "▲";
                    delta -= _toheight;
                }
            }
            var a = GetAccordionItem(this);
            if (a == null) return;
            var size = new Size
            {
                Width = a.ExpandSite.RenderSize.Width,
                Height = a.ExpandSite.RenderSize.Height + delta
            };
            a.ExpandSite.TargetSize = size;
            a.Schedule(AccordionAction.Resize);
        }

        void ImageButton2_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Visibility = (bool)e.NewValue == false ? Visibility.Collapsed : Visibility.Visible;
        }

        public void Dispose()
        {
            ClearValue(SourceProperty);
            ClearValue(TextProperty);
            ClearValue(ImageSizeProperty);
            if (Click != null)
            {
                foreach (var del in Click.GetInvocationList())
                    Click -= del as RoutedEventHandler;
                Click = null;
            }
            if (_expand != null)
            {
                _expand.Stop();
                _shrink.Stop();
                _expand = null;
                _shrink = null;
            }
            IsEnabledChanged -= ImageButton2_IsEnabledChanged;
            SelectedChanged = null;
            GC.Collect();
        }

        public static readonly DependencyProperty SourceProperty =

        DependencyProperty.Register("Source", typeof(BitmapImage), typeof(ImageButton2), null);

        public BitmapImage Source
        {

            get { return (BitmapImage)GetValue(SourceProperty); }

            set
            {
                SetValue(SourceProperty, value);
            }

        }

        public static DependencyProperty ImageSizeProperty =
             DependencyProperty.Register("ImageSize", typeof(double), typeof(ImageButton2), null);

        public double ImageSize
        {
            get {
                return (double)GetValue(ImageSizeProperty);
            }
            set
            {
                SetValue(ImageSizeProperty, value);
            }
        }

        public static   DependencyProperty TextProperty =

      DependencyProperty.Register("Text", typeof(string), typeof(ImageButton2), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set
            {
                SetValue(TextProperty,value);
            }
        }
    }
}
