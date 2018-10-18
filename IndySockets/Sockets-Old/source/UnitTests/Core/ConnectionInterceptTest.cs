using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
  [TestFixture]
  public class ConnectionInterceptTest {

    private class TestConnectionInterceptImplementation1: ConnectionIntercept {
      private int mConnectCalled = 0;

      public int ConnectCalled {
        get {
          return mConnectCalled;
        }
      }
      private int mDisconnectCalled = 0;

      public int DisconnectCalled {
        get {
          return mDisconnectCalled;
        }
      }
      public override void Connect(object sender) {
        mConnectCalled += 1;
      }

      public override void Disconnect() {
        mDisconnectCalled += 1;
      }

      public override void Receive(ref byte[] buffer) {
      }

      public override void Send(ref byte[] buffer) {
      }
    }

    [Test(Description = "Tests whether Connect and Disconnect get called")]
    public void TestSimple() {
      TcpClient<int, ReplyRFC> tcpClient = new TcpClient<int,ReplyRFC>();
      tcpClient.Socket = TestUtilities.GetSimulationSocket(typeof(ConnectionIntercept), "Simple");
      TestConnectionInterceptImplementation1 intercept = new TestConnectionInterceptImplementation1();
      tcpClient.Socket.Intercept = intercept;
      tcpClient.Host = "non-existing.host.xyz";
      tcpClient.Port = -1;
      tcpClient.Connect(tcpClient.Socket);
      tcpClient.Disconnect();
      Assert.AreEqual(1, intercept.ConnectCalled, "Connect isn't called only once");
      Assert.AreEqual(1, intercept.DisconnectCalled, "Disconnect isn't called only once");
    }
  }
}