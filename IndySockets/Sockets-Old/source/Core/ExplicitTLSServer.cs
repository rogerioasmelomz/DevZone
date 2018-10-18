using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class ExplicitTLSServer<TReplyCode, TReply, TContext>: TcpServerBase<TReplyCode, TReply, TContext>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new() {

		protected int mRegularProtPort;
		protected int mImplicitTLSProtPort;
		private UseTLSEnum mUseTLS = UseTLSEnum.NoTLSSupport;

		public override ServerSocket ServerSocket {
			set {
				if (ServerSocket != value) {
					base.ServerSocket = value;
					if (!(value is ServerSocketTLS)) {
						UseTLS = UseTLSEnum.NoTLSSupport;
					}
				}
			}
		}

		protected UseTLSEnum UseTLS {
			get {
				return mUseTLS;
			}
			set {
				if (!Active) {
					if ((!(ServerSocket is ServerSocketTLS)) && (value != UseTLSEnum.NoTLSSupport)) {
						throw new IndyException("TLS-enabled ServerSocket needed!");
					}
					if (mUseTLS != value) {
						if (value == UseTLSEnum.UseImplicitTLS) {
							if (DefaultPort == mRegularProtPort) {
								DefaultPort = mImplicitTLSProtPort;
							}
						} else {
							if (DefaultPort == mImplicitTLSProtPort) {
								DefaultPort = mRegularProtPort;
							}
						}
						mUseTLS = value;
					}
				} else {
					throw new IndyException("Cannot set UseTLS when active!");
				}
			}
		}
	}
}