using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core {
	public class CommandHandlerText<TContext>:
		CommandHandler<int, ReplyRFC, TContext, CommandHandlerListText<TContext>, CommandHandlerText<TContext>, CommandText<TContext>>
		where TContext: Context<int, ReplyRFC, TContext>, new() {

		private string mCmdDelimiter = " ";
		private string mCommand;
		private TextCommandEventHandler<TContext> mOnCommand;
		private string mParamDelimiter = " ";
		private bool mParseParams = true;
		private List<string> mResponse = new List<string>();
		protected bool mAppendExceptionToReply;

		public byte[] CommandBytes {
			get {
				return Encoding.ASCII.GetBytes(Command.ToUpperInvariant());
			}
		}

		public bool CommandBytesIs(byte[] checkBytes) {
			byte[] commandBytes = CommandBytes;
			if (checkBytes.Length != commandBytes.Length) {
				return false;
			}
			for (int i = 0; i < commandBytes.Length; i++) {
				if (commandBytes[i] != checkBytes[i]) {
					return false;
				}
			}
			return true;
		}

		public override void DoCommand(CommandText<TContext> command) {
			if (mParseParams) {
				if (command.Context.TcpConnection.Socket.InputBuffer.IndexOf(ParamDelimiter) == 0) {
					command.Context.TcpConnection.Socket.ReadTo(Encoding.ASCII.GetBytes(ParamDelimiter));
				}
				string commandLine = command.Context.TcpConnection.Socket.ReadLn();
				command.RawLineSet = String.Format("{0}{1}{2}",
					Command, CmdDelimiter, commandLine);
				command.UnparsedParamsSet = commandLine;
				command.Params.Clear();
				List<string> TempStrings = new List<string>(commandLine.Split(new string[] { ParamDelimiter }, StringSplitOptions.None));
				foreach (string s in TempStrings) {
					command.Params.Add(s.TrimEnd('\r', '\n'));
				}
			}
			if (mOnCommand != null) {
				mOnCommand(command);
			}
		}

		protected override void AfterRun(CommandText<TContext> command) {
			base.AfterRun(command);
			if (command.Response.Count > 0
				|| command.SendEmptyResponse) {
				command.Context.TcpConnection.WriteRFCStrings(command.Response);
			} else {
				if (command.Response.Count > 0) {
					command.Context.TcpConnection.WriteRFCStrings(command.Response);
				}
			}
		}

		public string CmdDelimiter {
			get {
				return mCmdDelimiter;
			}
			set {
				mCmdDelimiter = value;
			}
		}

		public string Command {
			get {
				return mCommand;
			}
			set {
				mCommand = value;
			}
		}

		public string ParamDelimiter {
			get {
				return mParamDelimiter;
			}
			set {
				mParamDelimiter = value;
			}
		}

		public bool ParseParams {
			get {
				return mParseParams;
			}
			set {
				mParseParams = value;
			}
		}

		public List<string> Response {
			get {
				return mResponse;
			}
		}

		public event TextCommandEventHandler<TContext> OnCommand {
			add {
				mOnCommand += value;
			}
			remove {
				mOnCommand -= value;
			}
		}
	}
}
