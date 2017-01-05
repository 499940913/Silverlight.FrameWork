using System.Xml.Linq;
using BaseTool;

namespace Silverlight.FrameWork.ViewModel
{
    public static class ViewModelHelper
    {
        public  static T CreateViewModel<T>(XElement element) where T:IViewModel ,new()
        {
            var viewModel = new T();
            var properties = viewModel.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = XmlHelper.GetValue(element, property.Name);
                if (string.IsNullOrEmpty(value)) continue;
                property.SetValue(viewModel,JsonHelper.ChangeType2(value,property.PropertyType), null);
            }
            return viewModel;
        }

        public static void ChangeViewModel<T>(XElement element,T t) where T : IViewModel, new()
        {
            var properties = t.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = XmlHelper.GetValue(element, property.Name);
                if (string.IsNullOrEmpty(value)) continue;
                property.SetValue(t, JsonHelper.ChangeType2(value, property.PropertyType), null);
            }
        }

        public static object CreatViewModel(XElement element)
        {
            switch (element.Name.ToString())
            {
                case "ImageViewModel":
                    return CreateViewModel<ImageViewModel>(element);
                case "TextViewModel":
                    return CreateViewModel<TextViewModel>(element);
                case "CommandViewModel":
                    return CreateViewModel<CommandViewModel>(element);
                case "AccordionItemViewModel":
                    return CreateViewModel<AccordionItemViewModel>(element);
                case "AccordionCommandViewModel":
                    return CreateViewModel<AccordionCommandViewModel>(element);
                case "FrameWorkViewModel":
                    return CreateViewModel<FrameWorkViewModel>(element);
                default:
                    return null;
            }
          
        }
    }
}
