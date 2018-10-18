using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Indy.Sockets.Protocols.SASL {
	public class SASLCRAMMD5: SASLUserPassMechanism {
		public override string StartAuthenticate(System.Text.Encoding encoding, string challenge) {
			if (challenge.Length > 0) {
				return UserName + " " + BuildKeyedMD5Auth(encoding, Password, challenge);
			} else {
				return "";
			}
		}

		private static string BuildKeyedMD5Auth(System.Text.Encoding encoding, string password, string challenge) {
			using (HMAC hasher = new HMACMD5(encoding.GetBytes(password))) {
				return encoding.GetString(hasher.ComputeHash(encoding.GetBytes(challenge)));
			}
		}

		public override string ContinueAuthenticate(System.Text.Encoding encoding, string lastResponse) {
			return "";
		}

		public override void FinishAuthenticate() {
		}

		public override bool IsAuthProtocolAvailable(IList<string> featStrings) {
			return (featStrings != null) && featStrings.Contains(ServiceName);
		}

		public override string ServiceName {
			get {
				return "CRAM-MD5";
			}
		}

		public override int SecurityLevel {
			get {
				return 0;
			}
		}
	}
}
