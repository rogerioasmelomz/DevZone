using System;
using System.Collections.Generic;
using System.IO;

using Indy.Sockets.Core;

namespace Indy.Sockets.Web
{
	public class HttpRequestInfo: RequestHeaderInfo
	{
	  protected bool _AuthExists;
	  protected ServerCookies _Cookies;
    protected SortedList<string, string> _Params = new SortedList<string, string>();
	  protected Stream _PostStream;
	  protected string _RawHttpCommand = "";
	  protected string _RemoteIP = "";
	  protected int _RemotePort = 0;
	  protected HttpSession _Session;
	  protected string _Document = "";
	  protected string _Command = "";
	  protected string _Version = "";
    protected string _AuthUserName = "";
	  protected string _AuthPassword = "";
	  protected string _UnparsedParams = "";
	  protected string _QueryParams = "";
	  protected string _FormParams;
	  protected HttpCommandEnum _CommandType;
	  protected CustomHttpServer _Server;
		protected ContextRFC _Context;
	  
	  protected virtual void DoDecodeAndSetParams(string AValue)
	  {
	    Params.Clear();
	    int i = 0;
	    while (i < AValue.Length)
	    {
	      int j = i;
	      while (j <= AValue.Length
	           &&AValue[j] != '&')
        {
          j++;
        }
        string S = AValue.Substring(i, j - i);
	      S = S.Replace('+', ' ');
        string[] parts = Protocols.URI.URLDecode(S).Split('=');
	      Params.Add(parts[0], parts[1]);
	      i = j + 1;
	    }
	  }
	  
	  internal void DecodeAndSetParams(string AValue)
	  {
	    DoDecodeAndSetParams(AValue);
	  }
	  
	  protected virtual void DecodeCommand()
	  {
			for (int i = 0; i < Http.HttpCommandNames.Length; i++)
	    {
				if (Command.Equals(Http.HttpCommandNames[i], StringComparison.InvariantCultureIgnoreCase))
	      {
	        _CommandType = (HttpCommandEnum)i;
	        break;
	      }
	    }
	  }
	  
	  internal Stream __PostStream
	  {
	    get
	    {
	      return _PostStream;
	    }
	    set
	    {
	      _PostStream = value;
	    }
	  }
	  
	  internal string __QueryParams
	  {
	    get
	    {
	      return _QueryParams;
	    }
	    set
	    {
	      _QueryParams = value;
	    }
	  }
	  
	  internal string __RemoteIP
	  {
	    get
	    {
	      return _RemoteIP;
	    }
	    set
	    {
	      _RemoteIP = value;
	    }
	  }
	  
	  internal int __RemotePort
	  {
	    get
	    {
	      return _RemotePort;
	    }
	    set
	    {
	      _RemotePort = value;
	    }
	  }
	  
	  internal string __Version
	  {
	    get
	    {
	      return _Version;
	    }
	    set
	    {
	      _Version = value;
	    }
	  }
	  
	  internal string __UnparsedParams
	  {
	    get
	    {
	      return _UnparsedParams;
	    }
	    set
	    {
	      _UnparsedParams = value;
	    }
	  }

		internal string __Command {
			get {
				return _Command;
			}
			set {
				if (_Command != value) {
					_Command = value;
					DecodeCommand();
				}
			}
		}
	  
	  internal string __Document
	  {
	    get
	    {
	      return _Document;
	    }
	    set
	    {
	      _Document = value;
	    }
	  }
	  
	  internal bool __AuthExists
	  {
	    get
	    {
	      return _AuthExists;
	    }
	    set
	    {
	      _AuthExists = value;
	    }
	  }
	  
	  internal string __AuthUserName
	  {
	    get
	    {
	      return _AuthUserName;
	    }
	    set
	    {
	      _AuthUserName = value;
	    }
	  }
	  
	  internal string __AuthPassword
	  {
	    get
	    {
	      return _AuthPassword;
	    }
	    set
	    {
	      _AuthPassword = value;
	    }
	  }
	  
	  internal string __Host
	  {
	    get
	    {
	      return _Host;
	    }
	    set
	    {
	      _Host = value;
	    }
	  }
	  
	  internal string __FormParams
	  {
	    get
	    {
	      return _FormParams;
	    }
	    set
	    {
	      _FormParams = value;
	    }
	  }

	  internal void SetSession(HttpSession ASession)
	  {
	    _Session = ASession;
	  }

		public HttpRequestInfo(string ARawHTTPCommand, string ARemoteIP, string ACommand, CustomHttpServer AServer, ContextRFC AContext)
	  {
	    _Server = AServer;
	    _Context = AContext;
	    _CommandType = HttpCommandEnum.Unknown;
      _Cookies = new ServerCookies();
      ContentLength = -1;
	  }
	  
	  ~HttpRequestInfo()
	  {
	    _Params.Clear();
	    _Params = null;
	    _Cookies.Clear();
	    _Cookies = null;
	    if (_PostStream != null)
	    {
	      _PostStream.Close();
	    }
	  }
	  
	  public HttpSession Session
	  {
	    get
	    {
	      return _Session;
	    }
	  }
	  
	  public bool AuthExists
	  {
	    get
	    {
	      return _AuthExists;
	    }
	  }
	  
	  public string AuthUserName
	  {
	    get
	    {
	      return _AuthUserName;
	    }
	  }
	  
	  public string AuthPassword
	  {
	    get
	    {
	      return _AuthPassword;
	    }
	  }
	  
	  public string Command
	  {
	    get
	    {
	      return _Command;
	    }
	  }
	  
	  public HttpCommandEnum CommandType
	  {
	    get
	    {
	      return _CommandType;
	    }
	  }
	  
	  public ServerCookies Cookies
	  {
	    get
	    {
	      return _Cookies;
	    }
	  }
	  
	  public string Document
	  {
	    get
	    {
	      return _Document;
	    }
	  }
	  
	  public SortedList<string, string> Params
	  {
	    get
	    {
	      return _Params;
	    }
	  }
	  
	  public Stream PostStream
	  {
	    get
	    {
	      return _PostStream;
	    }
	    set
	    {
	      _PostStream = value;
	    }
	  }
	  
	  public string RawHttpCommand
	  {
	    get
	    {
	      return _RawHttpCommand;
	    }
	  }
	  
	  public string RemoteIP
	  {
	    get
	    {
	      return _RemoteIP;
	    }
	  }
	  
	  public string UnparsedParams
	  {
	    get
	    {
	      return _UnparsedParams;
	    }
	  }
	  
	  public string FormParams
	  {
	    get
	    {
	      return _FormParams;
	    }
	  }
	  
	  public string QueryParams
	  {
	    get
	    {
	      return _QueryParams;
	    }
	  }
	  
	  public string Version
	  {
	    get
	    {
	      return _Version;
	    }
	  }
	  
	  public CustomHttpServer Server
	  {
	    get
	    {
	      return _Server;
	    }
	  }

		public ContextRFC Context
	  {
	    get
	    {
	      return _Context;
	    }
	  }
	  
	  public int RemotePort
	  {
	    get
	    {
	      return _RemotePort;
	    }
	  }
	}
}
