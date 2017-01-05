using System;
using System.Collections.Generic;
using System.Windows;
using Liquid;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Browser;
using BaseTool.Web;

namespace BaseTool
{
    public partial class DataManger
    {

        #region
        private string _autoSelectNodeId = string.Empty;
        //private readonly IDictionary<string, int> _myparams = new Dictionary<string, int>();
        private readonly string _checkFolderUrl;
        private readonly string _deleteFileUrl;
        private readonly string _downFileUrl;
        private readonly string _baseServiceAddress;
        private readonly HttpHelper _httphelper;
        private readonly string _targetPath;
        private readonly string _uploadUrl;
        private readonly string _getFilesInFolderUrl; 
        private Node _currentSelectedNode;
        private readonly bool _addTimeString;
        #endregion

        private readonly Dictionary<string, string> _mTitlemaps;
        private Dictionary<string, string> _filtermaps; 
        public DataManger(string checkurl,string deletefileurl, string downloadurl,string path,string uploadurl,string getfiles,Dictionary<string,string>titleMaps ,string baseService,bool addtimestring)
        {
            InitializeComponent();
            _mTitlemaps = titleMaps;
            _httphelper = new HttpHelper();
            _checkFolderUrl = checkurl;
            _deleteFileUrl = deletefileurl;
            _targetPath = path;
            _uploadUrl = uploadurl;
            _downFileUrl = downloadurl;
            _getFilesInFolderUrl = getfiles;
            _baseServiceAddress = baseService;
            _addTimeString = addtimestring;
            DialogResult = false;
        }

        private void PopulateChildrenJson(Node workingNode, string json)
        {
            var list = new List<ItemViewerItem>();
            workingNode.BulkUpdateBegin();
            workingNode.Nodes.Clear();
            items.Clear();
            var js = new DataContractJsonSerializer(typeof(List<UploaderFileNode>));
            var arrb = Encoding.UTF8.GetBytes(json);
            List<UploaderFileNode> clFileNodes;
            using (var stream = new MemoryStream(arrb))
            {
                clFileNodes = js.ReadObject(stream) as List<UploaderFileNode>;
                stream.Close();
            }
            if (clFileNodes != null)
                foreach (var filenode in clFileNodes)
                {
                    var num = filenode.Param;
                    var filename = filenode.ID;
                    var node = new Node();
                    var item = new FileItem
                    {
                        Text = GetTitle(filename),
                        LiquidTag = filename
                    };
                    node.ID = filename;
                    node.Title = GetTitle(filename);
                    node.Tag = filenode;
                    if (filename.EndsWith("/"))
                    {
                        node.HasChildren = num > 0;
                        node.Icon = "/BaseTool;component/images/folder.png";
                        node.IconExpanded = "/BaseTool;component/images/folderOpen.png";
                        node.IsContainer = true;
                        item.IconSource = new BitmapImage(new Uri("/BaseTool;component/images/large/folder.png", 0));
                        list.Add(item);
                    }
                    else
                    {
                        node.HasChildren = false;
                        node.Icon = "/BaseTool;component/images/" + GetIcon(filename);
                        node.IconExpanded = "/BaseTool;component/images/" + GetIcon(filename);
                        item.IconSource = new BitmapImage(new Uri("/BaseTool;component/images/large/" + GetIcon(filename), 0));
                        item.OtherText = Math.Round(num / 1024.0, 2) + "KB";
                        list.Add(item);
                        node.IsContainer = false;
                    }
                    workingNode.Nodes.Add(node);
                }
            items.Add(list);
            workingNode.BulkUpdateEnd();
            workingNode.IsBusy = false;
        }

        private void Items_DoubleClick(object sender, EventArgs e)
        {
            _autoSelectNodeId = items.Selected.LiquidTag.ToString();
            if (testTree.Selected != null)
            {
                Node n;
                if (testTree.Selected.ID != _autoSelectNodeId)
                {
                    n = testTree.Get(_autoSelectNodeId);
                    testTree.SetSelected(n);
                    if (!testTree.Selected.ParentNode.IsExpanded)
                    {
                        testTree.Selected.ParentNode.Expand();
                    }
                }
                else
                {
                    if (testTree.Selected.IsExpanded)
                    {
                        n = testTree.Get(_autoSelectNodeId);
                        testTree.SetSelected(n);
                    }
                    else
                    {
                        testTree.Selected.Expand();
                    }
                }
            }
        }

        private string GetTitle(string filename)
        {
            var split = filename.TrimEnd('/').Split('/');
            var title = filename;
            if (split.Length > 0)
            {
                title = split[split.Length - 1];
            }
            if (_mTitlemaps.Keys.Contains(title)) return _mTitlemaps[title];
            return title;
        }

        private string GetIcon(string filename)
        {
            var split = filename.Split('.');
            var extension = string.Empty;

            if (split.Length > 0)
            {
                extension = split[split.Length - 1].ToLower();
            }

            if (extension != "pdf" && extension != "xls" && extension != "doc" && extension != "gif" && extension != "mp3" &&
                extension != "ascx" && extension != "asmx" && extension != "aspx" && extension != "avi" && extension != "config" &&
                extension != "cs" && extension != "css" && extension != "htm" && extension != "html" && extension != "jpg" &&
                extension != "js" && extension != "mp4" && extension != "png" && extension != "txt" && extension != "xaml" &&
                extension != "xml" && extension != "zip")
            {
                extension = "unknown";
            }

            return extension + ".png";
        }

        private void BaseChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _httphelper.Post(_checkFolderUrl, JsonHelper.SerializerJson(_targetPath), MediaType.ApplicationJson, HeaderType.ApplicationJson,CheckFolderCompleted);
        }

        void CheckFolderCompleted(Object o)
        {
            var e = (RequstResult)o;
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show("读取文件夹失败，原因:" + e.Error.Message);
                Close();
            }
            else
            {
                _filtermaps = JsonHelper.GetDictionary(e.Result2String);
                var isok = Convert.ToBoolean(_filtermaps["status"]);
                _filtermaps.Remove("status");
                  if (!isok)
                {
                    System.Windows.MessageBox.Show("读取文件夹失败，窗口将关闭！");
                    Close();
                }
                else
                {
                    testTree.BuildRoot();
                    var node = new Node {ID = _targetPath + "/"};
                    node.Title = GetTitle(node.ID);
                    node.Icon = "/BaseTool;component/images/folder.png";
                    node.HasChildren =false;
                    testTree.Nodes.Add(node);
                }
            }
         
        }

        private void RestPop(Node node)
        {
            if (_currentSelectedNode != null)
            {
                try
                {
                    var isDir = node.ID.EndsWith("/");
                    if (_filtermaps.Count > 0 && !_filtermaps.ContainsKey(node.Title))
                    {
                        startUpload.IsEnabled = false;
                    }
                    else
                    {
                        startUpload.IsEnabled = isDir;
                    }
                    if (isDir)
                    {
                        Downloadfile.IsEnabled = false;
                        //_myparams.Clear();
                        BeginGetChildren(node);
                        ViewOnline.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Downloadfile.IsEnabled = true;
                        ViewOnline.Visibility = node.ID.EndsWith(".pdf") ? Visibility.Visible : Visibility.Collapsed;
                    }
                    deleteFile.IsEnabled = Downloadfile.IsEnabled;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }


        private void testTree_SelectionChange(object sender, TreeEventArgs e)
        {
            if (e.Target != null)
            {
                _currentSelectedNode = e.Target;
                RestPop(_currentSelectedNode);
            }
        }

        private void BeginGetChildren(Node node)
        {
            _httphelper.Post(_getFilesInFolderUrl, JsonHelper.SerializerJson(node.ID), MediaType.ApplicationJson, HeaderType.ApplicationJson,GerChildCompleted);
        }

        void GerChildCompleted(Object o)
        { 
            var e = (RequstResult)o;
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show("子节点失败，原因:" + e.Error.Message);
            }
            var obj = e.Result2String;
            if (obj != null)
            {
                PopulateChildrenJson(_currentSelectedNode, obj);
            }
        }

        private void startUpload_Click(object sender, RoutedEventArgs e)
        {

           
            var load = new Upload {TargetPath = _currentSelectedNode.ID,AddTimeString = _addTimeString};
            var filenode = (UploaderFileNode)_currentSelectedNode.Tag;
            if (filenode!=null&&filenode.MatchFiles != null && filenode.MatchFiles.Length > 0)
            {
                load.MatchFiles = filenode.MatchFiles;
                if (_currentSelectedNode.Nodes.Any(p => p.Title.IndexOf('.')>0))
                {
                   var currentfiles =  _currentSelectedNode.Nodes.Where(p=>p.Title.IndexOf('.') > 0).Select(p=>p.Title.Substring(0,p.Title.LastIndexOf('.'))).ToArray();
                    load.CurrentFiles = currentfiles;
                }
            }
            load.Closed += load_Closed;
            load.Title = "文件上传";
            load.ServiceUrl = _uploadUrl;
            if (_filtermaps.Count == 0)
            {
                if (!string.IsNullOrEmpty(FileFilter))
                {
                    load.FileFilter = FileFilter;
                }
            }
            else
            {
                if (_filtermaps.ContainsKey(_currentSelectedNode.Title))
                {
                    load.FileFilter = _filtermaps[_currentSelectedNode.Title];
                }
            }
            load.Show();
        }

        void load_Closed(object sender, EventArgs e)
        {
            RestPop(_currentSelectedNode);
            ((Upload)sender).Closed -= load_Closed;
        }

        private void deleteFile_Click(object sender, RoutedEventArgs e)
        {
            _httphelper.Post(_deleteFileUrl, JsonHelper.SerializerJson(_currentSelectedNode.ID), MediaType.ApplicationJson, HeaderType.ApplicationJson, DeletedCompleted);
        }


        public string FileFilter
        {
            get;
            set;
        }

        void DeletedCompleted(Object o)
        {
            var e = (RequstResult)o;
            if (e.Error != null)
            {
                System.Windows.MessageBox.Show("删除失败，原因:" + e.Error.Message);
            }
            var isok = Convert.ToBoolean(e.Result2String);
            System.Windows.MessageBox.Show(!isok ? "删除失败" : "删除成功");
            var pnode = _currentSelectedNode.ParentNode;
            RestPop(pnode);
            testTree.Selected = pnode;
        }

        private void Downloadfile_Click(object sender, RoutedEventArgs e)
        {
            var fname = _currentSelectedNode.ID;
            fname = HttpUtility.UrlEncode(fname);
            var uri = new Uri(_downFileUrl+ fname);
            HtmlPage.Window.Navigate(uri,"_blank");
        }

        private void ViewOnline_Click(object sender, RoutedEventArgs e)
        {
            var fname = _currentSelectedNode.ID;
            fname = HttpUtility.UrlEncode(fname);
            var url = _baseServiceAddress + string.Format("base64/getfile?filename={0}", fname);
            HtmlPage.Window.Invoke("OpenPDF2", url);
        }

    }

    public class UploaderFileNode
    {
        #region Private Properties

        private string _id = string.Empty;
        private int _param;

        #endregion

        #region Public Properties

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Param
        {
            get { return _param; }
            set { _param = value; }
        }

        public bool HasChildren
        {
            get { return _param > 0; }
        }

        public string[] MatchFiles { get; set; }

        #endregion

        #region Constructor

        public UploaderFileNode()
        {
        }

        public UploaderFileNode(string id, int param, bool isroot)
        {
            _id = id;
            _param = param;
            IsRoot = isroot;
        }
        public bool IsRoot
        {
            get;
            set;
        }

        #endregion
    }
}

