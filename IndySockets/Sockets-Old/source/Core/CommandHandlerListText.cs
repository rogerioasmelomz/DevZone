using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public class CommandHandlerListText<TContext>:
		CommandHandlerList<int, ReplyRFC, TContext, CommandHandlerListText<TContext>, CommandHandlerText<TContext>, CommandText<TContext>>
		where TContext: Context<int, ReplyRFC, TContext>, new() {

		public const bool Default_AppendExceptionToReply = true;
		private bool mAppendExceptionToReply = Default_AppendExceptionToReply;
		protected int IndexOfCommandEnd(Buffer buffer) {
			if (buffer.Size == 0) {
				return -1;
			}
			System.Collections.Specialized.StringCollection commandDelimiters = new System.Collections.Specialized.StringCollection();
			commandDelimiters.AddRange(new string[] { "\r\n", "\n", "\r" });
			for (int i = 0; i < this.Count; i++) {
				CommandHandlerText<TContext> tch = this[i];
				if (tch.Enabled) {
					if (!commandDelimiters.Contains(tch.CmdDelimiter)) {
						commandDelimiters.Add(tch.CmdDelimiter);
					}
				}
			}

			int TempResult = -1;
			do {
				TempResult = buffer.IndexOf(commandDelimiters[0]);
				commandDelimiters.RemoveAt(0);
			} while (TempResult == -1
				&& commandDelimiters.Count > 0);
			while (commandDelimiters.Count > 0) {
				int TempInt2 = buffer.IndexOf(commandDelimiters[0]);
				if (TempInt2 > -1) {
					TempResult = Math.Min(TempResult, TempInt2);
				}
				commandDelimiters.RemoveAt(0);
			}
			return TempResult;
		}

		protected override void DoAfterCommandHandler(TContext AContext) {
			base.DoAfterCommandHandler(AContext);
			if (AContext.TcpConnection.Socket.InputBuffer.PeekByte(0) == 13) {
				AContext.TcpConnection.Socket.InputBuffer.Remove(0);
			}
			if (AContext.TcpConnection.Socket.InputBuffer.PeekByte(0) == 10) {
				AContext.TcpConnection.Socket.InputBuffer.Remove(0);
			}
		}

		public override bool HandleCommand(TContext AContext) {
			if (Count > 0) {
				int commandEnd = IndexOfCommandEnd(AContext.TcpConnection.Socket.InputBuffer);
				if (commandEnd == -1) {
					AContext.TcpConnection.Socket.CheckForDataOnSource(Global.InfiniteTimeOut);
					AContext.TcpConnection.Socket.CheckForDisconnect(true, true);
					commandEnd = IndexOfCommandEnd(AContext.TcpConnection.Socket.InputBuffer);
				}
				if (commandEnd != -1) {
					byte[] command = AContext.TcpConnection.Socket.ReadBytes(commandEnd);
					command = FilterCommand(command);
					DoBeforeCommandHandler(AContext);
					try {
						foreach (CommandHandlerText<TContext> handler in this) {
							if (!handler.Enabled) {
								continue;
							}
							if (handler.CommandBytesIs(command)) {
								handler.OnException += CommandHandler_OnException;
								try {
									handler.DoCommand(AContext, command);
								} finally {
									handler.OnException -= CommandHandler_OnException;
								}
								return true;
							}
						}
					} finally {
						DoAfterCommandHandler(AContext);
					}
					DoUnhandledCommand(AContext, command);
					return true;
				}
			}
			return false;
		}

		protected byte[] FilterCommand(byte[] command) {
			string CommandStr = Encoding.ASCII.GetString(command);
			return Encoding.ASCII.GetBytes(CommandStr.ToUpperInvariant());
		}

		protected override void CommandHandler_OnException(CommandText<TContext> command, TContext AContext, Exception exception) {
			base.CommandHandler_OnException(command, AContext, exception);
			if (mAppendExceptionToReply) {
				command.Response.Add(exception.ToString());
			}
		}

		protected override void DoUnhandledCommand(TContext context, byte[] command) {
			base.DoUnhandledCommand(context, command);
			context.TcpConnection.Socket.ReadBytes(1);
			if (context.TcpConnection.Socket.InputBuffer.IndexOf("\n") == 0) {
				context.TcpConnection.Socket.ReadBytes(1);
			}
		}
	}
}
