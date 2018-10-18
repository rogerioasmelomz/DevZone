using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new()
		where TCommandHandlerList: CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommandHandler: CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommand: Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new() {
		protected Command() {
		}

		private TCommandHandler mCommandHandler;
		private bool mDisconnect;
		private bool mPerformReply = true;
		private TReply mReply = new TReply();
		private TContext mContext;

		internal TContext ContextSet {
			set {
				mContext = value;
			}
		}

		public void SendReply() {
			PerformReply = false;
			Context.TcpConnection.Socket.Write(Reply.FormattedReply);
		}

		public TContext Context {
			get {
				return mContext;
			}
		}

		public bool Disconnect {
			get {
				return mDisconnect;
			}
			set {
				mDisconnect = value;
			}
		}

		public bool PerformReply {
			get {
				return mPerformReply;
			}
			set {
				mPerformReply = value;
			}
		}

		public TReply Reply {
			get {
				return mReply;
			}
		}

		internal TCommandHandler CommandHandlerSet {
			set {
				mCommandHandler = value;
			}
		}

		public TCommandHandler CommandHandler {
			get {
				return mCommandHandler;
			}
		}
	}
}