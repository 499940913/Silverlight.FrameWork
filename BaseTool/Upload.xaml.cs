using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Threading;
using BaseTool.Web;

// ReSharper disable NotAccessedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BaseTool
{
    public partial class Upload 
    {
  
        #region 变量
        private long _sumBlockSize;
        private readonly int _blockSzie = 0x2800;
        private string _serviceurl = "";
        private byte[] _buffer;
        private int _currentFileNum;
        public bool Enableclose = true;
      //  private readonly List<string> _failures = new List<string>();
        private readonly List<FilesClass> _files = new List<FilesClass>();
        private int _iFileCount = 1;
        private bool _issmallfile;
        public int MaxFileCount =10;
        private FilesClass _obj;
        private FileStream _stream;
        private readonly HttpHelper _httphelper;
        public bool AddTimeString;
        private string[] _matchfiles;
        private string[] _currentfiles;
        #endregion


        public string[] MatchFiles {
            internal get { return _matchfiles; }
            set { _matchfiles = value; }
        }

        public string[] CurrentFiles
        {
            internal get { return _currentfiles; }
            set
            {
                _currentfiles = value;
                if (_currentfiles != null && _currentfiles.Length > 0)
                {
                    var list = MatchFiles.ToList();
                    foreach (var key in value)
                    {
                        if (list.Contains(key))
                        {
                            list.Remove(key);
                        }
                    }
                    MatchFiles = list.ToArray();
                }
            }
        }

        public Upload()
        {
            InitializeComponent();
            _httphelper = new HttpHelper();
            FileFilter = "所有文件(*.*)|*.*";
        }

        public string TargetPath { get; set; }

        public string ServiceUrl
        {
            get
            {
                return _serviceurl;
            }
            set
            {
                _serviceurl = value;
            }
        }

        public string FileFilter { get; set; }


        private int BlockSize
        {
            get
            {
                return _blockSzie;
            }
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            var msg = new List<string>();
            var op = new OpenFileDialog
            {
                Filter = FileFilter,
                Multiselect = true
            };
            op.ShowDialog();
            if (op.Files != null)
            {
                if (MatchFiles != null && MatchFiles.Length > 0)
                {
                    msg = MatchFiles.ToList();
                }
                foreach (var f in op.Files)
                {
                    var obj = new FilesClass
                    {
                        PropFileName = f,
                        PropNumber = _iFileCount.ToString()
                    };
                    _files.Add(obj);
                    var b = new MyProgressBar
                    {
                        Name = f.Name,
                        Description = f.Name + "等待上传。"
                    };
                    PragressContainer.Children.Add(b);
                    var fname = f.Name.Substring(0, f.Name.LastIndexOf('.'));
                    if (msg.Contains(fname))
                    {
                        msg.Remove(fname);
                    }
                }
                btnUpload.IsEnabled = !(msg.Count > 0);
                if (msg.Count > 0)
                {
                    var str="";
                    for (var i = 0; i < msg.Count; i++)
                    {
                        if (i!=msg.Count-1)
                        {
                            str += msg[i]+",";
                        }
                        else
                        {
                            str += msg[i];
                        }
                    }
                    MessageBox.Show(string.Format("缺少要求文件：{0}。",str));
                }
                PragressContainer.UpdateLayout();
            }
            else
            {
                MessageBox.Show("没有选择任何文件！");
            }
        }


        public void Clear()
        {
            HasCloseButton = true;
            _sumBlockSize = 0;
            _files.Clear();
            _iFileCount = 1;
           PragressContainer.Children.Clear();
           PragressContainer.UpdateLayout();
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
            GC.Collect();
        }



        private void UpLoadStart(int fileNum)
        {
            try
            {
                if (fileNum == 0)
                {
                    if (string.IsNullOrEmpty(ServiceUrl) || string.IsNullOrEmpty(TargetPath) || string.IsNullOrEmpty(FileFilter) || _files.Count == 0 || Context == null)
                    {
                        HasCloseButton = true;
                        return;
                    }
                    _currentFileNum = 0;
                }
                if (_files.Count > 0 && _files.Count > fileNum)
                {
                    _obj = _files[fileNum];
                    _stream = _obj.PropFileName.OpenRead();
                    Reset();
                    _issmallfile = _stream.Length <= BlockSize;
                    UpLoadStep();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public SynchronizationContext Context
        {
            get;
            set;
        }

        private void UpLoadStep()
        {
            var str = _files[_currentFileNum].PropFileName.Name;
            var b = (MyProgressBar)PragressContainer.FindName(str);
            if (AddTimeString)
            {
                if (str.Length <=9||!JsonHelper.IsDate(str.Substring(0,8)))
                {
                    str = string.Format("{0}_" + str, DateTime.Now.ToString("yyyyMMdd"));
                }
            }
            if (b.Description != "正在上传:" + str + "...")
            {
                b.Description = "正在上传:" + str + "...";
            }
            try
            {
                if (_issmallfile)
                {
                    _buffer = new byte[_stream.Length - _sumBlockSize];
                    _stream.Read(_buffer, 0, (int)(_stream.Length - _sumBlockSize));
                    var uploaditem = new UploadItem{ directory = TargetPath, filename = str, append = false, overwrite = true, offset = 0, buffer = _buffer };
                    var postdata = JsonHelper.SerializerJson(uploaditem);
                    _httphelper.Post(ServiceUrl, postdata, MediaType.ApplicationJson, HeaderType.ApplicationJson,UploadCompleted);
                    _sumBlockSize += _buffer.Length;
                }
                else if (_stream.Length - _sumBlockSize > BlockSize)
                {
                    _buffer = new byte[BlockSize];
                    _stream.Seek(_sumBlockSize, 0);
                    _stream.Read(_buffer, 0, BlockSize);
                    var uploaditem = new UploadItem { directory = TargetPath, filename = str, append = true, overwrite = true, offset = _sumBlockSize, buffer = _buffer };
                    var postdata = JsonHelper.SerializerJson(uploaditem);
                    _httphelper.Post(ServiceUrl, postdata, MediaType.ApplicationJson, HeaderType.ApplicationJson, UploadCompleted);
                    _sumBlockSize += _buffer.Length;
                }
                else
                {
                    _buffer = new byte[_stream.Length - _sumBlockSize];
                    _stream.Read(_buffer, 0, (int)(_stream.Length - _sumBlockSize));
                    var uploaditem = new UploadItem { directory = TargetPath, filename = str, append = true, overwrite = true, offset = _sumBlockSize, buffer = _buffer };
                    var postdata = JsonHelper.SerializerJson(uploaditem);
                    _httphelper.Post(ServiceUrl, postdata, MediaType.ApplicationJson, HeaderType.ApplicationJson, UploadCompleted);
                    _sumBlockSize += _buffer.Length;
                }
            }
            catch
            {
                // ignored
            }
        }

        public class UploadItem
        {
            public string directory { get; set; }
            public string filename { get; set; }
            public bool append { get; set; }
            public bool overwrite { get; set; }

            public long offset { get; set; }
            public byte[] buffer { get; set; }

        }

        public string CurrentFileName
        {
            get;
            set;
        }

        private void UploadCompleted(Object o)
        {
            var e = (RequstResult)o;
           // if (this.currentFileNum >this.files.Count - 1) return;
            if (_files.Count == 0)
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                    _stream = null;
                }
                return;
            }
            var f = _files[_currentFileNum];
            var b = (MyProgressBar)PragressContainer.FindName(f.PropFileName.Name);
            if (e.Error != null)
            {
                f.PropStatus = "上传失败！";
                b.Description = "文件名为:" + f.PropFileName.Name + "上传失败!";
            }
            var obj = e.Result2String;
            if (obj != null)
            {
                var isok = Convert.ToBoolean(obj);
                if (!isok)
                {
                    f.PropStatus = "上传失败！";
                    b.Description = "文件名为:" + f.PropFileName.Name + "上传失败!";
                }
                b.ProgressBarValue = (int)(_sumBlockSize / (double)_stream.Length * 100.0);
                if (_sumBlockSize == _stream.Length)
                {
                    _stream.Flush();
                    _stream.Close();
                    _stream.Dispose();
                    if (f.PropStatus != "上传失败！")
                    {
                        CurrentFileName =TargetPath+f.PropFileName.Name;
                        f.PropStatus = "上传成功！";
                    }
                    var num = _files.TakeWhile(class2 => class2.PropStatus == "上传成功！").Count();
                    _currentFileNum = num;
                    _sumBlockSize = 0L;
                    if (_files.Count > _currentFileNum)
                    {
                        UpLoadStart(_currentFileNum);
                    }
                    else
                    {
                        int num2;
                        MessageBox.Show("上传完成");
                        HasCloseButton = true;
                        Reset();
                        for (num2 = 0; num2 <= _files.Count - 1; num2++)
                        {
                            if (_files[num2].PropStatus == "上传成功！")
                            {
                                _files.RemoveAt(num2);
                                num2--;
                            }
                            else
                            {
                                _files[num2].PropStatus = "";
                            }
                        }
                        if (_files.Count > 0)
                        {
                            var text = "";
                            for (num2 = 0; num2 <= _files.Count - 1; num2++)
                            {
                                if (text == "")
                                {
                                    text = "以下文件上传失败。点击确认继续上传，取消完成上传:\r\n" + _files[num2].PropFileName.Name;
                                }
                                else
                                {
                                    text = text + "\r\n" + _files[num2].PropFileName.Name;
                                }
                            }
                        }
                        Enableclose = true;
                        return;
                    }
                }
                UpLoadStep();
            }
        }

        private void Reset()
        {
            _sumBlockSize = 0;

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (_files.Count <= MaxFileCount)
            {
                UpLoadStart(0);
            }
            else
            {
                MessageBox.Show("上传限制数量为" + MaxFileCount + ",请清除重新选择需要上传的项目！");
            }
        }

        private void BaseChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clear();
        }
    }

    public class FilesClass
    {
        string _strStatus = "";
        string _strNo = "";

        public string PropNumber
        {
            get
            {
                return _strNo;
            }
            set
            {
                _strNo = value;
            }
        }

        public FileInfo PropFileName { get; set; }

        public string PropStatus
        {
            get
            {
                return _strStatus;
            }
            set
            {
                _strStatus = value;
            }
        }
    }
}

