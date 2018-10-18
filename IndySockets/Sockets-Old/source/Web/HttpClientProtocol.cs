using System;
using System.IO;

using Indy.Sockets.Protocols;

namespace Indy.Sockets.Web {
  public class HttpClientProtocol: IDisposable {
    protected HttpClient _HttpClient;
    protected int _ResponseCode;
    protected HttpClientResponseInfo _Response;
    protected HttpClientRequestInfo _Request;

    internal void SetRequest(HttpClientRequestInfo ARequest) {
      _Request = ARequest;
    }

    internal void SetResponse(HttpClientResponseInfo AResponse) {
      _Response = AResponse;
    }

    public HttpClientProtocol(HttpClient AConnection) {
      _HttpClient = AConnection;
      _Response = new HttpClientResponseInfo(_HttpClient);
      _Request = new HttpClientRequestInfo(_HttpClient);
    }

    private void CheckException(params short[] AIgnoreReplies) {
      int LResponseCode;
      using (Stream LRespStream = new MemoryStream()) {
        using (Stream LTempStream = new MemoryStream()) {
          try {
            _Response.ContentStream = LRespStream;
            _HttpClient.ReadResult(_Response);
            LResponseCode = _Response.ResponseCode;
            if (AIgnoreReplies.Length > 0) {
              foreach (short s in AIgnoreReplies) {
                if (LResponseCode == s) {
                  return;
                }
              }
            }
            using (StreamReader sr = new StreamReader(LRespStream)) {
              try {
                throw new HttpClientProtocolException(LResponseCode, _HttpClient.ResponseText, sr.ReadToEnd());
              } finally {
                sr.Close();
              }
            }
          } finally {
            _Response.ContentStream = LTempStream;
          }
        }
      }
    }

    private void ReadContent() {
      _HttpClient.ReadResult(_Response);
    }

    public HttpWhatsNextEnum ProcessResponse(params short[] AIgnoreReplies) {
      string LLocation = "";
      HttpCommandEnum LMethod;
      bool LNeedAuth = false;
      int LResponseDigit = _Response.ResponseCode / 100;
      HttpWhatsNextEnum TempResult;
      if ((LResponseDigit == 3
           && _Response.ResponseCode != 304)
        || (_Response.Location != "")) {
        LLocation = _Response.Location;
        if (_HttpClient.HandleRedirects
          && _HttpClient._RedirectCount < _HttpClient.RedirectMaximum) {
          LMethod = _Request.Method;
          if (_HttpClient.doOnRedirect(ref LLocation, ref LMethod, _HttpClient._RedirectCount)) {
            TempResult = HttpWhatsNextEnum.GoToUrl;
            _Request.Url = LLocation;
            if (_Response.ResponseCode == 302
              || _Response.ResponseCode == 303) {
              _Request.Source = null;
              _Request.Method = HttpCommandEnum.Get;
            } else {
              _Request.Method = LMethod;
            }
          } else {
            CheckException(AIgnoreReplies);
            return HttpWhatsNextEnum.JustExit;
          }
        } else {
          TempResult = HttpWhatsNextEnum.JustExit;
          LMethod = _Request.Method;
          if (!_HttpClient.doOnRedirect(ref LLocation, ref LMethod, _HttpClient._RedirectCount)) {
            CheckException(AIgnoreReplies);
            return HttpWhatsNextEnum.JustExit;
          } else {
            _Response.Location = LLocation;
          }
        }
        if (_HttpClient.Connected()) {
          /*
           * This is a workaround for buggy HttpClient 1.1 server which
           * don't return any body with 302 responses.
           */
#warning TODO: add support for timeouts
//          LTemp = _HttpClient.Socket.ReadTimeOut;
//          try {
//            _HttpClient.Socket.ReadTimeOut = 4000;
//            try {
              ReadContent();
//            } catch {
//
//            }
//          } finally {
//            _HttpClient.Socket.ReadTimeOut = LTemp;
//          }
        }
      } else {
        // if we get an error we disconnect if we use SSL
        // SSL is not supported and implemented yet.

        //if (_HttpClient.Socket != null)
        //{
        //  // Delphi Code:
        //  //   Response.KeepAlive := not (FHTTP.Connected and (FHTTP.Socket is TIdSSLSocketSocketBase) and Response.KeepAlive);
        //}
        if (LResponseDigit != 2) {
          switch (_Response.ResponseCode) {
            case 401: { // HttpClient Server Authorization Required
                if (_HttpClient.AuthRetries > _HttpClient.MaxAuthRetries
                  || !_HttpClient.doOnAuthorization(_Request, _Response)) {
                  if (_Request.Authentication != null) {
                    _Request.Authentication.Reset();
                  }
                  CheckException(AIgnoreReplies);
                  return HttpWhatsNextEnum.JustExit;
                } else {
                  LNeedAuth = (_HttpClient.HttpOptions | HttpOptionsEnum.InProcessAuth) == _HttpClient.HttpOptions;
                }
                break;
              }
            case 407: { // Proxy Server authorization required
                if (_HttpClient._AuthProxyRetries > _HttpClient.MaxAuthRetries
                  || !_HttpClient.doOnProxyAuthorization(_Request, _Response)) {
                  if (_HttpClient.ProxyParams.Authentication != null) {
                    _HttpClient.ProxyParams.Authentication.Reset();
                  }
                  CheckException(AIgnoreReplies);
                  return HttpWhatsNextEnum.JustExit;
                } else {
                  LNeedAuth = (_HttpClient.HttpOptions | HttpOptionsEnum.InProcessAuth) == _HttpClient.HttpOptions;
                }
                break;
              }
            default: {
                CheckException(AIgnoreReplies);
                return HttpWhatsNextEnum.JustExit;
              }
          }
        }
        if (LNeedAuth) {
          ReadContent();
          return HttpWhatsNextEnum.AuthRequest;
        } else {
          if (_Response.ResponseCode != 204) {
            _HttpClient.ReadResult(_Response);
          }
          return HttpWhatsNextEnum.JustExit;
        }
      }
      return TempResult;
    }

    public void BuildAndSendRequest(URI AUri) {
      _Request.SetHeaders();
      _HttpClient.ProxyParams.SetHeaders(_Request.RawHeaders);

      if (AUri != null) {
        _HttpClient.SetCookies(AUri, _Request);
      }
#warning TODO: Add support for buffering
//      _HttpClient.Socket.WriteBufferOpen();
//      try {
        switch (_Request.Method) {
          case HttpCommandEnum.Head: {
							_HttpClient.Socket.WriteLn("HEAD " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Get: {
							_HttpClient.Socket.WriteLn("GET " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Post: {
							_HttpClient.Socket.WriteLn("POST " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Options: {
							_HttpClient.Socket.WriteLn("OPTIONS " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Trace: {
							_HttpClient.Socket.WriteLn("TRACE " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Put: {
							_HttpClient.Socket.WriteLn("PUT " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
          case HttpCommandEnum.Connect: {
							_HttpClient.Socket.WriteLn("CONNECT " + _Request.Url + " HTTP/" + Http.HttpProtocolVersionNames[(int)_HttpClient.ProtocolVersion]);
              break;
            }
        }
        for (int i = 0; i < _Request.RawHeaders.Count; i++) {
          if (_Request.RawHeaders[i].Length > 0) {
            _HttpClient.Socket.WriteLn(_Request.RawHeaders[i]);
          }
        }
        _HttpClient.Socket.WriteLn("");
//        _HttpClient.Socket.WriteBufferClose();
//      } catch (ConnectionClosedGracefullyException) {
//      } catch (Exception) {
//        if (_HttpClient.Socket.WriteBufferingActive()) {
//          _HttpClient.Socket.WriteBufferCancel();
//        }
//        throw;
//      }
    }

    public void RetrieveHeaders(int AMaxHeaderCount) {
      _Response.RawHeaders.Clear();
      string S = _HttpClient.Socket.ReadLn();
      try {
        int LHeaderCount = 0;
        while (S != ""
             && (AMaxHeaderCount > 0 || LHeaderCount <= AMaxHeaderCount)) {
          _Response.RawHeaders.Add(S);
          S = _HttpClient.Socket.ReadLn();
          LHeaderCount++;
        }
      } catch (Core.ConnectionClosedGracefullyException) {
        _HttpClient.Disconnect();
      } catch (Exception) {
        throw;
      }
      _Response.ProcessHeaders();
    }

    public int ResponseCode {
      get {
        return _ResponseCode;
      }
    }

    public HttpClientRequestInfo Request {
      get {
        return _Request;
      }
    }

    public HttpClientResponseInfo Response {
      get {
        return _Response;
      }
    }

    void IDisposable.Dispose() {
      ((IDisposable)this._Request).Dispose();
      ((IDisposable)this._Response).Dispose();
      _Response = null;
      _Request = null;
      GC.SuppressFinalize(this);
    }
  }
}