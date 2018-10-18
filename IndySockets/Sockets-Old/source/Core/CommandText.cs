using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public class CommandText<TContext>:
		Command<int, ReplyRFC, TContext, CommandHandlerListText<TContext>, CommandHandlerText<TContext>, CommandText<TContext>>
		where TContext: Context<int, ReplyRFC, TContext>, new() {
		private List<string> mParams = new List<string>();
		private string mRawLine;
		private List<string> mResponse = new List<string>();
		private string mUnparsedParams;
		private bool mSendEmptyResponse = false;

		public List<string> Params {
			get {
				return mParams;
			}
		}

		public string RawLine {
			get {
				return mRawLine;
			}
		}

		internal string RawLineSet {
			set {
				mRawLine = value;
			}
		}

		public List<string> Response {
			get {
				return mResponse;
			}
		}

		public string UnparsedParams {
			get {
				return mUnparsedParams;
			}
		}

		internal string UnparsedParamsSet {
			set {
				mUnparsedParams = value;
			}
		}

		public bool SendEmptyResponse {
			get {
				return mSendEmptyResponse;
			}
		}
	}
}
