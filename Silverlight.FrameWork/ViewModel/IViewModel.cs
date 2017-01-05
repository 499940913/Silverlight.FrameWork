using System.Xml.Linq;

namespace Silverlight.FrameWork.ViewModel
{
    public interface IViewModel
    {
        void InitiViewModel(XElement element, IAppFrame mFrame);
        
        string Name { get; set; }
    }

}
