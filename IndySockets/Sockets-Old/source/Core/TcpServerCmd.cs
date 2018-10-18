using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public delegate void CommandHandlersExceptionEventHandler<TCommand, TContext>(TCommand command, TContext AContext, Exception exception);
	public delegate void CommandHandlerEventHandler<TCommandHandlerList, TContext>(TCommandHandlerList ASender, TContext AContext);
	public delegate void CommandEventHandler<TCommand>(TCommand ASender);
	public delegate void CommandHandlersUnhandledCommandEvent<TContext>(TContext context, byte[] command);

	/// <summary>
	/// TCP server with support for command handlers.
	/// </summary>
	/// <typeparam name="TReplyCode">The type of reply code used by the specified <typeparamref name="TReply"/>.</typeparam>
	/// <typeparam name="TReply">The type of reply used by the specified <typeparamref name="TContext"/></typeparam>
	/// <typeparam name="TContext">The type of contexts used by this <see cref="TcpServerCmd{TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand}"/> instance.</typeparam>
	/// <typeparam name="TCommandHandlerList">The type of <see cref="CommandHandlerList{TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand}"/> to use.</typeparam>
	/// <typeparam name="TCommandHandler">The type of <see cref="CommandHandler{TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand}"/> to use.</typeparam>
	/// <typeparam name="TCommand">The type of <see cref="Command{TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand}"/> to use.</typeparam>
	public partial class TcpServerCmd<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>:
		TcpServerBase<TReplyCode, TReply, TContext>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new()
		where TCommandHandlerList: CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommandHandler: CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommand: Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new() {

		private TCommandHandlerList mCommandHandlers = new TCommandHandlerList();
		private bool mCommandHandlersInitialized = false;

		/// <summary>
		/// Handles the client connection.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">The event args.</param>
		protected override void DoExecute(object sender, ContextRunEventArgs<TReplyCode, TReply, TContext> eventArgs) {
			mCommandHandlers.HandleCommand(eventArgs.Context);
			eventArgs.ReturnValue = eventArgs.Context.TcpConnection.Connected();
		}

		/// <summary>
		/// Starts up the server. If no <see cref="ServerSocket"/> specified, a <see cref="ServerSocketTcp"/> is created.
		/// Is no <see cref="Scheduler"/> specified, a <see cref="SchedulerOfThreadDefault"/> is created.
		/// </summary>
		public override void Open() {
			base.Open();
			if (!mCommandHandlersInitialized) {
				mCommandHandlersInitialized = true;
				InitializeCommandHandlers();
			}
		}

		/// <summary>
		/// Initializes the command handlers.
		/// </summary>
		protected virtual void InitializeCommandHandlers() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TcpServerCmd{TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand}"/> class.
		/// </summary>
		public TcpServerCmd() {
			mCommandHandlers.OnCommandHandlersException += DoCommandHandlersException;
		}

		/// <summary>
		/// Does the command handlers exception.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="AContext">The A context.</param>
		/// <param name="exception">The exception.</param>
		private void DoCommandHandlersException(TCommand command, TContext AContext, Exception exception) {
			this.DoContextException(AContext, exception);
		}

		/// <summary>
		/// Gets the command handlers of this server.
		/// </summary>
		/// <value>The command handlers.</value>
		public TCommandHandlerList CommandHandlers {
			get {
				return mCommandHandlers;
			}
		}
	}
}