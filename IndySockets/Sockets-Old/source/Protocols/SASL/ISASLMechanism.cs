using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Protocols.SASL {
	public interface ISASLMechanism {
		string StartAuthenticate(System.Text.Encoding encoding, string challenge);
		string ContinueAuthenticate(System.Text.Encoding encoding, string lastResponse);
		void FinishAuthenticate();
		bool IsAuthProtocolAvailable(IList<string> featStrings);
		string ServiceName {
			get;
		}
		int SecurityLevel {
			get;
		}
	}
}
