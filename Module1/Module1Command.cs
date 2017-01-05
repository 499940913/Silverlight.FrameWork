using System.Windows;
using Silverlight.FrameWork;

namespace Module1
{
    /// <summary>
    /// 模块1中的命令，执行在onclick中
    /// </summary>
    public class Module1Command : Command
    {
        private Module1Control miku;
        public Module1Command()
        {

        }

        /// <summary>
        /// mvvm模式下，宿主元素创建完毕自动触发以下代码
        /// </summary>
        public override void OnLoaded()
        {
            base.OnLoaded();
            GetObjectProxy getObjectProxy = new GetObjectProxy();
            getObjectProxy.GetObjectCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    miku = (Module1Control)e.Result;
                }
            };
            getObjectProxy.GetObjectAsynch("miku", Frame);
        }

        private bool _show = true;

        /// <summary>
        /// 宿主点击后自动触发
        /// </summary>
        public override void OnClick()
        {
            base.OnClick();
            if (miku == null) return;
            miku.Visibility = _show ? Visibility.Collapsed : Visibility.Visible;
            _show = !_show;
        }
    }
}
