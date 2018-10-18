using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Indy.Sockets.Core;

namespace Indy.Sockets.Web {
	public sealed class HttpServer: CustomHttpServer {
		private List<IWebRequestHandler> mRequestHandlers = new List<IWebRequestHandler>();
		private ReaderWriterLock mRequestHandlersLock = new ReaderWriterLock();
		private HttpCommandEventHandler mOnRequestNotHandled;

		public override void Open() {
			base.Open();
			SessionList.OnSessionStart += new SessionEventHandler(SessionList_OnSessionStart);
			SessionList.OnSessionEnd += new SessionEventHandler(SessionList_OnSessionEnd);
		}

		void SessionList_OnSessionEnd(HttpSession Sender) {
			RequestHandlersLock.AcquireReaderLock(-1);
			try {
				for (int i = 0; i < mRequestHandlers.Count; i++) {
					mRequestHandlers[0].SessionEnd(Sender);
				}
			} finally {
				RequestHandlersLock.ReleaseReaderLock();
			}
		}

		void SessionList_OnSessionStart(HttpSession Sender) {
			RequestHandlersLock.AcquireReaderLock(-1);
			try {
				for (int i = 0; i < mRequestHandlers.Count; i++) {
					mRequestHandlers[0].SessionStart(Sender);
				}
			} finally {
				RequestHandlersLock.ReleaseReaderLock();
			}
		}

		protected override void DoRequestNotHandled(ContextRFC AContext, HttpRequestInfo ARequestInfo, HttpResponseInfo AResponseInfo) {
			bool LHandled = false;
			if (mOnRequestNotHandled != null) {
				mOnRequestNotHandled(AContext, ARequestInfo, AResponseInfo, ref LHandled);
			}
			if (!LHandled) {
				AResponseInfo.ContentText = "<html><body><h3>The server didn't handle your request.</h3></body></html>\r\n";
				AResponseInfo.ContentType = "text/html";
			}
		}

		protected override void DoCommand(ContextRFC context, HttpRequestInfo request, HttpResponseInfo response, ref bool handled) {
			RequestHandlersLock.AcquireReaderLock(-1);
			try {
				for (int i = 0; i < mRequestHandlers.Count && !handled; i++) {
					mRequestHandlers[0].HandleCommand(request, response, ref handled);
				}
			} finally {
				RequestHandlersLock.ReleaseReaderLock();
			}
		}
		public List<IWebRequestHandler> RequestHandlers {
			get {
				return mRequestHandlers;
			}
		}

		public ReaderWriterLock RequestHandlersLock {
			get {
				return mRequestHandlersLock;
			}
		}

		public event HttpCommandEventHandler OnRequestNotHandled {
			add {
				mOnRequestNotHandled += value;
			}
			remove {
				mOnRequestNotHandled -= value;
			}
		}
	}
}
