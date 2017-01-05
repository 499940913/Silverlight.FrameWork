using System.Windows;
using Silverlight.FrameWork;

namespace Module1
{
    public class LoginCommand:Command
    {
        public override void OnLoaded()
        {
            base.OnLoaded();
            MessageBox.Show("登录按钮已经创建且进行绑定了哦");
        }

        public override void OnClick()
        {
            base.OnClick();
            MessageBox.Show("你点了登录按钮。");
            Login login = Frame.GetObject<Login>("login");  //点击获取登录页
            login.Content=new MainPage((AppFrame)Frame);
        }
    }
}
