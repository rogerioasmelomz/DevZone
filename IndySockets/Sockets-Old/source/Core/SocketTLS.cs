using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class SocketTLS: Socket {
		private bool mPassThrough = true;
		private bool mIsPeer;
		private string mURIToCheck;

		public virtual bool PassThrough {
			get {
				return mPassThrough;
			}
			set {
				mPassThrough = value;
			}
		}
	}
}