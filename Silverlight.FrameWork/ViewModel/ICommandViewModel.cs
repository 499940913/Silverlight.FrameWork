namespace Silverlight.FrameWork.ViewModel
{
    public interface ICommandViewModel
    {
        Command Command { get; set;}

        string Content { get; set; }
    }
}
