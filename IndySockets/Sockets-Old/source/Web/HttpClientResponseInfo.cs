using System;
using System.IO;

namespace Indy.Sockets.Web
{
	public class HttpClientResponseInfo: ResponseHeaderInfo
	{
	  protected HttpClient _HttpClient;
	  protected int _ResponseCode;
	  protected string _ResponseText = "";
	  protected bool _KeepAlive;
	  protected Stream _ContentStream;
	  protected HttpProtocolVersionEnum _ResponseVersion;

    public HttpClientResponseInfo(HttpClient AParent)
    {
      _HttpClient = AParent;
    }
    
    public bool KeepAlive
    {
      get
      {
        if (_HttpClient.Connected())
        {
          _HttpClient.Socket.CheckForDisconnect(false);
        }
        _KeepAlive = _HttpClient.Connected();        
        
        try
        {
          string S = _ResponseText.Substring(5, 3);
					ResponseVersion = (HttpProtocolVersionEnum)Array.IndexOf(Http.HttpProtocolVersionNames, S);
          if (_KeepAlive)
          {
            switch(_HttpClient.ProtocolVersion)
            {
              case HttpProtocolVersionEnum.v1_0:
                {
                  /*
                   * By default we assume that keep-alive is not by default and will keep
                   * the connection only if there is "keep-alive"
                   */
                  _KeepAlive =
                    String.Equals(Connection.Trim(), "KEEP-ALIVE", StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(ProxyConnection.Trim(), "KEEP-ALIVE", StringComparison.InvariantCultureIgnoreCase);
                  break;
                }
              case HttpProtocolVersionEnum.v1_1:
                {
                  /*
                   * By default we assume that keep-alive is by default and will close
                   * the connection only if there is "close"
                   */
                  _KeepAlive =
                    String.Equals(Connection.Trim(), "CLOSE", StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(ProxyConnection.Trim(), "CLOSE", StringComparison.InvariantCultureIgnoreCase);
                  break;
                }
            }
          }
        }
        catch
        {
        }
        return _KeepAlive;
      }
      set
      {
        _KeepAlive = value;
      }
    }
    
    public string ResponseText
    {
      get
      {
        return _ResponseText;
      }
      set
      {
        _ResponseText = value;
      }
    }
    
    public int ResponseCode
    {
      get
      {
        string S = _ResponseText;
        Core.Global.Fetch(ref S);
        S = S.Trim();
        _ResponseCode = Core.Global.StrToInt32Def(Core.Global.Fetch(ref S, " ", false), -1);
        return _ResponseCode;
      }
      set
      {
        _ResponseCode = value;
      }
    }
    
    public HttpProtocolVersionEnum ResponseVersion
    {
      get
      {
        return _ResponseVersion;
      }
      set
      {
        _ResponseVersion = value;
      }
    }
    
    public Stream ContentStream
    {
      get
      {
        return _ContentStream;
      }
      set
      {
        _ContentStream = value;
      }
    }
	}
}
