
namespace Silverlight.FrameWork.ViewModel
{
    /// <summary>
    ///界面元素的Viewmodel
    /// </summary>
    public interface IFrameworkElementViewModel
    {
        string Visibility { get; set;}

        bool IsEnabled { get; set;}

        string Opacity { get; set;}

        string Width { get; set;}

        string Height { get; set;}

        string Margin { get; set;}

        string VerticalAlignment { get; set; }
        string HorizontalAlignment { get; set; }

        int? Row { get; set;}

        int? Column { get; set;}

        int? RowSpan { get; set; }

        int? ColumnSpan { get; set; }

        string Tip { get; set; }

        
    }
}
