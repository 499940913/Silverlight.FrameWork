using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;
using BaseTool.Web;

namespace Silverlight.FrameWork.ViewModel
{
    public static class AssemblyHelper
    {

        public static Assembly GetAssembly(string assemblyname)
        {
              assemblyname = assemblyname.Substring(0,
                 assemblyname.LastIndexOf('.'));
             return   AppDomain.CurrentDomain.GetAssemblies()
                   .FirstOrDefault(p => p.FullName.Split(',')[0].ToLower().Equals(assemblyname.ToLower()));
        }

        public static void CreateCommandFromViewModel(IAssemblyViewModel assemblyViewModel,
            ICommandViewModel commandViewModel, IAppFrame frame)
        {
            var assemblyLoader = frame.GetObject<AssemblyLoader>("AssemblyLoader");
           // assemblyLoader.SetVisible(true);
            var assemblyProxy=new AssemblyProxy();
            assemblyLoader.SetMessage(string.Format("正在获取引用{0}...",assemblyViewModel.AssemblyName));
            assemblyProxy.GetAssemblyCompleted += (s, e) =>
                {
                    var assembly = e.Assembly;
                    if (assembly == null) return;
                    assemblyLoader.SetMessage(string.Format("引用{0}获取成功！开始创建{1}实例", assemblyViewModel.AssemblyName,assemblyViewModel.ClassName));
                    commandViewModel.Command= (Command)assembly.CreateInstance(assemblyViewModel.ClassName);
                    if (commandViewModel.Command!=null)
                    {
                        commandViewModel.Command.OnCreate(frame);
                    }
                  //  assemblyLoader.SetVisible(false);
                }
             ;
            assemblyProxy.GetAssembly(assemblyViewModel, frame);
        }
    }

    
    //获取引用的代理类
    public class AssemblyProxy
    {
        private IAppFrame _frame;

        private string _version;

        public event EventHandler<GetAssemblyEventArgs> GetAssemblyCompleted;

        public void GetAssembly(IAssemblyViewModel assemblyViewModel, IAppFrame frame)
        {
            _frame = frame;
            _version = "1.0";//获取引用或包体的版本，在版本号未发生变化的情况下使用缓存避免频繁请求服务器资源增加负担
            if (_frame.GetObject<string>("version") != null) _version = _frame.GetObject<string>("version");
            var assemblyLoader = frame.GetObject<AssemblyLoader>("AssemblyLoader");
           // assemblyLoader.SetVisible(true);
            var assembly = AssemblyHelper.GetAssembly(assemblyViewModel.AssemblyName);
            if (assembly != null)
            {
                if (GetAssemblyCompleted!=null)
                {
                    GetAssemblyCompleted(this,new GetAssemblyEventArgs(assembly));
                }
            }
            else
            {
                //去服务器端下载引用
                var resroot = frame.GetObject<string>("AssemblyRoot");
                var httphelper = new HttpHelper();
                if (!string.IsNullOrEmpty(assemblyViewModel.PackageName))
                {
                    //对同一个资源下载两遍有点傻啊
                    if (frame.GetObject<MemoryStream>(assemblyViewModel.PackageName) == null)
                    {
                        //好像太快了看不到加载过程
                        assemblyLoader.SetMessage(string.Format("正在下载{0}包体...", assemblyViewModel.PackageName));
                        httphelper.Get(resroot + assemblyViewModel.PackageName + string.Format("?v={0}",_version), "application/x-silverlight-app", s =>
                        {
                            if (!s.IsGetResponse) return;
                            assemblyLoader.SetMessage(string.Format("{0}包体下载成功，开始创建{1}引用。", assemblyViewModel.PackageName, assemblyViewModel.AssemblyName));
                            var stream = s.GetResponseStream();
                            frame.AddObject(assemblyViewModel.PackageName, stream);
                            assembly = GetAssemblyFromPackage(assemblyViewModel, _frame);
                            if (GetAssemblyCompleted != null)
                            {
                                GetAssemblyCompleted(this, new GetAssemblyEventArgs(assembly));
                            }
                        });
                    }
                    else
                    {
                        assemblyLoader.SetMessage(string.Format("{0}包体下载成功，开始创建{1}引用。", assemblyViewModel.PackageName, assemblyViewModel.AssemblyName));
                        assembly = GetAssemblyFromPackage(assemblyViewModel, _frame);
                        if (GetAssemblyCompleted != null)
                        {
                            GetAssemblyCompleted(this, new GetAssemblyEventArgs(assembly));
                        }
                    }
                }
                else
                {
                    assemblyLoader.SetMessage(string.Format("正在下载{0}引用...", assemblyViewModel.AssemblyName));
                    httphelper.Get(resroot + assemblyViewModel.AssemblyName + string.Format("?v={0}", _version), "application/x-msdownload", s =>
                    {
                        if (!s.IsGetResponse) return;
                        using (var ms = s.GetResponseStream())
                        {
                            var assemblyPart = new AssemblyPart();
                            assembly = assemblyPart.Load(ms);
                            if (GetAssemblyCompleted != null)
                            {
                                GetAssemblyCompleted(this, new GetAssemblyEventArgs(assembly));
                            }
                            ms.Close();
                        }
                    });
                }
            }
        }

        private static Assembly GetAssemblyFromPackage(IAssemblyViewModel assemblyViewModel, IAppFrame frame)
        {
            var stream = frame.GetObject<MemoryStream>(assemblyViewModel.PackageName);
            Assembly assembly = null;
            var sri = new StreamResourceInfo(stream, "application/binary");
            using (var manifestStream =
               Application.GetResourceStream(sri, new Uri("AppManifest.xaml", UriKind.Relative)).Stream)
            {
                var appManifest = new StreamReader(manifestStream).ReadToEnd();
                var deploymentRoot = XDocument.Parse(appManifest).Root;
                var deploymentParts = deploymentRoot.Elements().Elements().ToList();
                foreach (var xElement in deploymentParts)
                {
                    var source = xElement.Attribute("Source").Value;
                    var streamInfo =
                        Application.GetResourceStream(new StreamResourceInfo(stream, "application/binary"),
                            new Uri(source, UriKind.Relative));
                    var asmPart = new AssemblyPart { Source = source };
                    if (source == assemblyViewModel.AssemblyName)
                    {
                        assembly = asmPart.Load(streamInfo.Stream);
                    }
                    else
                    {
                        var existAssembly = AssemblyHelper.GetAssembly(source);
                        if (existAssembly == null)
                        {
                            asmPart.Load(streamInfo.Stream);
                        }
                    }
                }
                manifestStream.Close();
            }
            return assembly;
        }


    }

    //下载或获取引用完毕后的事件参数
    public class GetAssemblyEventArgs:EventArgs
    {
        public GetAssemblyEventArgs(Assembly assembly)
        {
            Assembly = assembly;
        }
        public Assembly Assembly { get; set; }
    }
}
