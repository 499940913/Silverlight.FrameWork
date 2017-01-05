
using System.ComponentModel;

namespace Silverlight.FrameWork.ViewModel
{ 
    /// <summary>
    /// 从引用中创建的Viewmodel
    /// </summary>
    public interface IAssemblyViewModel
    {
        string ClassName { get; set;}

        string AssemblyName { get; set;}

        [Description("Xap包名")]
        string PackageName { get; set; }

        int? UiIndex { get; set; }

    }
}
