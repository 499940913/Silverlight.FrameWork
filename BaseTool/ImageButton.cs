using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;

namespace BaseTool
{
    public class ImageButton : Button
    {

        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;

        private bool _isSelected;

        private string _mName;


        public ImageButton()
        {
            DefaultStyleKey = typeof(ImageButton);
            SelectedChanged += ImageButton_SelectedChanged;
        }

        void ImageButton_SelectedChanged(object sender, SelectedChangedEventArgs e)
        {
            var imagebutton = (ImageButton)sender;
            var sb1 = (Storyboard)GetTemplateChild("BorderTK2");
            var sb2= (Storyboard)GetTemplateChild("BorderTK1");
            if (sb1 == null) return;
            if (sb2 == null) return;
            try
            {
                if (imagebutton.Selected)
                {
                    var panel = imagebutton.Parent as StackPanel;
                    if (panel != null)
                    {
                        var stk = panel;
                        var buttons = from child in stk.Children where child is ImageButton && !sender.Equals(child) && ((ImageButton)child).Selected select (ImageButton)child;
                        foreach (var btn in buttons)
                        {
                            btn.Selected = false;
                        }
                    }
                    sb1.Stop();
                    sb2.Begin();
                }
                else
                {
                    sb1.Begin();
                    sb2.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static readonly DependencyProperty SourceProperty =

        DependencyProperty.Register("Source", typeof(BitmapImage), typeof(ImageButton), null);

        /// <summary>
        /// 照片名称
        /// </summary>
        /// 
  
        public string ImageName
        {
            get 
            {
                return _mName;
            }
            set
            {
                _mName = value;
            }
        }

        /// <summary>

        /// 获取或设置标题图标

        /// </summary>

        public BitmapImage Source
        {

            get { return (BitmapImage)GetValue(SourceProperty); }

            set {
                SetValue(SourceProperty, value);
            }

        }

        public void ClearUp()
        {
            try
            {
                SelectedChanged -= ImageButton_SelectedChanged;
                SelectedChanged = null;
                var sb1 = (Storyboard)GetTemplateChild("BorderTK2");
                var sb2 = (Storyboard)GetTemplateChild("BorderTK1");
                sb1.Stop();
                sb2.Stop();
                ClearValue(SourceProperty);
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        /// <summary>

        /// 判断照片是否选中

        /// </summary>
        /// 
        public bool Selected
        {
            get {
                return _isSelected; 
            }

            set {
                if (_isSelected == value) return;
                _isSelected = value;
                if (SelectedChanged != null)
                {
                    var e = new SelectedChangedEventArgs("SelectedProperty", _isSelected, value);
                    SelectedChanged(this,e);
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            Selected = !Selected;
        }
    }

    public class SelectedChangedEventArgs : EventArgs
    {
        public SelectedChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Cancel { get; set; }
        public string PropertyName { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; set; }
    }
}
