using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class Reply<TCode> {
		private TCode mCode = default(TCode);
		private Encoding mEncoding = Encoding.ASCII;
		protected Reply() {
		}

		protected abstract bool IsCodeValid(TCode code);

		public abstract void ReadFromSocket(Socket socket);
		public virtual void Clear() {
			mCode = default(TCode);
		}

		public abstract void ThrowReplyError();

		public TCode Code {
			get {
				return mCode;
			}
			set {
				if (IsCodeValid(value)) {
					mCode = value;
				} else {
					throw new Exception("Code is invalid");
				}
			}
		}

		public abstract byte[] FormattedReply {
			get;
			set;
		}

		public Encoding Encoding {
			get {
				return mEncoding;
			}
			set {
				mEncoding = value;
			}
		}
	}
}
