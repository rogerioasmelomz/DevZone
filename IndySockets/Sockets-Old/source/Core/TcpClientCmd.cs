using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Indy.Sockets.Core {
	public class TcpClientCmd<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>:
		TcpClient<TReplyCode, TReply>
		where TContext: Context<TReplyCode, TReply, TContext>, new()
		where TReplyCode: IEquatable<TReplyCode>
		where TReply: Reply<TReplyCode>, new()
		where TCommandHandlerList: CommandHandlerList<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommandHandler: CommandHandler<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new()
		where TCommand: Command<TReplyCode, TReply, TContext, TCommandHandlerList, TCommandHandler, TCommand>, new() {

		private TReply mExceptionReply = new TReply();
		private TCommandHandlerList mCommandHandlers = new TCommandHandlerList();
		private Thread mListenThread;

		private void ListenThreadMethod() {
			TContext xContext = new TContext();
			xContext.TcpConnection = this;
			while (Connected()) {
				Socket.CheckForDataOnSource();
				Socket.CheckForDisconnect();
				mCommandHandlers.HandleCommand(xContext);
				Thread.Sleep(5);
			}
		}

		public override void Connect(Socket aSocket) {
			base.Connect(aSocket);
			try {
				mListenThread = new Thread(ListenThreadMethod);
				mListenThread.IsBackground = true;
			} catch (Exception E) {
				Disconnect(true);
				throw new IndyException("Error while creating listening thread", E);
			}
		}

		public override void Disconnect(bool ANotifyPeer) {
			if (mListenThread!= null){
				mListenThread.Abort();}
			base.Disconnect(ANotifyPeer);
			if (mListenThread != null) {
				mListenThread.Join();
				mListenThread = null;
			}
		}
	}
}