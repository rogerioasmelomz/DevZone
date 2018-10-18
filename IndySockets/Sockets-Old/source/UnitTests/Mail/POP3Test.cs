using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Indy.Sockets.Core;
using Indy.Sockets.Mail;

using Indy.Sockets.UnitTests.Core;

namespace Indy.Sockets.UnitTests.Mail {

  [TestFixture]
  [Category("NotWorking")]
  public class POP3Test {
  
    [Test]
    public void Connect() {
    
      POP3 xPOP = new POP3();
      Socket theSocket = TestUtilities.GetSimulationSocket(typeof(POP3), "SimpleConnect");
      xPOP.Connect(theSocket);
      Assert.IsTrue(xPOP.Socket.Connected());
      xPOP.Disconnect();
      Assert.IsFalse(xPOP.Socket.Connected());
    }
    
    [Test]  
    public void RetrieveRaw() {
      
      POP3 xPOP = new POP3();
      
      xPOP.Username = "indytest";
      xPOP.Password = "indytest";
      xPOP.Connect(TestUtilities.GetSimulationSocket(typeof(POP3), "RetrieveRaw"));      
      xPOP.Login();
      xPOP.RetrieveRaw(1);
      xPOP.Disconnect();
    }
    
    [Test]
    public void Login() {
      
      POP3 xPOP = new POP3();
      
      xPOP.Username = "indytest";
      xPOP.Password = "indytest";
      xPOP.Connect(TestUtilities.GetSimulationSocket(typeof(POP3), "Login"));
      xPOP.Login();
      xPOP.Disconnect();
    }
    
    [Test] 
    public void GetMessageCountTest() {
      
      POP3 xPOP = new POP3();

      xPOP.Username = "indytest";
      xPOP.Password = "indytest";
      xPOP.Connect(TestUtilities.GetSimulationSocket(typeof(POP3), "GetMessageCountTest"));
      xPOP.Login();
      Assert.AreEqual(3, xPOP.GetMessageCount());
      xPOP.Disconnect();
    }
    
  }
}
