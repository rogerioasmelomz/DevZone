using System;
using System.Collections.Generic;
using System.Text;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core.TcpClient {
	public class SimpleReply<TCode>: Reply<TCode> {
		public override void ReadFromSocket(Socket socket) {
			FormattedReply = socket.ReadTo(Encoding.ASCII.GetBytes("-END\r"));
		}

		public override void ThrowReplyError() {
			throw new Exception("Reply Error");
		}

		public string TextMessage;

		protected override bool IsCodeValid(TCode code) {
			return true;
		}

		public override byte[] FormattedReply {
			get {
				StringBuilder sb = new StringBuilder();
				sb.Append("-BEGIN\r");
				sb.AppendFormat("CODE=" + Code.ToString());
				sb.Append("\r\t" + TextMessage);
				sb.Append("\r-END\r");
				return Encoding.ASCII.GetBytes(sb.ToString());
			}
			set {
				string TempString = Encoding.ASCII.GetString(value);
				TempString = TempString.Substring(12);
				string theCode = TempString.Substring(0, TempString.IndexOf("\r"));
				this.Code = (TCode)Convert.ChangeType(theCode, typeof(TCode));

				TempString = TempString.Remove(0, TempString.IndexOf("\r") + 1);
				TempString = TempString.Substring(0, TempString.Length - 6);
				TextMessage = TempString;
			}
		}
	}
}