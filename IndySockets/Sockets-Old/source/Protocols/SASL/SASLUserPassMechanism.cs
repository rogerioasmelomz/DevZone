using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Protocols.SASL {
	public abstract class SASLUserPassMechanism: ISASLMechanism {
		private IUserPassProvider mUserPassProvider;
		public abstract string StartAuthenticate(System.Text.Encoding encoding, string challenge);
		public abstract string ContinueAuthenticate(System.Text.Encoding encoding, string lastResponse);
		public abstract void FinishAuthenticate();
		public abstract bool IsAuthProtocolAvailable(IList<string> featStrings);
		public abstract string ServiceName {
			get;
		}

		public abstract int SecurityLevel {
			get;
		}

		protected string UserName {
			get {
				if (mUserPassProvider != null) {
					return mUserPassProvider.UserName;
				}
				throw new Core.IndyException("No UserPassProvider specified!");
			}
		}

		protected string Password {
			get {
				if (mUserPassProvider != null) {
					return mUserPassProvider.Password;
				}
				throw new Core.IndyException("No UserPassProvider specified!");
			}
		}

		public IUserPassProvider UserPassProvider {
			get {
				return mUserPassProvider;
			}
			set {
				mUserPassProvider = value;
			}
		}
	}
}
