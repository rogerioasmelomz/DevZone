using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core.TcpServerCmdText {
  [TestFixture]
  public class CustomContextTypeTest {
    public class OurCustomContext : Context<int, ReplyRFC, OurCustomContext> {
      public string OurValue = "Default";
    }

    private void HandleSET(CommandText<OurCustomContext> command) {
      command.Context.OurValue = command.UnparsedParams;
      Assert.IsTrue(command.Params.Count > 0, "1");
      Assert.IsTrue(command.UnparsedParams.Trim().Length > 0, "2");
    }

    private void HandleGET(CommandText<OurCustomContext> command) {
      command.Response.Add(command.Context.OurValue);
    }

    private void HandleQuit(CommandText<OurCustomContext> command) {
      command.Response.Add("Bye");
    }

    [Test(Description = "Tests the TcpServerCmdText class using a custom Context class")]
    public void DoTest() {
      TcpServerCmdText<OurCustomContext> server = new TcpServerCmdText<OurCustomContext>();
      ServerSocketSimulator serverSimulator = TestUtilities.GetSimulationServerSocket(server, "CustomContext");
      server.ServerSocket = serverSimulator;
      CommandHandlerText<OurCustomContext> tch = new CommandHandlerText<OurCustomContext>();
      tch.Command = "SET";
      tch.NormalReply.Code = 200;
      tch.OnCommand += HandleSET;
      server.CommandHandlers.Add(tch);
      tch = new CommandHandlerText<OurCustomContext>();
      tch.Command = "GET";
      tch.NormalReply.Code = 200;
      tch.OnCommand += HandleGET;
      server.CommandHandlers.Add(tch);
      tch = new CommandHandlerText<OurCustomContext>();
      tch.Command = "QUIT";
      tch.NormalReply.Code = 200;
      tch.Disconnect = true;
      tch.OnCommand += HandleQuit;
      server.CommandHandlers.Add(tch);
      server.ExceptionReply.Code = 100;
      server.Open();
      try {
        serverSimulator.WaitForFinish();
      } finally {
        server.Close();
      }
    }
  }
}