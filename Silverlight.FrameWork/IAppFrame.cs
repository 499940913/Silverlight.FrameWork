using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using BaseTool;
using Silverlight.FrameWork.ViewModel;


// ReSharper disable NotAccessedField.Local

namespace Silverlight.FrameWork
{
    public interface IAppFrame
    {
        
        int TimeOut { get; set;}

        bool AddObject(string key, object obj);

        bool AddObjectOverWrite(string key, object obj);

        object GetObject(string key);

        T GetObject<T>(string key) where T : class;

        bool Build_UI();
    }

    public class AppFrame : IAppFrame,IDisposable
    {
        private int _timeOut = 1000;//默认1秒get无值就为空

        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value;}
        }

        private readonly object _lock;
       
        private bool _hasdispose;


        ~AppFrame()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (_hasdispose) return;
            if (_objects!=null&&_objects.Count>0)
            {
                foreach (var obj in _objects.Values)
                {
                    var disposable = obj as IDisposable;
                    if (disposable != null)
                      disposable.Dispose();
                }
                _objects.Clear();
            }
            if (dispose) GC.SuppressFinalize(this);
            _hasdispose = true;
        }

        private readonly Dictionary<string, object> _objects;

        public AppFrame()
        {
            _objects = new Dictionary<string, object>();
            _lock=new object();
        }

        public bool AddObjectOverWrite(string key, object obj)
        {
            if (string.IsNullOrEmpty(key)) return false;
            key = key.ToLower();
            if (_objects.Keys.Contains(key))
                _objects.Remove(key);
            _objects.Add(key, obj);
            return true;
        }

        public bool AddObject(string key, object obj)
        {
            if (string.IsNullOrEmpty(key)) return false; 
            key = key.ToLower();
            if (_objects.Keys.Contains(key)) return false;
            _objects.Add(key, obj);
            return true;
        }

        //这些都是同步获取的
        public object GetObject(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            key = key.ToLower();
            return _objects.Keys.Contains(key) ? _objects[key] : null;
        }

        //同步获取
        public T GetObject<T>(string key) where T : class
        {
            var o = GetObject(key);
            var o1 = o as T;
            return o1;
        }

      

        private void BuildTitleControls(XDocument document)
        {
            var titlecontainer=GetObject<StackPanel>("TitleControls");
            titlecontainer.Children.Clear();
            var assemblyLoader = GetObject<AssemblyLoader>("AssemblyLoader");
            var titleelement = XmlHelper.GetXElements(document.Root, "TitleControls");
            if (titleelement.Length == 0) return;
            assemblyLoader.SetMessage("开始创建标题栏元素...");
            var elements = titleelement.Elements();
            var xElements = elements as XElement[] ?? elements.ToArray();
            for (var i = 0; i < xElements.Length; i++)
            {
                titlecontainer.Children.Add(new StackPanel());
            }
            foreach (var element in xElements)
            {
                var o = ViewModelHelper.CreatViewModel(element);
                if (o == null) continue;
                var viewModel = (IViewModel)o;
                viewModel.InitiViewModel(element, this);
                assemblyLoader.SetMessage(string.Format("开始创建{0}元素...", viewModel.Name));
                var model = o as IAssemblyViewModel;
                if (model == null) continue;
                var assemblyViewModel = model;
                var assemblyProxy = new AssemblyProxy();
                assemblyProxy.GetAssemblyCompleted += (s, e) =>
                {
                    var assembly = e.Assembly;
                    if (assembly == null) return;
                    assemblyLoader.SetMessage(string.Format("开始创建{0}的实例，类型为{1}！", viewModel.Name, assemblyViewModel.ClassName));
                    var instance = assembly.CreateInstance(assemblyViewModel.ClassName);
                    if (instance == null) return;
                    assemblyLoader.SetMessage(string.Format("创建{0}的实例成功，类型为{1}！", viewModel.Name, assemblyViewModel.ClassName));
                    var uiElement = (FrameworkElement)instance;
                    BindingHelper.Binding(o, uiElement);
                  //  assemblyLoader.SetVisible(false);
                    ReplaceUiElement(titlecontainer.Children, assemblyViewModel, uiElement);
                };
                assemblyProxy.GetAssembly(assemblyViewModel, this);
            }
        }

        private void BuildAccordion(XDocument document)
        {
            var accordionContainer = GetObject<Accordion>("Accordion");
            accordionContainer.Items.Clear();
            var assemblyLoader = GetObject<AssemblyLoader>("AssemblyLoader");
            var accordionelement = XmlHelper.GetXElements(document.Root, "Accordion");
            if (accordionelement.Length ==0 ) return;
            assemblyLoader.SetMessage("开始创建手风琴样式菜单栏...");
            var accordionItems= accordionelement.Elements();
            foreach (var  item in accordionItems)
            {
                var accordionItemViewModel = new AccordionItemViewModel();
                accordionItemViewModel.InitiViewModel(item, this);
                var accordionItem = new AccordionItem();
                BindingHelper.Binding(accordionItemViewModel, accordionItem);
                var contentcontainer = new StackPanel();
                accordionItem.Content = contentcontainer;
                accordionContainer.Items.Add(accordionItem);
                foreach (var command in accordionItemViewModel.Commands)
                {
                    var imageButton2=new ImageButton2();
                    contentcontainer.Children.Add(imageButton2);
                    AppendAccordionCommand(imageButton2,command);
                    BindingHelper.Binding(command, imageButton2);
                }
            }
        }

        private static void AppendAccordionCommand(ItemsControl imageButton,AccordionCommandViewModel viewModel)
        {
            foreach (var command in viewModel.Children)
            {
                var imageButton2 = new ImageButton2();
                imageButton.Items.Add(imageButton2);
                AppendAccordionCommand(imageButton2, command);
                BindingHelper.Binding(command, imageButton2);
            }
        }

        public void ReplaceUiElement(UIElementCollection container,IAssemblyViewModel viewModel,FrameworkElement uiElement)
        {
            if (!viewModel.UiIndex.HasValue)
            {
                MessageBox.Show("界面元素无索引值！");
                throw new Exception("界面元素无索引值！");
            }
            container.Insert(viewModel.UiIndex.Value, uiElement);
        }

        private void BuildContainer(XDocument document)
        {
            var container = GetObject<Grid>("Container");
            container.Children.Clear();
            var assemblyLoader = GetObject<AssemblyLoader>("AssemblyLoader");
            var containerelement = XmlHelper.GetXElements(document.Root, "Container");
            if (containerelement.Length == 0) return;
            assemblyLoader.SetMessage("开始创建主界面容器...");
            var elements = containerelement.Elements();
            var xElements = elements as XElement[] ?? elements.ToArray();
            for (var i = 0; i < xElements.Length; i++)
            {
               container.Children.Add(new StackPanel());
            }
            foreach (var element in xElements)
            {
                var o = ViewModelHelper.CreatViewModel(element);
                if (o == null) continue;
                var viewModel = (IViewModel)o;
                viewModel.InitiViewModel(element, this);
                assemblyLoader.SetMessage(string.Format("开始创建{0}元素...", viewModel.Name));
                var model = o as IAssemblyViewModel;
                if (model == null) continue;
                var assemblyViewModel = model;
                var assemblyProxy = new AssemblyProxy();
                //异步操作有问题好烦啊
                assemblyProxy.GetAssemblyCompleted += (s, e) =>
                {
                    var assembly = e.Assembly;
                    if (assembly == null) return;
                    assemblyLoader.SetMessage(string.Format("开始创建{0}的实例，类型为{1}！", viewModel.Name, assemblyViewModel.ClassName));
                    var instance = assembly.CreateInstance(assemblyViewModel.ClassName);
                    if (instance == null) return;
                    assemblyLoader.SetMessage(string.Format("创建{0}的实例成功，类型为{1}！", viewModel.Name, assemblyViewModel.ClassName));
                    var uiElement = (FrameworkElement)instance;
                    BindingHelper.Binding(o, uiElement);
                    ((FrameWorkControl)uiElement).OnCreate(this);
                  //  assemblyLoader.SetVisible(false);
                   ReplaceUiElement(container.Children,assemblyViewModel,uiElement);
                };
                assemblyProxy.GetAssembly(assemblyViewModel, this);
            }
        }

        public bool Build_UI()
        {   
            //搭界面时候先出蒙版把main遮住，显示各项加载情况。
            try
            {

                var main = GetObject<MainPage>("main");
                if (main == null)
                    return false;
                var frameWorkViewModel = GetObject<FrameWorkViewModel>("FrameWorkViewModel");
                if (frameWorkViewModel == null) return false;
                var uiconfig = GetObject<string>("uiconfig");
                if (uiconfig == null) return false;
                var document = XmlHelper.GetXDocument(uiconfig);
                frameWorkViewModel.InitiViewModel(document.Root,this);
                var title = XmlHelper.GetXElements(document.Root, "TextViewModel").FirstOrDefault(p => XmlHelper.GetValue(p, "Name").Equals("Title"));
                if (title != null) BindingHelper.Binding(ViewModelHelper.CreatViewModel(title), (FrameworkElement)main.FindName("Title"));
                var banner = XmlHelper.GetXElements(document.Root, "ImageViewModel").FirstOrDefault(p => XmlHelper.GetValue(p, "Name").Equals("Banner"));
                if (banner != null) BindingHelper.Binding(ViewModelHelper.CreatViewModel(banner), (FrameworkElement)main.FindName("Banner"));
                //锁住线程，避免对象初始化的时候顺序乱了Get不到东西
                lock (_lock)
                {
                    BuildTitleControls(document);//创建标题栏部分元素
                }
                lock (_lock)
                {
                    BuildContainer(document);//创建主界面容器元素
                }
                lock (_lock)
                {
                    BuildAccordion(document);//创建菜单
                }
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
           return false;
        }
    }

    /// <summary>
    /// 对资源轮询获取，超时返回空
    /// </summary>
    public class GetObjectProxy
    {
        public event EventHandler<GetObjectArgs> GetObjectCompleted;
        //异步获取对象
        public void GetObjectAsynch(string key, IAppFrame appFrame)
        {
            var assemblyLoader = appFrame.GetObject<AssemblyLoader>("AssemblyLoader");
            var o = appFrame.GetObject(key);
            if (o != null)
            {   assemblyLoader.SetMessage(string.Format("获取对象{0}成功！",key));
                if (GetObjectCompleted != null) GetObjectCompleted(this, new GetObjectArgs
                {
                    Result = o
                });
            }
            else
            {
                var sum = 0;
                var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50) };//50毫秒轮询判断value是否为空
                timer.Tick += (s1, e1) =>
                {
                    var res = appFrame.GetObject(key);
                    if (res != null)
                    {
                        assemblyLoader.SetMessage(string.Format("获取对象{0}成功！", key));
                        if (GetObjectCompleted != null) GetObjectCompleted(this, new GetObjectArgs
                        {
                            Result = res
                        });
                        timer.Stop();
                    }
                    else
                    {
                        sum += 50;
                        if (sum < appFrame.TimeOut) return;
                        timer.Stop();//超时未获取对象
                        assemblyLoader.SetMessage(string.Format("获取对象{0}失败！", key));
                        if (GetObjectCompleted != null)
                            GetObjectCompleted(this, new GetObjectArgs
                            {
                                Result = null
                            });
                    }
                };
                timer.Start();
            }
        }
    }

    public class GetObjectArgs: EventArgs
    {
        public object Result { get; set;}
       
    }

    public abstract class Command : ICommand
    {
        public IAppFrame Frame;
      
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;


        public virtual void OnLoaded()
        {

        }

        public  void Execute(object parameter)
        {
           OnClick();   
        }

        public virtual void OnClick()
        {

        }

        public virtual void OnCreate(IAppFrame appFrame)
        {
            Frame = appFrame;
        }
    }


    public abstract class FrameWorkControl:BaseFrameWorkControl 
    {
        public IAppFrame Frame;

        public virtual void OnCreate(IAppFrame appFrame)
        {
            Frame = appFrame;
        }


        public virtual void OnLoaded()
        {

        }
    }



}