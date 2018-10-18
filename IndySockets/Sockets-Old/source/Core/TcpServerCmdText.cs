using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public delegate void TextCommandEventHandler<TContext>(CommandText<TContext> command)
			where TContext: Context<int, ReplyRFC, TContext>, new();
	public class TcpServerCmdText<TContext>:
		TcpServerCmd
		<int,
			ReplyRFC,
			TContext,
			CommandHandlerListText<TContext>,
			CommandHandlerText<TContext>,
			CommandText<TContext>>
		where TContext: Context<int, ReplyRFC, TContext>, new() {

		private ReplyRFC mExceptionReply = new ReplyRFC();

		public TcpServerCmdText() {
			mExceptionReply.Encoding = base.Encoding;
			this.CommandHandlers.UnhandledCommand += HandleUnhandledCommand;
		}

		private void HandleUnhandledCommand(TContext context, byte[] command) {
			ReplyRFC r = new ReplyRFC();
			r.Encoding = Encoding;
			r.FormattedReply = mExceptionReply.FormattedReply;
			r.Text.Clear();
			r.Text.Add("Unhandled command: " + Encoding.ASCII.GetString(command));
			context.TcpConnection.Socket.Write(r.FormattedReply);
		}

		public ReplyRFC ExceptionReply {
			get {
				return mExceptionReply;
			}
		}
	}
}