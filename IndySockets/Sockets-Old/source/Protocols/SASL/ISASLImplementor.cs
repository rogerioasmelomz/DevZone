using System;
using System.Collections.Generic;
using System.Text;
using Indy.Sockets.Core;

namespace Indy.Sockets.Protocols.SASL {
	public interface ISASLImplementor<TReplyCode, TReply>
		where TReply: Reply<TReplyCode>, new()
		where TReplyCode: IEquatable<TReplyCode> {
		TReplyCode LastCmdResultCode {
			get;
		}

		byte[] LastCmdResultContent {
			get;
		}

		void SendCmd(byte[] cmd, byte[] param1);
	}
}
