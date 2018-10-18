using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
  [TestFixture]
  public class SocketTest {
    [Test]
		[ExpectedException(typeof(ClosedSocketException))]
    public void TestReadWrite() {
			LoopbackSocket socket = new LoopbackSocket();
      socket.Open();
      try {
        Assert.AreEqual(0, socket.InputBuffer.Size, "1");
        socket.Write("Hello, World");
        Assert.AreEqual(12, socket.InputBuffer.Size, "2");
      } finally {
        socket.Close();
      }
			socket.Write("Hello, World!");
    }
  }
}
