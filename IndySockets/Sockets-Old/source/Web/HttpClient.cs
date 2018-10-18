using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using Indy.Sockets.Core;
using Indy.Sockets.Protocols;
using Indy.Sockets.Protocols.Authentication;

namespace Indy.Sockets.Web {
	public class HttpClient: TcpClient<int, ReplyRFC> {
		internal int _RedirectCount;
		internal int _AuthProxyRetries;
		protected int _AuthRetries;
		protected CookieManager _CookieManager;
		protected CompressorBase _Compressor;
		protected int _MaxAuthRetries;
		protected int _MaxHeaderLines;
		protected bool _AllowCookies;
		protected AuthenticationManager _AuthenticationManager;
		protected HttpProtocolVersionEnum _ProtocolVersion;
		protected int _RedirectMaximum;
		protected bool _HandleRedirects;
		protected HttpOptionsEnum _HttpOptions;
		protected URI _Url;
		protected HttpClientProtocol _HttpProto;
		protected ProxyConnectionInfo _ProxyParams;
		protected HttpOnRedirectEventHandler _OnRedirect;
		protected HttpOnSelectAuthorizationEventHandler _OnSelectAuthorization;
		protected HttpOnSelectAuthorizationEventHandler _OnSelectProxyAuthorization;
		protected HttpOnAuthorizationEventHandler _OnAuthorization;
		protected HttpOnAuthorizationEventHandler _OnProxyAuthorization;

		protected virtual void DoRequest(HttpCommandEnum AMethod, string AUrl, Stream ASource, Stream AResponseContent, params short[] AIgnoreReplies) {
			try {
				int LResponseLocation = -1;
				if (AResponseContent != null) {
					LResponseLocation = (int)AResponseContent.Position;
				}
				_AuthRetries = 0;
				_AuthProxyRetries = 0;
				URI AUri = new URI(AUrl);
				Request.Url = AUrl;
				Request.Method = AMethod;
				Request.Source = ASource;
				Response.ContentStream = AResponseContent;
				try {
					bool BreakWhileLoop = false;
					do {
						_RedirectCount++;
						PrepareRequest(Request);
						if (Socket is SocketTLS) {
							// Delphi Code:
							//      TIdSSLIOHandlerSocketBase(IOHandler).URIToCheck := FURI.URI;
						}
						ConnectToHost(Request, Response);
						/*
						 * Workaround for servers which response with 100 Continue on GET and HEAD
						 * This workaround is just for temporary until we have final HTTP 1.1 
						 * realisation. HTTP 1.1 is ongoing because of all the buggy and 
						 * conflicting servers.
						 */
						do {
							Response.ResponseText = Socket.ReadLn();
							_HttpProto.RetrieveHeaders(MaxHeaderLines);
							ProcessCookies(Request, Response);
						}
						while (Response.ResponseCode == 100);

						switch (_HttpProto.ProcessResponse(AIgnoreReplies)) {
							case HttpWhatsNextEnum.AuthRequest: {
									_RedirectCount -= 1;
									Request.Url = AUrl;
									break;
								}
							case HttpWhatsNextEnum.ReadAndGo: {
									ReadResult(Response);
									if (AResponseContent != null) {
										AResponseContent.Position = LResponseLocation;
										AResponseContent.SetLength(LResponseLocation);
									}
									_AuthRetries = 0;
									_AuthProxyRetries = 0;
									break;
								}
							case HttpWhatsNextEnum.GoToUrl: {
									if (AResponseContent != null) {
										AResponseContent.Position = LResponseLocation;
										AResponseContent.SetLength(LResponseLocation);
									}
									if (Response.Location != "") {
										URI _Uri = new URI(Response.Location);

										if (_Uri.Host != "") {
											Request.Host = _Uri.Host;
										}
										if (_Uri.Port != "") {
											this.Port = Int32.Parse(_Uri.Port);
										}
										// todo: make more generic
										if (this.Port == Core.TcpPorts.Https) {
											Request.Url = "https://" + Request.Host;
										} else {
											Request.Url = "http://" + Request.Host;
										}
										Request.Url += _Uri.Path + _Uri.Document;
										if (_Uri.Params != "") {
											Request.Url += "?" + _Uri.Params;
										}
										if (_Uri.Bookmark != "") {
											Request.Url += _Uri.Bookmark;
										}
									}
									_AuthRetries = 0;
									_AuthProxyRetries = 0;
									break;
								}
							case HttpWhatsNextEnum.JustExit: {
									BreakWhileLoop = true;
									break;
								}
							case HttpWhatsNextEnum.DontKnow: {
									throw new IndyException(ResourceStrings.NotAcceptable);
								}
						}
					}
					while (true && !BreakWhileLoop);
				} finally {
					if (!Response.KeepAlive) {
						Disconnect();
					}
					_RedirectCount = 0;
				}
			}
				//		  catch(ConnectionClosedGracefullyException)
				//		  {
				//		  }
			catch (Exception) {
				throw;
			}
		}

		internal bool doOnAuthorization(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			return DoOnAuthorization(ARequest, AResponse);
		}

		protected virtual bool DoOnAuthorization(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			Type Auth = null;
			string S = "";
			_AuthRetries++;
			if (ARequest.Authentication == null) {
				for (int i = 0; i < AResponse.WWWAuthenticate.Count; i++) {
					S = AResponse.WWWAuthenticate[i];
					Auth = AuthenticationRegistry.FindAuthenticationMethod(Core.Global.Fetch(ref S));
					if (Auth != null) {
						break;
					}
				}
			}
			if (Auth == null) {
				return false;
			}
			if (_OnSelectAuthorization != null) {
				_OnSelectAuthorization(this, ref Auth, AResponse.WWWAuthenticate);
			}
			ARequest.Authentication = AuthenticationRegistry.CreateAuthenticationMethod(Auth);
			bool TempResult = (_OnAuthorization != null) || ARequest.Password.Trim() != "";
			if (TempResult) {
				ARequest.Authentication.UserName = ARequest.UserName;
				ARequest.Authentication.Password = ARequest.Password;
				ARequest.Authentication.Params.Values("Authorization", ARequest.Authentication.Authentication());
				ARequest.Authentication.AuthParams = AResponse.WWWAuthenticate;
				TempResult = false;
				bool BreakWithWhileLoop = false;
				do {
					switch (ARequest.Authentication.Next()) {
						case AuthenticationWhatsNextEnum.AskTheProgram: {
								if (_OnAuthorization != null) {
									ARequest.Authentication.UserName = ARequest.UserName;
									ARequest.Authentication.Password = ARequest.Password;
									_OnAuthorization(this, ARequest.Authentication, ref TempResult);
									if (TempResult) {
										ARequest.BasicAuthentication = true;
										ARequest.UserName = ARequest.Authentication.UserName;
										ARequest.Password = ARequest.Authentication.Password;
									} else {
										return false;
									}
								}
								break;
							}
						case AuthenticationWhatsNextEnum.DoRequest: {
								return true;
							}
						case AuthenticationWhatsNextEnum.Fail: {
								return false;
							}
					}
				}
				while (true && !BreakWithWhileLoop);
			}
			return TempResult;
		}

		internal bool doOnProxyAuthorization(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			return DoOnProxyAuthorization(ARequest, AResponse);
		}

		protected virtual bool DoOnProxyAuthorization(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			Type Auth = null;
			string S = "";
			_AuthProxyRetries++;
			if (ProxyParams.Authentication == null) {
				for (int i = 0; i < AResponse.ProxyAuthenticate.Count; i++) {
					S = AResponse.ProxyAuthenticate[i];
					Auth = AuthenticationRegistry.FindAuthenticationMethod(Core.Global.Fetch(ref S));
					if (Auth != null) {
						break;
					}
				}
			}
			if (Auth == null) {
				return false;
			}
			if (_OnSelectProxyAuthorization != null) {
				_OnSelectProxyAuthorization(this, ref Auth, AResponse.ProxyAuthenticate);
			}

			ProxyParams.Authentication = AuthenticationRegistry.CreateAuthenticationMethod(Auth);
			bool TempResult = _OnProxyAuthorization != null;
			if (AResponse.ResponseCode == 407) {
				ProxyParams.ProxyPassword = "";
				ProxyParams.Authentication.Reset();
			}
			if (TempResult) {
				ProxyParams.Authentication.UserName = ProxyParams.ProxyUserName;
				ProxyParams.Authentication.Password = ProxyParams.ProxyPassword;
				ProxyParams.Authentication.AuthParams = AResponse.ProxyAuthenticate;
				TempResult = false;
				bool BreakWhileLoop = false;
				do {
					switch (ProxyParams.Authentication.Next()) {
						case AuthenticationWhatsNextEnum.AskTheProgram: {
								if (_OnProxyAuthorization != null) {
									ProxyParams.Authentication.UserName = ProxyParams.ProxyUserName;
									ProxyParams.Authentication.Password = ProxyParams.ProxyPassword;
									_OnProxyAuthorization(this, ProxyParams.Authentication, ref TempResult);
									if (TempResult) {
										ProxyParams.ProxyUserName = ProxyParams.Authentication.UserName;
										ProxyParams.ProxyPassword = ProxyParams.Authentication.Password;
									} else {
										BreakWhileLoop = true;
									}
								}
								break;
							}
						case AuthenticationWhatsNextEnum.DoRequest: {
								return true;
							}
						case AuthenticationWhatsNextEnum.Fail: {
								return false;
							}
					}
				}
				while (true && !BreakWhileLoop);
			}
			return TempResult;
		}

		internal bool doOnRedirect(ref string RLocation, ref HttpCommandEnum RMethod, int ARedirectCount) {
			return DoOnRedirect(ref RLocation, ref RMethod, ARedirectCount);
		}

		protected virtual bool DoOnRedirect(ref string RLocation, ref HttpCommandEnum RMethod, int ARedirectCount) {
			bool TempResult = HandleRedirects;
			if (_OnRedirect != null) {
				_OnRedirect(this, ref RLocation, ARedirectCount, ref TempResult, ref RMethod);
			}
			return TempResult;
		}

		internal void ProcessCookies(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			if (_CookieManager == null
				&& AllowCookies) {
				CookieManager = new CookieManager();
			}
			if (_CookieManager != null) {
				List<string> Cookies = new List<string>();
				AResponse.RawHeaders.Extract("Set-cookie", Cookies);
				for (int i = 0; i < Cookies.Count; i++) {
					CookieManager.AddCookie(Cookies[i], _Url.Host);
				}
				Cookies.Clear();
				AResponse.RawHeaders.Extract("Set-cookie2", Cookies);
				for (int i = 0; i < Cookies.Count; i++) {
					CookieManager.AddCookie2(Cookies[i], _Url.Host);
				}
			}
		}

		protected HttpConnectionTypeEnum SetHostAndPort() {
			string LHost = Url.Host;
			int LPort = Core.Global.StrToInt32Def(Url.Port, 80);
			mPort = LPort;
			HttpConnectionTypeEnum TempResult;
			if (ProxyParams.ProxyServer.Length > 0) {
				if ((!LHost.Equals(ProxyParams.ProxyServer, StringComparison.InvariantCultureIgnoreCase)
						|| LPort != ProxyParams.ProxyPort)
					 && Connected()) {
					Disconnect();
				}
				LHost = ProxyParams.ProxyServer;
				LPort = ProxyParams.ProxyPort;
				if (Url.Protocol.Equals("HTTPS", StringComparison.InvariantCultureIgnoreCase)) {
					TempResult = HttpConnectionTypeEnum.SslProxy;
					if (Socket != null) {
						if (Socket is SocketTLS) {
							((SocketTLS)Socket).PassThrough = true;
						} else {
							throw new IndyException("Invalid Socket");
						}
					}
				} else {
					TempResult = HttpConnectionTypeEnum.Proxy;
					if (Socket != null
						&& Socket is SocketTLS) {
						((SocketTLS)Socket).PassThrough = true;
					}
				}
			} else {
				TempResult = HttpConnectionTypeEnum.Normal;
#warning TODO: Add support for IPv6
				/*        if (Socket is IOHandlerSocket) {
          if (((IOHandlerSocket)Socket).Binding != null) {
            if (Url.IPVersion != ((IOHandlerSocket)Socket).Binding.IPVersion) {
              if (Connected()) {
                Disconnect();
              }
            }
          }
        }                */
				if (!LHost.Equals(Url.Host, StringComparison.InvariantCultureIgnoreCase)
					 || LPort != Core.Global.StrToInt32Def(Url.Port, 80)) {
					if (Connected()) {
						Disconnect();
					}
					LHost = Url.Host;
					LPort = Core.Global.StrToInt32Def(Url.Port, 80);
				}
				if (Url.Protocol.Equals("HTTPS", StringComparison.InvariantCultureIgnoreCase)) {
					if (Socket != null
						|| !(Socket is SocketTLS)) {
						throw new IndyException("Invalid Socket");
					} else {
						((SocketTLS)Socket).PassThrough = false;
						TempResult = HttpConnectionTypeEnum.Ssl;
					}
				} else {
					if (Socket != null) {
						if (Socket is SocketTLS) {
							((SocketTLS)Socket).PassThrough = true;
						}
					}
				}
			}

			Host = LHost;
			Port = LPort;
			return TempResult;
		}

		internal void SetCookies(URI AUrl, HttpClientRequestInfo ARequest) {
			if (_CookieManager != null) {
				// TO DO: integrate SSL
				string S = _CookieManager.GenerateCookieList(AUrl, false);
				if (S.Length > 0) {
					ARequest.RawHeaders.Values("Cookie", S);
				}
			}
		}

		private int ChunkSize() {
			int j;
			string S = Socket.ReadLn();
			j = S.IndexOf(" ");
			if (j > 0) {
				S = S.Substring(0, j);
			}
			try {
				return Int32.Parse(S, NumberStyles.HexNumber);
			} catch (FormatException) {
				return 0;
			}
		}

		internal void ReadResult(HttpClientResponseInfo AResponse) {
			int Size;
			if (AResponse.ContentStream != null) {
				if (AResponse.ContentLength > 0) {
					try {
						Socket.ReadStream(AResponse.ContentStream, AResponse.ContentLength);
					} catch (ConnectionClosedGracefullyException) {
					} catch (Exception) {
						throw;
					}
				} else {
					if (AResponse.RawHeaders.Values("Transfer-Encoding").IndexOf("chunked") > 0) {
#warning TODO: Add Status support
						//            DoStatus(StatusEnum.StatusText, ResourceStrings.ChunkStarted);
						Size = ChunkSize();
						while (Size > 0) {
							Socket.ReadStream(AResponse.ContentStream, Size);
							Socket.ReadLn(); // blank line
							Size = ChunkSize();
						}
						Socket.ReadLn();
					} else {
						if (!AResponse.HasContentLength) {
							Socket.ReadStream(AResponse.ContentStream, -1, true);
						}
					}
				}
				if (_Compressor != null
					&& AResponse.ContentEncoding == "deflate") {
					AResponse.ContentStream.Position = 0;
					_Compressor.DecompressDeflateStream(AResponse.ContentStream);
				} else {
					if (_Compressor != null
						&& AResponse.ContentEncoding == "gzip") {
						AResponse.ContentStream.Position = 0;
						_Compressor.DecompressGZipStream(AResponse.ContentStream);
					}
				}
			}
		}

		protected void PrepareRequest(HttpClientRequestInfo ARequest) {
			URI LUrl = new URI(ARequest.Url);
			if (LUrl.UserName.Length > 0) {
				ARequest.UserName = LUrl.UserName;
				ARequest.Password = LUrl.Password;
			}
			_Url.UserName = ARequest.UserName;
			_Url.Password = ARequest.Password;
			_Url.Path = Http.ProcessPath(_Url.Path, LUrl.Path);
			_Url.Document = LUrl.Document;
			_Url.Params = LUrl.Params;
			if (LUrl.Host.Length > 0) {
				_Url.Host = LUrl.Host;
			}
			if (LUrl.Protocol.Length > 0) {
				_Url.Protocol = LUrl.Protocol;
			} else {
				if (_Url.Protocol.Equals("https", StringComparison.InvariantCultureIgnoreCase)) {
					_Url.Protocol = "https";
				} else {
					_Url.Protocol = "http";
				}
			}
			if (LUrl.Port.Length > 0) {
				_Url.Port = LUrl.Port;
			} else {
				if (LUrl.Protocol.Equals("http", StringComparison.InvariantCultureIgnoreCase)) {
					_Url.Port = Core.TcpPorts.Http.ToString();
				} else {
					if (LUrl.Protocol.Equals("https", StringComparison.InvariantCultureIgnoreCase)) {
						_Url.Port = Core.TcpPorts.Https.ToString();
					} else {
						if (_Url.Port.Length == 0) {
							throw new HttpUnknownProtocolException("");
						}
					}
				}
			}
			ARequest.Url = _Url.Path + _Url.Document;
			if (_Url.Params != "") {
				ARequest.Url += "?" + _Url.Params;
			}
			if (ARequest.Method == HttpCommandEnum.Options) {
				if (LUrl.Document.Trim().Length == 0) {
					ARequest.Url = LUrl.Document;
				}
			}
#warning TODO: add IPv6 support
			//ARequest.IPVersion = LUrl.IPVersion;
			//_Url.IPVersion = ARequest.IPVersion;
			if (ARequest.Method == HttpCommandEnum.Trace
				|| ARequest.Method == HttpCommandEnum.Put
				|| ARequest.Method == HttpCommandEnum.Options
				|| ARequest.Method == HttpCommandEnum.Delete) {
				if (ProtocolVersion != HttpProtocolVersionEnum.v1_1) {
					throw new IndyException(ResourceStrings.MethodRequiresVersion);
				}
			}
			if (ARequest.Method == HttpCommandEnum.Post
				|| ARequest.Method == HttpCommandEnum.Put) {
				ARequest.ContentLength = (int)ARequest.Source.Length;
			} else {
				ARequest.ContentLength = -1;
			}
			ARequest.Host = _Url.Host;
			if (_Url.Port != Core.TcpPorts.Http.ToString()) {
				ARequest.Host += ":" + _Url.Port;
				Port = Int32.Parse(_Url.Port);
			}
			if (Host != ARequest.Host
				&& ARequest.Host != ""
				&& (Response.ResponseCode / 100) == 3) {
				Host = ARequest.Host;
			}
		}

		protected void ConnectToHost(HttpClientRequestInfo ARequest, HttpClientResponseInfo AResponse) {
			ARequest.UseProxy = SetHostAndPort();
			if (ARequest.UseProxy == HttpConnectionTypeEnum.Proxy) {
				ARequest.Url = _Url.URIString;
			}
			switch (ARequest.UseProxy) {
				case HttpConnectionTypeEnum.Normal: {
						if (ProtocolVersion == HttpProtocolVersionEnum.v1_0
							&& ARequest.Connection.Length == 0) {
							ARequest.Connection = "keep-alive";
						}
						break;
					}
				case HttpConnectionTypeEnum.Proxy: {
						if (ProtocolVersion == HttpProtocolVersionEnum.v1_0
							&& ARequest.Connection.Length == 0) {
							ARequest.ProxyConnection = "keep-alive";
						}
						break;
					}
				case HttpConnectionTypeEnum.Ssl:
				case HttpConnectionTypeEnum.SslProxy: {
						ARequest.Connection = "";
						break;
					}
			}
			if (ARequest.UseProxy == HttpConnectionTypeEnum.SslProxy) {
				using (HttpClientProtocol LocalHttp = new HttpClientProtocol(this)) {
					LocalHttp.Request.UserAgent = ARequest.UserAgent;
					LocalHttp.Request.Host = ARequest.Host;
					LocalHttp.Request.ContentLength = ARequest.ContentLength;
					LocalHttp.Request.Pragma = "no-cache";
					LocalHttp.Request.Url = Url.Host + ":" + Url.Port;
					LocalHttp.Request.Method = HttpCommandEnum.Connect;
					LocalHttp.Request.ProxyConnection = "keep-alive";
					using (LocalHttp.Response.ContentStream = new MemoryStream()) {
						try {
							do {
								CheckAndConnect(LocalHttp.Response);
								LocalHttp.BuildAndSendRequest(null);
								LocalHttp.Response.ResponseText = Socket.ReadLn();
								if (LocalHttp.Response.ResponseText.Length == 0) {
									LocalHttp.Response.ResponseText = "HTTP/1.0 200 OK";
									LocalHttp.Response.Connection = "close";
								} else {
									LocalHttp.RetrieveHeaders(MaxHeaderLines);
									ProcessCookies(LocalHttp.Request, LocalHttp.Response);
								}
								if (LocalHttp.Response.ResponseCode == 200) {
									((SocketTLS)Socket).PassThrough = false;
									break;
								} else {
									LocalHttp.ProcessResponse();
								}
							}
							while (true);
						} catch (Exception E) {
							// todo: Add property that will contain the error messages.
							throw new Exception("Error Occurred", E);
						}
					}
				}
			} else {
				CheckAndConnect(AResponse);
			}
			_HttpProto.BuildAndSendRequest(Url);
			if (ARequest.Method == HttpCommandEnum.Post
				|| ARequest.Method == HttpCommandEnum.Put) {
				Socket.Write(ARequest.Source, 0, false);
			}
		}

		protected void EncodeRequestParams(SortedList<string, string> AStrings) {
			for (int i = 0; i < AStrings.Count; i++) {
				if (AStrings.Values[i].Length > 0) {
					AStrings.Values[i] = URI.ParamsEncode(AStrings.Values[i]);
				}
			}
		}

		protected string SetRequestParams(SortedList<string, string> AStrings) {
			if (AStrings != null) {
				if ((_HttpOptions | HttpOptionsEnum.ForceEncodeParams) == _HttpOptions) {
					EncodeRequestParams(AStrings);
				}
				string TempResult = "";
				foreach (string key in AStrings.Keys) {
					TempResult += key.Trim() + "\r\n";
				}
				if (TempResult.Length > 0) {
					TempResult = TempResult.Substring(0, TempResult.Length - 2);
				}
				return TempResult;
			} else {
				return "";
			}
		}

		protected void CheckAndConnect(HttpClientResponseInfo AResponse) {
			if (!AResponse.KeepAlive) {
				Disconnect();
			}
			CheckForGracefulDisconnect(false);
			if (!Connected()) {
#warning TODO: Add IPv6 support
				//        IPVersion = _Url.IPVersion;
				if (this.Socket != null) {
					Connect(this.Socket);
				} else {
					Connect();
				}
			}
			CheckForGracefulDisconnect(false);
		}

		protected override void DoOnDisconnected() {
			base.DoOnDisconnected();
			if (Request.Authentication != null
				&& Request.Authentication.CurrentStep == Request.Authentication.Steps) {
				using (Request.Authentication) {
					if (AuthenticationManager != null) {
						AuthenticationManager.AddAuthentication(Request.Authentication, Url);
					}
				}
			}
			if (ProxyParams.Authentication != null
				&& ProxyParams.Authentication.CurrentStep == ProxyParams.Authentication.Steps) {
				ProxyParams.Authentication.Reset();
			}
		}

		public HttpClient() {
			_Url = new URI("");
			_AuthRetries = 0;
			_AuthProxyRetries = 0;
			AllowCookies = true;
			_HttpOptions = HttpOptionsEnum.ForceEncodeParams;
			_RedirectMaximum = Http.HttpClient_RedirectMax;
			_HandleRedirects = Http.HttpClient_HandleRedirects;
			_ProtocolVersion = Http.HttpClient_ProtocolVersion;
			_HttpProto = new HttpClientProtocol(this);
			_ProxyParams = new ProxyConnectionInfo();
			_MaxAuthRetries = Http.HttpClient_MaxAuthRetries;
			_MaxHeaderLines = Http.HttpClient_MaxHeaderLines;
		}

		public void Options(string AUrl) {
			DoRequest(HttpCommandEnum.Options, AUrl, null, null);
		}

		public void Get(string AUrl, Stream AResponseContent) {
			Get(AUrl, AResponseContent, new short[] { });
		}

		public void Get(string AUrl, Stream AResponseContent, params short[] AIgnoreReplies) {
			DoRequest(HttpCommandEnum.Get, AUrl, null, AResponseContent, AIgnoreReplies);
		}

		public string Get(string AUrl) {
			return Get(AUrl, new short[] { });
		}

		public string Get(string AUrl, params short[] AIgnoreReplies) {
			using (MemoryStream LStream = new MemoryStream()) {
				Get(AUrl, LStream, AIgnoreReplies);
				LStream.Position = 0;
				using (StreamReader LStreamReader = new StreamReader(LStream)) {
					try {
						return LStreamReader.ReadToEnd();
					} finally {
						LStreamReader.Close();
					}
				}
			}
		}

		public void Trace(string AUrl, Stream AResponseContent) {
			DoRequest(HttpCommandEnum.Trace, AUrl, null, AResponseContent);
		}

		public string Trace(string AUrl) {
			using (MemoryStream ms = new MemoryStream()) {
				Trace(AUrl, ms);
				ms.Position = 0;
				using (StreamReader sr = new StreamReader(ms)) {
					try {
						return sr.ReadToEnd();
					} finally {
						sr.Close();
					}
				}
			}
		}

		public void Head(string AUrl) {
			DoRequest(HttpCommandEnum.Head, AUrl, null, null);
		}

		public string Post(string AUrl, SortedList<string, string> ASource) {
			using (MemoryStream ms = new MemoryStream()) {
				Post(AUrl, ASource, ms);
				ms.Position = 0;
				using (StreamReader sr = new StreamReader(ms)) {
					try {
						return sr.ReadToEnd();
					} finally {
						sr.Close();
					}
				}
			}
		}

		public string Post(string AUrl, Stream ASource) {
			using (MemoryStream ms = new MemoryStream()) {
				Post(AUrl, ASource, ms);
				ms.Position = 0;
				using (StreamReader sr = new StreamReader(ms)) {
					try {
						return sr.ReadToEnd();
					} finally {
						sr.Close();
					}
				}
			}
		}
		/*
		function Post(AURL: string; ASource: TIdMultiPartFormDataStream): string; overload;
		procedure Post(AURL: string; ASource: TIdMultiPartFormDataStream; AResponseContent: TStream); overload;    
		*/

		public void Post(string AUrl, SortedList<string, string> ASource, Stream AResponseContent) {
			if (Request.ContentType == ""
				|| Request.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase)) {
				Request.ContentType = "application/x-www-form-urlencoded";
			}
			using (MemoryStream ms = new MemoryStream()) {
				using (StreamWriter sw = new StreamWriter(ms)) {
					sw.Write(SetRequestParams(ASource));
					sw.Flush();
					ms.Position = 0;
					Post(AUrl, ms, AResponseContent);
					sw.Close();
				}
			}
		}

		public void Post(string AUrl, Stream ASource, Stream AResponseContent) {
			HttpProtocolVersionEnum LOldProtocol;

			/*
			 * PLEASE READ CAREFULLY
			 * 
			 * Currently when issuing a POST, HttpClient will automatically
			 * set the protocol version to 1.0 independently of the value it had 
			 * initially. This is because there are some servers that don't respect
			 * the RFC to the full extent. In particular, they don't respect 
			 * sending/not sending the Expect: 100-Continue header. Until we find 
			 * an optimum solution that does NOT break the RFC, we will restrict 
			 * POSTs to version 1.0.
			 */
			if (Connected()) {
				Disconnect();
			}
			LOldProtocol = _ProtocolVersion;
			/*
			 * if KeepOrigProtocol is SET, assume the developer knows the operations
			 * of the server.
			 */
			if ((_HttpOptions | HttpOptionsEnum.KeepOrigProtocol) != _HttpOptions) {
				_ProtocolVersion = HttpProtocolVersionEnum.v1_0;
			}
			try {
				DoRequest(HttpCommandEnum.Post, AUrl, ASource, AResponseContent);
			} finally {
				_ProtocolVersion = LOldProtocol;
			}
		}

		public string Put(string AUrl, Stream ASource) {
			using (MemoryStream ms = new MemoryStream()) {
				Post(AUrl, ASource, ms);
				ms.Position = 0;
				using (StreamReader sr = new StreamReader(ms)) {
					try {
						return sr.ReadToEnd();
					} finally {
						sr.Close();
					}
				}
			}
		}

		public void Put(string AUrl, Stream ASource, Stream AResponseContent) {
			DoRequest(HttpCommandEnum.Put, AUrl, ASource, AResponseContent);
		}
		public virtual CompressorBase Compressor {
			get {
				return _Compressor;
			}
			set {
				_Compressor = value;
			}
		}

		public int ResponseCode {
			get {
				return -1;
			}
		}

		public string ResponseText {
			get {
				return _HttpProto.Response.ResponseText;
			}
		}

		public HttpClientResponseInfo Response {
			get {
				return _HttpProto.Response;
			}
		}

		public URI Url {
			get {
				return _Url;
			}
		}

		public virtual int AuthRetries {
			get {
				return _AuthRetries;
			}
			set {
				_AuthRetries = value;
			}
		}

		public virtual int AuthProxyRetries {
			get {
				return _AuthProxyRetries;
			}
			set {
				_AuthProxyRetries = value;
			}
		}

		public virtual int MaxAuthRetries {
			get {
				return _MaxAuthRetries;
			}
			set {
				_MaxAuthRetries = value;
			}
		}

		public virtual bool AllowCookies {
			get {
				return _AllowCookies;
			}
			set {
				_AllowCookies = value;
			}
		}

		public virtual bool HandleRedirects {
			get {
				return _HandleRedirects;
			}
			set {
				_HandleRedirects = value;
			}
		}

		public virtual HttpProtocolVersionEnum ProtocolVersion {
			get {
				return _ProtocolVersion;
			}
			set {
				_ProtocolVersion = value;
			}
		}

		public virtual int RedirectMaximum {
			get {
				return _RedirectMaximum;
			}
			set {
				_RedirectMaximum = value;
			}
		}

		public virtual int MaxHeaderLines {
			get {
				return _MaxHeaderLines;
			}
			set {
				_MaxHeaderLines = value;
			}
		}

		public virtual ProxyConnectionInfo ProxyParams {
			get {
				return _ProxyParams;
			}
			set {
				_ProxyParams = value;
			}
		}

		public virtual HttpClientRequestInfo Request {
			get {
				return _HttpProto.Request;
			}
			set {
				_HttpProto.SetRequest(value);
			}
		}

		public virtual HttpOptionsEnum HttpOptions {
			get {
				return _HttpOptions;
			}
			set {
				_HttpOptions = value;
			}
		}

		public virtual event HttpOnRedirectEventHandler OnRedirect {
			add {
				_OnRedirect += value;
			}
			remove {
				_OnRedirect -= value;
			}
		}

		public virtual event HttpOnSelectAuthorizationEventHandler OnSelectAuthorization {
			add {
				_OnSelectAuthorization += value;
			}
			remove {
				_OnSelectAuthorization -= value;
			}
		}

		public virtual event HttpOnSelectAuthorizationEventHandler OnSelectProxyAuthorization {
			add {
				_OnSelectProxyAuthorization += value;
			}
			remove {
				_OnSelectProxyAuthorization -= value;
			}
		}

		public virtual event HttpOnAuthorizationEventHandler OnAuthorization {
			add {
				_OnAuthorization += value;
			}
			remove {
				_OnAuthorization -= value;
			}
		}

		public virtual event HttpOnAuthorizationEventHandler OnProxyAuthorization {
			add {
				_OnProxyAuthorization += value;
			}
			remove {
				_OnProxyAuthorization -= value;
			}
		}

		public virtual CookieManager CookieManager {
			get {
				return _CookieManager;
			}
			set {
				_CookieManager = value;
			}
		}

		public virtual AuthenticationManager AuthenticationManager {
			get {
				return _AuthenticationManager;
			}
			set {
				_AuthenticationManager = value;
			}
		}
	}
}