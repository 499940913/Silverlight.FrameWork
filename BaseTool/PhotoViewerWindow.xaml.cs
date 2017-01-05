using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;

namespace BaseTool
{
    public partial class PhotoViewerWindow
    {
        public PhotoViewerWindow()
        {
            InitializeComponent();
            Loaded += PhotoViewerWindow_Loaded;
        }

        void PhotoViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_byStream)
            {
                SetPhoto(_photoUrl);
            }
            else
            {
                SetPhoto(_photosStream);
            }
        }

        private void SetPhoto(string urls)
        {
            ClearUp();
            _uris = urls.Split(';');
            if (_uris != null && _uris.Length > 0)
            {
                PhotoContainer.MouseLeave += PhotoContainer_MouseLeave;
                for (var i = 0; i <= _uris.Length - 1; i++)
                {
                    var mName = _uris[i].Substring(_uris[i].LastIndexOf('/') + 1, _uris[i].LastIndexOf('.') - _uris[i].LastIndexOf('/') - 1);
                    var imagebutton = new ImageButton { Source = new BitmapImage(new Uri(_uris[i])), ImageName = mName };
                    PhotoContainer.Children.Add(imagebutton);
                    if (i == 0)
                    {
                        MainPhoto.Source = imagebutton.Source;
                    }
                    imagebutton.MouseEnter += imagebutton_MouseEnter;
                    imagebutton.SelectedChanged += imagebutton_SelectedChanged;
                }
            }
            _uris = null;
            _photoUrl = null;
            GC.Collect();
        }


        private void SetPhoto(Dictionary<string, string> photos)
        {
            ClearUp();
            if (photos.Count > 0)
            {
                PhotoContainer.MouseLeave += PhotoContainer_MouseLeave;
                Stream ms = null;
                try
                {
                    var inti = true;
                    foreach (var key in photos.Keys)
                    {
                        var byteArray = Convert.FromBase64String(photos[key]);
                        ms = new MemoryStream(byteArray);
                        var bitmap = new BitmapImage();
                        bitmap.SetSource(ms);
                        var imagebutton = new ImageButton { Source = bitmap, ImageName = key };
                        PhotoContainer.Children.Add(imagebutton);
                        if (inti)
                        {
                            inti = false;
                            MainPhoto.Source = ((ImageButton)PhotoContainer.Children.ElementAt(0)).Source;
                        }
                        imagebutton.MouseEnter += imagebutton_MouseEnter;
                        imagebutton.SelectedChanged += imagebutton_SelectedChanged;
                        ms.Close();
                        ms.Dispose();
                        ms = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    photos.Clear();
                    if (_photosStream != null)
                    {
                        _photosStream.Clear();
                        _photosStream = null; 
                    }
                    if (ms != null)
                    {
                        ms.Close();
                        ms.Dispose();
                    }
                    GC.Collect();
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_photosStream != null)
            {
                _photosStream.Clear();
                _photosStream = null;
            }
            _photoUrl = null;
            ClearUp();
            Loaded -= PhotoViewerWindow_Loaded;
            base.OnClosing(e);
        }

        string[] _uris;

        private bool _byStream;

        private string _photoUrl;

        public void GetPhoto(string urls)
        {
            _byStream = false;
            _photoUrl = urls;
        }

        private Dictionary<string, string> _photosStream;
        public void GetPhoto(Dictionary<string, string> photos)
        {
            _byStream = true;
            _photosStream = photos;
        }

        void imagebutton_SelectedChanged(object sender, SelectedChangedEventArgs e)
        {
            var btn = (ImageButton)sender;
            PhotoNameText.Text = btn.ImageName;
            PhotoPreview.Source = btn.Source;
            MainPhoto.Source = btn.Source;
        }

        void imagebutton_MouseEnter(object sender, MouseEventArgs e)
        {
            PreviewPhotoVisible.Stop();
            var btn = (ImageButton)sender;
            PhotoPreview.Source = btn.Source;
            PreviewPhotoVisible.Begin();
            var generalTransform = btn.TransformToVisual(LayoutRoot);
            var point = generalTransform.Transform(new Point());
            var halfwidth = btn.ActualWidth / 2;

            var halfcontainerwidth = PreviewScroll.ActualWidth / 2;
             PreviewScroll.TransformToVisual(LayoutRoot);
            var point2 = generalTransform.Transform(new Point());
            if (halfwidth + point.X < halfcontainerwidth + point2.X)
            {
                preposition.X = halfwidth + point.X - PhotoPreviewGrid.ActualWidth / 2;
            }
            else
            {
                preposition.X = halfcontainerwidth + point2.X - PhotoPreviewGrid.ActualWidth / 2;
            }
        }

        void PhotoContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            PhotoPreviewGrid.Opacity = 0;
        }

        public void ClearUp()
        {
            OffsetScroll(0);
            PhotoContainer.MouseLeave -= PhotoContainer_MouseLeave;
            var buttons = from child in PhotoContainer.Children where child is ImageButton select (ImageButton)child;
            try
            {
                while (buttons.Count() != 0)
                {
                    var btn = buttons.ElementAt(buttons.Count() - 1);
                    btn.MouseEnter -= imagebutton_MouseEnter;
                    btn.SelectedChanged -= imagebutton_SelectedChanged;
                    btn.ClearUp();
                    PhotoContainer.Children.Remove(btn);
                    btn = null;
                }
                PhotoContainer.Children.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PhotoPreview.Source = null;
            MainPhoto.Source = null;
            PhotoPreviewGrid.Opacity = 0;
            GC.Collect();
        }

        private void MainPhoto_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                PhotoMouseLeave.Stop();
                PhotoMouseOver.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainPhoto_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                PhotoMouseOver.Stop();
                PhotoMouseLeave.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            //往前翻页
            var scrolwidth = PreviewScroll.ActualWidth - 8;
            if (PhotoContainer.ActualWidth <= scrolwidth) return;
            var generalTransform = PhotoContainer.TransformToVisual(PreviewScroll);
            var point = generalTransform.Transform(new Point());
            if (PhotoContainer.ActualWidth + point.X <= scrolwidth) return;
            OffsetScroll(scrolwidth - point.X);
        }

        private void BackwardsButton_Click(object sender, RoutedEventArgs e)
        {
            //往后翻页
            var scrolwidth = PreviewScroll.ActualWidth - 8;
            if (PhotoContainer.ActualWidth <= scrolwidth) return;
            var generalTransform = PhotoContainer.TransformToVisual(PreviewScroll);
            var point = generalTransform.Transform(new Point());
            if (point.X == 4) return;
            OffsetScroll(-point.X - scrolwidth);
        }


        private void OffsetScroll(double offset)
        {
            PreviewScroll.ScrollToHorizontalOffset(offset);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClearUp();
            Close();
        }
    }
}

