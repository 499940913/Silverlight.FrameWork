namespace Module1
{
    public partial class Module1Control
    {
        public Module1Control()
        {
            InitializeComponent();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
            Frame.AddObject("miku",this);//将该控件以miku的键名加入到公共变量中，待会由Module1Command获取
        }
    }
}
