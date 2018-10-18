using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core.TcpClient
{
  [TestFixture]
  public class SimpleTcpClientTest
  {
    [Test(Description = "Connects and sends hello, and retrieves hello")]
    public void SimpleConnectAndEcho()
    {
      TcpClient<int, SimpleReply<int>> tempClient = new TcpClient<int, SimpleReply<int>>();
      tempClient.Connect(TestUtilities.GetSimulationSocket(typeof(TcpClient<int, SimpleReply<int>>), "SimpleConnectAndEcho"));
      Assert.IsTrue(tempClient.Connected(), "Not connected");
      tempClient.Socket.WriteLn("Hello, World!");
      Assert.IsTrue(String.Equals("Hello, World!", tempClient.Socket.ReadLn()), "Server responded wrong");
      tempClient.Disconnect();
      Assert.IsFalse(tempClient.Connected(), "Still connected");
    }

    [Test(Description = "Tests the SendCmd/GetResponse structure")]
    public void TestResponseReplyStructure()
    {
      TcpClient<int, SimpleReply<int>> tempClient = new TcpClient<int, SimpleReply<int>>();
      Socket theSocket = TestUtilities.GetSimulationSocket(typeof(TcpClient<int, SimpleReply<int>>), "TestResponseReplyStructure");
      tempClient.Connect(theSocket);
      Assert.IsTrue(tempClient.Connected(), "Not connected");
      Assert.AreEqual(1, tempClient.SendCmd("This is a sample message"), "First response is not fine");
      Assert.AreEqual(2, tempClient.SendCmd("Message 2"), "Second response");
      tempClient.Disconnect();
      Assert.IsFalse(tempClient.Connected(), "Still Connected");
    }
  }
}