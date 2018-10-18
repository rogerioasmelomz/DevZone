using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core.TcpServerCmdText {
	[TestFixture]
	public class TextCmdTcpServerTest {
		private void HandleCommand1(CommandText<ContextRFC> command) {
			Assert.IsTrue(command.PerformReply, "0");
			Assert.AreEqual("Hello", command.Params[0], "HandleCommand1, 1");
			Assert.AreEqual("Param", command.Params[1], "HandleCommand1, 2");
			StringAssert.AreEqualIgnoringCase("Command1", command.CommandHandler.Command, "2B");
			Assert.AreEqual("Test command structure", command.CommandHandler.Description[0], "2C");
			Assert.AreEqual("*", command.CommandHandler.HelpSuperScript, "2D");
			Assert.IsFalse(command.CommandHandler.HelpVisible, "2E");
			Assert.AreEqual("COMMAND1 Hello Param", command.RawLine, "3");
			command.Response.Add("Hello, World!");
		}

		private void HandleCommand2(CommandText<ContextRFC> command) {
			Assert.Fail("Command2 is disabled!");
		}

		private void HandleEXCEPTION(CommandText<ContextRFC> command) {
			throw new Exception("Error");
		}

		private void HandleQuit(CommandText<ContextRFC> command) {
			Assert.AreEqual(0, command.Reply.Text.Count, "3");
			command.Reply.Text.Add("Bye");
		}

		private void HandleQuit2(CommandText<ContextRFC> command) {
			Assert.AreEqual(0, command.Reply.Text.Count, "3");
			command.Reply.Text.Add("Bye");
			command.Disconnect = true;
		}

		private void HandleMakeException(CommandText<ContextRFC> command) {
			throw new ArgumentException();
		}

		[Test(Description = "Tests the Command and reply system, simple")]
		public void SimpleTest() {
			TcpServerCmdText<ContextRFC> server = new TcpServerCmdText<ContextRFC>();
			ServerSocketSimulator serverSimulator = TestUtilities.GetSimulationServerSocket(server, "SimpleTest");
			server.ServerSocket = serverSimulator;
			CommandHandlerText<ContextRFC> tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Command1";
			tch.Description.Add("Test command structure");
			tch.HelpSuperScript = "*";
			tch.HelpVisible = false;
			tch.NormalReply.Code = 200;
			tch.NormalReply.Text.Add("Here it comes");
			tch.OnCommand += HandleCommand1;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Command2";
			tch.NormalReply.Code = 200;
			tch.NormalReply.Text.Add("Done");
			tch.OnCommand += HandleCommand2;
			tch.Enabled = false;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Quit";
			tch.NormalReply.Code = 200;
			tch.Disconnect = true;
			tch.OnCommand += HandleQuit;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Quit2";
			tch.NormalReply.Code = 200;
			//tch.Disconnect = true;
			tch.OnCommand += HandleQuit2;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "EXCEPTION";
			tch.NormalReply.Code = 200;
			tch.ExceptionReply.Code = 500;
			tch.ExceptionReply.Text.Add("ERROR");
			tch.OnCommand += HandleEXCEPTION;
			server.CommandHandlers.Add(tch);
			server.ExceptionReply.Code = 500;
			server.ServerSocket.Intercept = new TextWriterServerIntercept();
			server.Open();
			try {
				serverSimulator.WaitForFinish();
			} finally {
				server.Close();
			}
		}

		[Test(Description = "Just Open and Closed the server")]
		public void OpenCloseTest() {
			TcpServerCmdText<ContextRFC> server = new TcpServerCmdText<ContextRFC>();
			ServerSocketSimulator serverSimulator = TestUtilities.GetSimulationServerSocket(server, "OpenClose");
			server.ServerSocket = serverSimulator;
			server.Open();
			try {
				serverSimulator.WaitForFinish();
			} finally {
				server.Close();
			}
		}

		[Test(Description = "Test DoCommandHandlersException")]
		[ExpectedException(typeof(Exception), "Exception occurred during test. Message: Value does not fall within the expected range.")]
		public void TestDoCommandHandlersException() {
			TcpServerCmdText<ContextRFC> server = new TcpServerCmdText<ContextRFC>();
			CommandHandlerText<ContextRFC> tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "MakeException";
			tch.NormalReply.Code = 200;
			tch.OnCommand += HandleMakeException;
			tch.ExceptionReply.Code = 501;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Command2";
			tch.NormalReply.Code = 200;
			tch.OnCommand += delegate(CommandText<ContextRFC> command) {
				Assert.Fail("Shouldn't be called");
			};
			server.CommandHandlers.Add(tch);
			server.ExceptionReply.Code = 500;
			ServerSocketSimulator serverSimulator = TestUtilities.GetSimulationServerSocket(server, "TestDoCommandHandlersException");
			server.ServerSocket = serverSimulator;
			server.Open();
			try {
				serverSimulator.WaitForFinish();
			} finally {
				server.Close();
			}
		}

		[Test(Description = "Tests several events from CommandHandlerList")]
		public void TestCommandHandlerListEvents() {
			TcpServerCmdText<ContextRFC> server = new TcpServerCmdText<ContextRFC>();
			ServerSocketSimulator serverSimulator = TestUtilities.GetSimulationServerSocket(server, "SimpleTest");
			server.ServerSocket = serverSimulator;
			CommandHandlerText<ContextRFC> tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Command1";
			tch.Description.Add("Test command structure");
			tch.HelpSuperScript = "*";
			tch.HelpVisible = false;
			tch.NormalReply.Code = 200;
			tch.NormalReply.Text.Add("Here it comes");
			tch.OnCommand += HandleCommand1;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Command2";
			tch.NormalReply.Code = 200;
			tch.NormalReply.Text.Add("Done");
			tch.OnCommand += HandleCommand2;
			tch.Enabled = false;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Quit";
			tch.NormalReply.Code = 200;
			tch.Disconnect = true;
			tch.OnCommand += HandleQuit;
			server.CommandHandlers.Add(tch);
			tch = new CommandHandlerText<ContextRFC>();
			tch.Command = "Quit2";
			tch.NormalReply.Code = 200;
			tch.Disconnect = true;
			tch.OnCommand += HandleQuit2;
			server.CommandHandlers.Add(tch);
			server.ExceptionReply.Code = 500;
			server.ServerSocket.Intercept = new TextWriterServerIntercept();
			int BeforeCommandHandlerCallCount = 0;
			int AfterCommandHandlerCallCount = 0;
			CommandHandlerEventHandler<CommandHandlerListText<ContextRFC>, ContextRFC> doBeforeCommandHandler = delegate(CommandHandlerListText<ContextRFC> ASender, ContextRFC AContext) {
				Interlocked.Increment(ref BeforeCommandHandlerCallCount);
			};
			CommandHandlerEventHandler<CommandHandlerListText<ContextRFC>, ContextRFC> doAfterCommandHandler = delegate(CommandHandlerListText<ContextRFC> ASender, ContextRFC AContext) {
				Interlocked.Increment(ref AfterCommandHandlerCallCount);
			};
			server.CommandHandlers.OnBeforeCommandHandler += doBeforeCommandHandler;
			server.CommandHandlers.OnAfterCommandHandler += doAfterCommandHandler;
			server.Open();
			try {
				serverSimulator.WaitForFinish();
			} finally {
				server.Close();
			}
			Assert.AreEqual(6, BeforeCommandHandlerCallCount, "1");
			Assert.AreEqual(6, AfterCommandHandlerCallCount, "2");
			serverSimulator = TestUtilities.GetSimulationServerSocket(server, "SimpleTest");
			server.ServerSocket = serverSimulator;
			server.CommandHandlers.OnBeforeCommandHandler -= doBeforeCommandHandler;
			server.CommandHandlers.OnAfterCommandHandler -= doAfterCommandHandler;
			server.Open();
			try {
				serverSimulator.WaitForFinish();
			} finally {
				server.Close();
			}
			Assert.AreEqual(6, BeforeCommandHandlerCallCount, "3");
			Assert.AreEqual(6, AfterCommandHandlerCallCount, "4");
		}
	}
}