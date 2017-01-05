
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using BaseTool.Web;

namespace Silverlight.FrameWork.ViewModel
{
    public static class BindingHelper
    {
       public static  void Binding(object viewMolde,FrameworkElement targetElement)
       {
           targetElement.DataContext = viewMolde;
           var properties = viewMolde.GetType().GetProperties();
           var properties2 = targetElement.GetType().GetProperties();
           foreach (var property in properties)
           {
               var value = property.GetValue(viewMolde,null);
               if (value == null)
               {
                 if (property.Name!="Command") continue;
               }
               switch (property.Name)
               {
                   case "Row":
                       if (targetElement.Parent is Grid)
                           if (value != null) Grid.SetRow(targetElement, int.Parse(value.ToString()));
                       break;
                   case "RowSpan":
                       if (targetElement.Parent is Grid)
                           if (value != null) Grid.SetRowSpan(targetElement, int.Parse(value.ToString()));
                       break;
                   case "Column":
                       if (targetElement.Parent is Grid)
                           if (value != null) Grid.SetColumn(targetElement,int.Parse(value.ToString()));
                       break;
                   case "ColumnSpan":
                       if (targetElement.Parent is Grid)
                           if (value != null) Grid.SetColumnSpan(targetElement, int.Parse(value.ToString()));
                       break;
                   case  "Tip":
                       ToolTipService.SetToolTip(targetElement,value);
                       break;
                   case "Name":
                       break;
                   default :
                       var property2 = properties2.FirstOrDefault(p => p.Name.Equals(property.Name));
                       if (property2!=null)
                       {
                           if (!BindingPath(property.Name, viewMolde, targetElement))
                           {
                               var molde = viewMolde as ITextViewMolde;
                               if (molde != null && property.Name.Equals("FontSource")&&molde.FontFamily!=null)
                               {
                                 var httpHelper=new HttpHelper();
                                    httpHelper.Get(molde.FontSource, s =>
                                    {
                                        if (!s.IsGetResponse)
                                            return;
                                        var f = targetElement;
                                        property2.SetValue(f, new FontSource(s.GetResponseStream()), null);
                                      //被字体名称纠结一晚上....
                                    });   
                               }
                           }
                       }
                       break;
               }
           }
           targetElement.Loaded += (s, e) =>
           {
               //反射执行
               var methodInfo = targetElement.GetType().GetMethod("OnLoaded");
               if (methodInfo != null)
               {
                   methodInfo.Invoke(targetElement, null);
               }
               var commandproperty = properties.FirstOrDefault(p => p.Name.Equals("Command"));
               if (commandproperty == null) return;
               var value = commandproperty.GetValue(viewMolde, null);
               if (value != null)
               {
                   ((Command) value).OnLoaded();
                   return;
               }
               var timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 50)};//50毫秒轮询判断value是否为空
               timer.Tick += (s1, e1) =>
               {
                   value = commandproperty.GetValue(viewMolde, null);
                   if (value == null) return;
                   ((Command)value).OnLoaded();
                   timer.Stop();
               };
               timer.Start();
           };
       }

    

        public static DependencyProperty GetDependencyProperty(Type type, string name)
        {
            while (true)
            {
                var fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.Static);
                var dp = fieldInfo != null ? (DependencyProperty) fieldInfo.GetValue(null) : null;
                if (dp != null) return dp;
                if (type.BaseType == null) return null;
                type = type.BaseType;
            }
        }

        public static bool BindingPath(string path, object viewModel, FrameworkElement targetElement)
       {
           var dp = GetDependencyProperty(targetElement.GetType(), path + "Property");
           if (dp==null) return false;
           var binding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath(path),
                Mode = BindingMode.OneWay
            };
           targetElement.SetBinding(dp, binding);
           return true;
       }
    }
}
