using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BaseTool
{
    public partial class Camera
    {

        private bool _videoIsPlaying;
        private CaptureSource _capSrc = new CaptureSource();
        private VideoBrush _myVideoBrush = new VideoBrush();
        private WriteableBitmap _bitmap;
         VideoCaptureDevice _video;
        public Camera()
        {
            InitializeComponent();
            StartCamera.Unchecked += StartCamera_Unchecked;
            StartCamera.Checked += StartCamera_Checked;
            SizeChanged += Camera_SizeChanged;
        }

        void StartCamera_Checked(object sender, RoutedEventArgs e)
        {
            if (!_videoIsPlaying)
            {
                _videoIsPlaying = true;
                _video = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                if (_video == null)
                {
                    MessageBox.Show("未发现摄像头！");
                    StartCamera.IsChecked = false;
                    return;
                }
                _capSrc.VideoCaptureDevice = _video;
                if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
                {
                    _myVideoBrush.SetSource(_capSrc);
                    CameraCanvas.Fill = _myVideoBrush;
                    _capSrc.Start();
                }
            }
        }

        private void StopVideo()
        {
            if (_videoIsPlaying)
            {
                _videoIsPlaying = false;
                _capSrc.Stop();
                _videoIsPlaying = false;
                if (_video != null)
                {
                    _video = null;
                }
                GC.Collect();
            }
        }

        void StartCamera_Unchecked(object sender, RoutedEventArgs e)
        {
            StopVideo();
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            StartCamera.Unchecked -= StartCamera_Unchecked;
            StartCamera.Checked -= StartCamera_Checked;
            SizeChanged -= Camera_SizeChanged;
            if (_videoIsPlaying)
            {
                StopVideo();
                _capSrc = null;
                _myVideoBrush = null;
                if (_bitmap != null)
                {
                    _bitmap = null;
                }
                GC.Collect();

            }
            base.OnClosing(e);
        }

       private  void Camera_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var  width= ActualHeight/3*2;
            if(ActualWidth != width)
            {
                Layout.Width = width;
                UpdateLayout();
            }
        }

     
    }
}

