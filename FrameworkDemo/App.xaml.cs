using System;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using BaseTool;
using BaseTool.Web;
using Silverlight.FrameWork;

namespace FrameworkDemo
{
    public partial class App
    {
        private AppFrame _frame;
        private readonly SynchronizationContext _syn;
        private int _count;
        public App()
        {
           _frame=new AppFrame();
            _syn = SynchronizationContext.Current;
            Startup += (s,e) =>
            {
                string[] configs = { "LoginConfig.xml", "appconfig.xml" ,"uiconfig.xml"};
                foreach (var config in configs)
                {
                    ThreadPool.QueueUserWorkItem(DownloadConfig, config);
                }
            };
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;
            InitializeComponent();
            var loader = new AssemblyLoader();
            _frame.AddObject("AssemblyLoader", loader);
            _frame.AddObject("host", Current.Host.Source.ToString()
                        .Substring(0, Current.Host.Source.ToString().IndexOf("ClientBin", StringComparison.Ordinal)));
        }


        private void DownloadConfig(object config)
        {
            _syn.Post(s =>
            {
               var url = HttpHelper.GetTimeStampUrl(_frame.GetObject<string>("host")+s);
                var httphelper = new HttpHelper();
                httphelper.Get(url, result =>
                {
                    if (!result.IsGetResponse) return;
                    var xdoc = XDocument.Parse(result.Result2String);
                    var name = xdoc.Root.Name.ToString();
                    _count++;
                    if (name.ToLower() == "appconfig")
                    {
                        var nodes = XmlHelper.GetXElements(xdoc.Root, "add");
                        foreach (var node in nodes)
                        {
                            _frame.AddObject(XmlHelper.GetValue(node, "key"), XmlHelper.GetValue(node, "value"));
                        }
                    }
                    else
                    {
                        _frame.AddObject(name, xdoc.ToString());
                    }
                    if (_count != 3) return;
                    RootVisual = new Login(_frame);
                });
            }, config);
        }


    private void Application_Exit(object sender, EventArgs e)
        {
           
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // 如果应用程序是在调试器外运行的，则使用浏览器的
            // 异常机制报告该异常。在 IE 上，将在状态栏中用一个 
            // 黄色警报图标来显示该异常，而 Firefox 则会显示一个脚本错误。
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // 注意:  这使应用程序可以在已引发异常但尚未处理该异常的情况下
                // 继续运行。
                // 对于生产应用程序，此错误处理应替换为向网站报告错误
                // 并停止应用程序。
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
