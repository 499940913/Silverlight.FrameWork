using System;
using System.Net;
using System.Threading;
using System.IO;
using System.Windows.Browser;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace BaseTool.Web
{


    public class RequstResult
    {
        private byte[] _result;

        public byte[] Result
        {
            get { return _result; }
            set
            {
                _result = value;
                if (_result != null)
                {
                    _getResponse = true;
                }
            }
        }

        public Stream GetResponseStream()
        {
            return !IsGetResponse ? null : new MemoryStream(_result);
        }

        private string _resultstring;

        public string Result2String
        {
            get
            {
                if (!IsGetResponse) return null;
                if (_resultstring != null) return _resultstring;
                using (var ms = GetResponseStream())
                {
                    using (var reader = new StreamReader(ms))
                    {
                        _resultstring =reader.ReadToEnd();
                        reader.Close();
                    }
                    ms.Close();
                }
                return _resultstring;
            }
        }

        public Exception Error { get; set; }

        private bool _getResponse;

        public bool IsGetResponse
        {
            get { return _getResponse; }
        }


    }

    public delegate void RequestCallback(RequstResult e);

    public class HttpHelper
    {
        private readonly SynchronizationContext _syn;
        private string _postData;
        private SendOrPostCallback _sendOrPostCallback;
        private RequestCallback _Callback;

        private object _sender;

        public object Sender
        {
            set { _sender = value;}
        }

        public HttpHelper()
        {
            _syn = SynchronizationContext.Current;
        }

        public class RequestErro
        {
            public string exception { get; set; }
            public string innerexception { get; set; }
        }

        public  void Post(string url, string data, string mediaType, string headertype, RequestCallback callBack)
        {
            url = GetTimeStampUrl(url);
            var endpoint = new Uri(url);
            _sendOrPostCallback = SendOrPostCallback;
            _Callback = callBack;
            var request = (HttpWebRequest) WebRequest.Create(endpoint);
            request.Method = "POST";
            request.ContentType = mediaType;
            request.Accept = headertype;
            _postData = data;
            request.BeginGetRequestStream(RequestReadySocket, request);
        }

        private void SendOrPostCallback(object state)
        {
            _Callback((RequstResult) state);
        }

        public void Get(string url, RequestCallback callBack)
        {
            Get(url,null, callBack);
        }


        public void Get(string url, string headertype,RequestCallback callBack)
        {
            var endpoint = new Uri(url);
            _sendOrPostCallback = SendOrPostCallback;
            _Callback = callBack;
            var request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = "GET";
            //360 Get请求有问题这里做处理
            var productname = HtmlPage.BrowserInformation.ProductName;
            if (string.IsNullOrEmpty(productname)||(!productname.ToLower().Equals("chrome")&&!productname.ToLower().Equals("chromium"))&&!string.IsNullOrEmpty(headertype))
            {
                request.Accept = headertype;
            }
            request.BeginGetResponse(ResponseReadySocket, request);
        }


        private void GetResponse(Stream responseStream)
        {
            if (responseStream == null)
            {
                _syn.Post(_sendOrPostCallback, new RequstResult());
                return;
            }
            try
            {
                using (var reader = new BinaryReader(responseStream))
                {
                    var bytes = reader.ReadBytes((int)responseStream.Length);
                    _syn.Post(_sendOrPostCallback, GetRequstResult(bytes));
                    reader.Close();
                }
                responseStream.Close();
            }
            catch (Exception exception)
            {
                _syn.Post(_sendOrPostCallback, new RequstResult {Error = exception});
            }
          
        }

        private static RequstResult GetRequstResult(byte[]bytes)
        {
            var requst = new RequstResult();
            var erro = JsonHelper.DeSerializerJson<RequestErro>(bytes,false);
            if (erro != null&&erro.exception!=null&&erro.innerexception!=null)
            {
                requst.Error =
                     new Exception(string.Format("请求发生错误，细节:\r\n{0}\r\n{1}",erro.exception,erro.innerexception));
            }
            else
            {
                requst.Result = bytes;
            }
            return requst;
        }

        public static string GetTimeStampUrl(string inputurl)
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = Convert.ToInt64(ts.TotalSeconds);
            if (inputurl.IndexOf("?", StringComparison.Ordinal) > 0)
            {
                return inputurl + "&timestamp=" + time;
            }
            return inputurl + "?timestamp=" + time;
        }

        private void RequestReadySocket(IAsyncResult asyncResult)
        {
            var request = asyncResult.AsyncState as WebRequest;
            if (request != null)
            {
                using (var requestStream = request.EndGetRequestStream(asyncResult))
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        writer.Write(_postData);
                        writer.Flush();
                        writer.Close();
                    }
                    requestStream.Close();
                }
            }
            if (request!=null)
            request.BeginGetResponse(ResponseReadySocket, request);
        }

        private void ResponseReadySocket(IAsyncResult asyncResult)
        {
            try
            { 
                var request = asyncResult.AsyncState as WebRequest;
                if (request == null) return;
                var response = request.EndGetResponse(asyncResult);
                using (var responseStream = response.GetResponseStream())
                {
                    GetResponse(responseStream);
                }
            }
            catch (Exception e)
            {
                _syn.Post(_sendOrPostCallback, new RequstResult{Error=e});
            }

        }

    }

    public sealed class HeaderType
    {
        public const string ApplicationXml = "application/xml";
        public const string ApplicationJson = "application/json";
        public const string ApplicationFormUrlencoded = "application/x-www-form-urlencoded";
        public const string Any = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";//IE
        public const string TextJson = "text/json";
        public const string TextXml = "text/xml";//最少在IE下编码没问题，360煞笔
    }

     public sealed class MediaType
     {
        public const string ApplicationXml = "application/xml";
        public const string ApplicationJson = "application/json";
        public const string ApplicationFormUrlencoded = "application/x-www-form-urlencoded";
    }


}
