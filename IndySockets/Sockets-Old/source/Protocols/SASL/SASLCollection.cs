using System;
using System.Collections.Generic;
using System.Text;

using Indy.Sockets.Core;
using Indy.Sockets.Protocols.Encoding;

namespace Indy.Sockets.Protocols.SASL {
	public class SASLCollection<TReplyCode, TReply>: List<ISASLMechanism>
		where TReply: Reply<TReplyCode>, new()
		where TReplyCode: IEquatable<TReplyCode> {
		private System.Text.Encoding mEncoding = System.Text.Encoding.ASCII;

		private static bool CheckStrFail(TReplyCode str, TReplyCode[] ok, TReplyCode[] cont) {
			return (Array.IndexOf(ok, str) == -1)
				 && (Array.IndexOf(cont, str) == -1);
		}

		private static bool PerformSASLLogin(byte[] cmd, ISASLMechanism saslMechanism,
			EncoderBase encoderBase64, DecoderBase decoderBase64, TReplyCode[] okReplies, TReplyCode[] continueReplies,
			ISASLImplementor<TReplyCode, TReply> client, System.Text.Encoding encoding) {

			client.SendCmd(cmd, encoding.GetBytes(saslMechanism.ServiceName));
			if (CheckStrFail(client.LastCmdResultCode, okReplies, continueReplies)) {
				return false;
			}
			if (Array.IndexOf(okReplies, client.LastCmdResultCode) > -1) {
				// we've authenticated successfully
				return true;
			}
			string s = decoderBase64.DecodeString(encoding.GetString(client.LastCmdResultContent).TrimEnd());
			s = saslMechanism.StartAuthenticate(encoding, s);
			client.SendCmd(encoding.GetBytes(encoderBase64.Encode(s)), new byte[0]);
			if (CheckStrFail(client.LastCmdResultCode, okReplies, continueReplies)) {
				saslMechanism.FinishAuthenticate();
				return false;
			}
			while (Array.IndexOf(continueReplies, client.LastCmdResultCode) > -1) {
				s = decoderBase64.DecodeString(encoding.GetString(client.LastCmdResultContent).TrimEnd());
				s = saslMechanism.ContinueAuthenticate(encoding, s);
				client.SendCmd(encoding.GetBytes(encoderBase64.Encode(s)), new byte[0]);
				if (CheckStrFail(client.LastCmdResultCode, okReplies, continueReplies)) {
					saslMechanism.FinishAuthenticate();
					return false;
				}
			}
			saslMechanism.FinishAuthenticate();
			return Array.IndexOf(okReplies, client.LastCmdResultCode) > -1;
		}

		public bool Login(byte[] cmd, TReplyCode[] okReplies, TReplyCode[] continueReplies,
					ISASLImplementor<TReplyCode, TReply> client, IList<string> capaReply, string authString) {
			IList<string> xSupportedSASL = ParseCapaReply(capaReply, authString);
			List<ISASLMechanism> xSASLList = new List<ISASLMechanism>();
			if (xSupportedSASL == null) {
				xSupportedSASL = new string[0];
			}
			foreach (ISASLMechanism s in this) {
				if (!s.IsAuthProtocolAvailable(xSupportedSASL)) {
					continue;
				}
				xSASLList.Add(s);
			}
			if (xSASLList.Count > 0) {
				EncoderBase64 LE = new EncoderBase64();
				DecoderBase64 LD = new DecoderBase64();
				foreach (ISASLMechanism m in xSASLList) {
					if (PerformSASLLogin(cmd, m, LE, LD, okReplies, continueReplies, client, mEncoding)) {
						return true;
					}
				}
			}
			return false;
		}

		public bool Login(byte[] cmd, string serviceName, TReplyCode[] okReplies,
					TReplyCode[] continueReplies, ISASLImplementor<TReplyCode, TReply> client, IList<string> capaReply, string authString) {
			IList<string> xSupportedSASL = ParseCapaReply(capaReply, authString);
			if (xSupportedSASL != null) {
				xSupportedSASL = new List<string>();
			}
			if (xSupportedSASL.IndexOf(serviceName) == -1) {
				return false;
			}
			ISASLMechanism xSASL = Find(serviceName);
			if (xSASL != null) {
				EncoderBase64 xE = new EncoderBase64();
				DecoderBase64 xD = new DecoderBase64();
				bool TempResult = PerformSASLLogin(cmd, xSASL, xE, xD, okReplies, continueReplies, client, mEncoding);
				xSupportedSASL.Clear();
				return TempResult;
				// was:
				//if not Result then begin
				//  AClient.RaiseExceptionForLastCmdResult;
				//end;
			}
			return false;
		}
		

		public IList<string> ParseCapaReply(IList<string> capaReply, string authString) {
			if (capaReply == null){return null;}
			List<string> tempResult = new List<string>();
			foreach (string item in capaReply) {
				string s = item.ToUpperInvariant();
				string xPrefix = s.Substring(0, authString.Length + 1);
				if (String.Equals(xPrefix, authString + " ", StringComparison.InvariantCultureIgnoreCase) ||
					String.Equals(xPrefix, authString + "=", StringComparison.InvariantCultureIgnoreCase)) {
					s = s.Substring(xPrefix.Length);
					s = s.Replace("=", " ");
					while (s.Length > 0) {
						string xEntry = Global.Fetch(ref s, " ");
						if (!String.IsNullOrEmpty(xEntry)) {
							if (!tempResult.Contains(xEntry)) {
								tempResult.Add(xEntry);
							}
						}
					}
				}
			}
			return tempResult;
		}

		public ISASLMechanism Find(string serviceName) {
			foreach (ISASLMechanism s in this) {
				if (String.Equals(s.ServiceName, serviceName, StringComparison.InvariantCulture)) {
					return s;
				}
			}
			return null;
		}

		public System.Text.Encoding Encoding {
			get {
				return mEncoding;
			}
			set {
				mEncoding = value;
			}
		}
	}
}
