using System;
using System.Windows;
using System.Windows.Controls;
using Silverlight.FrameWork.ViewModel;


namespace Silverlight.FrameWork
{
    public partial class MainPage : ILayout
    {
        private readonly IAppFrame _frame;

        public MainPage(AppFrame frame)
        {
            InitializeComponent();
            _frame = frame;
            var assemblyLoader = _frame.GetObject<AssemblyLoader>("AssemblyLoader");
            var login = _frame.GetObject<ILayout>("login");
            try
            {
                if (login.GetLayoutRoot().Children.Contains(assemblyLoader))
                    login.GetLayoutRoot().Children.Remove(assemblyLoader);
                if (!LayoutRoot.Children.Contains(assemblyLoader))
                    LayoutRoot.Children.Add(assemblyLoader);
            }
            catch (Exception)
            {
                // ignored
            }
            Loaded += (s, e) =>
            {
                _frame.AddObject("layout", GetLayoutRoot());
                _frame.AddObject("main", this);
                var frameWorkViewModel = (FrameWorkViewModel)Resources["FrameWorkViewModel"];
                frame.AddObject("FrameWorkViewModel", frameWorkViewModel);
                frame.AddObject("TitleControls", TitleControls);
                frame.AddObject("Accordion", Accordion);
                frame.AddObject("Container", Container);
                if (!frame.Build_UI())
                {
                    MessageBox.Show("创建界面失败！");
                }
            };
        }

        public Grid GetLayoutRoot()
        {
            return LayoutRoot;
        }
    }


}
