using System;
using System.Collections.Generic;
using System.Text;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	public class LoopbackSocket: Socket {
		private bool mOpen = false;
		protected override int ReadFromSource(bool aThrowIfDisconnected, int aTimeOut, bool AThrowIfTimeout) {
			if (AThrowIfTimeout) {
				throw new Exception("Timeout");
			}
			return -1;
		}

		public override void Close() {
			mOpen = false;
			base.Close();
		}

		public override void Open() {
			base.Open();
			mOpen = true;
		}

		public override bool Connected() {
			return base.Connected() && mOpen;
		}

		public override void CheckForDisconnect(bool throwExceptionIfDisconnected, bool ignoreBuffer) {
			//
		}

		public override void CheckForDataOnSource(int aTimeOut) {
			//
		}

		public override void Transmit(ref byte[] data) {
			base.Transmit(ref data);
			InputBuffer.Write(data);
		}

		public override bool IsOpen() {
			return true;
		}
	}
}
