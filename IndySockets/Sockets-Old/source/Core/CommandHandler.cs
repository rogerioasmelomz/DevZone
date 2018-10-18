using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new()
		where TCommandHandlerList: CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommandHandler: CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommand: Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new() {
		public CommandHandler() {
		}

		protected TCommandHandlerList mCollection;
		protected object mData;
		protected List<string> mDescription = new List<string>();
		protected bool mDisconnect;
		protected bool mEnabled = true;
		protected TReply mExceptionReply = new TReply();
		protected string mHelpSuperScript = "";
		protected bool mHelpVisible;
		protected TReply mNormalReply = new TReply();

		private TCommandHandler typedThis {
			get {
				return (TCommandHandler)this;
			}
		}

		protected virtual void AfterRun(TCommand command) {
		}

		public abstract void DoCommand(TCommand command);
		public virtual void DoCommand(TContext context, byte[] command) {
			TCommand LCommand = new TCommand();
			try {
				try {
					LCommand.Disconnect = this.Disconnect;
					LCommand.CommandHandlerSet = typedThis;
					LCommand.ContextSet = context;
					LCommand.Reply.FormattedReply = NormalReply.FormattedReply;
					DoCommand(LCommand);
				} catch (Exception E) {
					if (LCommand.PerformReply) {
						if (!ExceptionReply.Code.Equals(default(TReplyCode))) {
							LCommand.Reply.FormattedReply = this.ExceptionReply.FormattedReply;
						}
						//            if (!LCommand.Reply.Equals(default(TReplyCode)))
						//            {
						//              LCommand.Reply.Text.Add(E.Message);
						//              LCommand.SendReply();
						//            }
						//            else
						//            {
						//            }
					}
					OnException(LCommand, context, E);
				}
				if (LCommand.PerformReply) {
					LCommand.SendReply();
				}
				AfterRun(LCommand);
			} finally {
				if (LCommand.Disconnect) {
					context.TcpConnection.Disconnect();
				}
			}
		}

		internal event CommandHandlersExceptionEventHandler<TCommand, TContext> OnException;

		public string HelpSuperScript {
			get {
				return mHelpSuperScript;
			}
			set {
				mHelpSuperScript = value;
			}
		}

		public bool HelpVisible {
			get {
				return mHelpVisible;
			}
			set {
				mHelpVisible = value;
			}
		}

		public List<string> Description {
			get {
				return mDescription;
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

		public bool Enabled {
			get {
				return mEnabled;
			}
			set {
				mEnabled = value;
			}
		}

		public TReply ExceptionReply {
			get {
				return mExceptionReply;
			}
		}

		public TReply NormalReply {
			get {
				return mNormalReply;
			}
		}
	}
}