using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public abstract class CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>: List<TCommandHandler>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new()
		where TCommandHandlerList: CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommandHandler: CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommand: Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new() {

		public CommandHandlerList() {
		}

		private CommandHandlersUnhandledCommandEvent<TContext> mUnhandledCommand;

		/// <summary>
		/// Occurs when a command is not handled.
		/// </summary>
		public event CommandHandlersUnhandledCommandEvent<TContext> UnhandledCommand {
			add {
				mUnhandledCommand += value;
			}
			remove {
				mUnhandledCommand -= value;
			}
		}

		private CommandHandlerEventHandler<TCommandHandlerList, TContext> mOnAfterCommandHandler;

		/// <summary>
		/// Occurs after a <see cref="CommandHandler"/> has been ran.
		/// </summary>
		public event CommandHandlerEventHandler<TCommandHandlerList, TContext> OnAfterCommandHandler {
			add {
				mOnAfterCommandHandler += value;
			}
			remove {
				mOnAfterCommandHandler -= value;
			}
		}

		private CommandHandlersExceptionEventHandler<TCommand, TContext> mOnCommandHandlersException;

		/// <summary>
		/// Occurs when an unhandled exception occurs during execution of a <see cref="CommandHandler"/>.
		/// </summary>
		public event CommandHandlersExceptionEventHandler<TCommand, TContext> OnCommandHandlersException {
			add {
				mOnCommandHandlersException += value;
			}
			remove {
				mOnCommandHandlersException -= value;
			}
		}

		private CommandHandlerEventHandler<TCommandHandlerList, TContext> mOnBeforeCommandHandler;

		/// <summary>
		/// Occurs before a <see cref="CommandHandler"/> gets executed.
		/// </summary>
		public event CommandHandlerEventHandler<TCommandHandlerList, TContext> OnBeforeCommandHandler {
			add {
				mOnBeforeCommandHandler += value;
			}
			remove {
				mOnBeforeCommandHandler -= value;
			}
		}

		/// <summary>
		/// Handles the command, if possible.
		/// </summary>
		/// <param name="AContext">The context in which the current code is running.</param>
		/// <returns><see langword="true"/> if handled, <see langword="false"/> if not.</returns>
		public abstract bool HandleCommand(TContext AContext);

		/// <summary>
		/// Handles command handler exceptions.
		/// </summary>
		/// <param name="command">The command on which the exception occurred.</param>
		/// <param name="AContext">The context on which the exception happened.</param>
		/// <param name="exception">The exception.</param>
		protected virtual void CommandHandler_OnException(TCommand command, TContext AContext, Exception exception) {
			if (mOnCommandHandlersException != null) {
				mOnCommandHandlersException(command, AContext, exception);
			}
		}

		/// <summary>
		/// Gets called when a command is not handled.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="command">The command.</param>
		protected virtual void DoUnhandledCommand(TContext context, byte[] command) {
			if (mUnhandledCommand != null) {
				mUnhandledCommand(context, command);
			}
		}

		/// <summary>
		/// Gets called before a command handler gets ran.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void DoBeforeCommandHandler(TContext context) {
			if (mOnBeforeCommandHandler != null) {
				mOnBeforeCommandHandler(typedThis, context);
			}
		}

		private TCommandHandlerList typedThis {
			get {
				return (TCommandHandlerList)this;
			}
		}

		/// <summary>
		/// Gets called after a command handler has been ran.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void DoAfterCommandHandler(TContext context) {
			if (mOnAfterCommandHandler != null) {
				mOnAfterCommandHandler(typedThis, context);
			}
		}
	}
}