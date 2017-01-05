using System.Windows;

namespace BaseTool
{
    public partial class BaseChildWindow 
    {
        #region 方法重写
        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.RootVisual.SetValue(IsEnabledProperty, true);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            Application.Current.RootVisual.SetValue(IsEnabledProperty, false);
        }
        #endregion

  

        public BaseChildWindow()
        {
            InitializeComponent();
        }

    
      
    }
}

