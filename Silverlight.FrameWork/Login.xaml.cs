using System.Windows;
using System.Windows.Controls;
using Silverlight.FrameWork.ViewModel;

namespace Silverlight.FrameWork
{
    public partial class Login:ILayout
    {
        private readonly IAppFrame _frame;
        public Login(IAppFrame frame)
        { 
            InitializeComponent();
            _frame = frame;
            _frame.AddObject("Login", this);
            Loaded += (s,e) =>
            {
                Binding<ImageViewModel>("ImageViewModel");
                Binding<TextViewModel>("TextViewModel");
                Binding<CommandViewModel>("CommandViewModel");
                LayoutRoot.Children.Add(_frame.GetObject<AssemblyLoader>("AssemblyLoader"));
                var timeout = _frame.GetObject<string>("timeout");
                if (!string.IsNullOrEmpty(timeout)) _frame.TimeOut = int.Parse(timeout);
            };
        }
        private void Binding<T>(string tag) where T:IViewModel,new ()
        {
            var ui = _frame.GetObject<string>("LoginConfig");
            var document = BaseTool.XmlHelper.GetXDocument(ui);
            var rootElement = document.Root;
            var children = BaseTool.XmlHelper.GetXElements(rootElement, tag);
          
            foreach (var child in children)
            {
                var  viewmodel=new T();
                viewmodel.InitiViewModel(child,_frame);
                if (string.IsNullOrEmpty(viewmodel.Name)) continue;
                var element = FindName(viewmodel.Name);
                if (element == null) continue;
                BindingHelper.Binding(viewmodel, (FrameworkElement)element);
            }
        }
        public Grid GetLayoutRoot()
        {
             return LayoutRoot;
        }
    }
}
