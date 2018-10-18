using System;
using System.IO;

namespace Indy.Sockets.Web {
  public class HttpClientRequestInfo: RequestHeaderInfo {
    protected HttpClient _HttpClient;
    protected string _Url = "";
    protected HttpCommandEnum _Method;
    protected Stream _SourceStream;
    protected HttpConnectionTypeEnum _UseProxy;
    //	  protected IPVersionEnum _IPVersion;

    public HttpClientRequestInfo(HttpClient AHttp) {
      _HttpClient = AHttp;
      _UseProxy = HttpConnectionTypeEnum.Normal;
      Url = "/";
    }

    public string Url {
      get {
        return _Url;
      }
      set {
        _Url = value;
      }
    }

    public HttpCommandEnum Method {
      get {
        return _Method;
      }
      set {
        _Method = value;
      }
    }

    public Stream Source {
      get {
        return _SourceStream;
      }
      set {
        _SourceStream = value;
      }
    }

    public HttpConnectionTypeEnum UseProxy {
      get {
        return _UseProxy;
      }
      set {
        _UseProxy = value;
      }
    }

//    public IPVersionEnum IPVersion {
//      get {
//        return _IPVersion;
//      }
//      set {
//        _IPVersion = value;
//      }
//    }
  }
}
