using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using IS = Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	[TestFixture]
	public class SocketSimulatorTest {
		private static IS.SocketSimulator GetSimulator(string name) {
			return TestUtilities.GetSimulationSocket(typeof(IS.SocketSimulator), name);
		}

		[Test]
		public void TestCorrectRead() {
			IS.SocketSimulator simulator = GetSimulator("TestCorrectRead");
			// check CheckForDataOnSource. It should throw an exception when called if the socket is closed
			simulator.CheckForDataOnSource();
			simulator.Open();
			try {
				Assert.AreEqual("Hello, World\\!", simulator.ReadLn());
			} finally {
				simulator.Close();
			}
		}

		[Test]
		public void TestCorrectWriteAndTLSStuff() {
			IS.SocketSimulator simulator = GetSimulator("TestCorrectWriteAndTLSStuff");
			simulator.Open();
			try {
				simulator.WriteLn("Hello, World!");
				simulator.PassThrough = false;
				simulator.Write(new byte[] { 0, 1, 2, 3, 4 });
				simulator.PassThrough = true;
			} finally {
				simulator.Close();
			}
		}

		[Test]
		[ExpectedException(typeof(Exception), "Still data to read. (Size = 16, SimulationName = 'Core.SocketSimulator.TestStillDataToReadException', Data = 'This isn't read!')")]
		public void TestStillDataToReadException() {
			IS.SocketSimulator simulator = GetSimulator("TestStillDataToReadException");
			simulator.Open();
			try {
				Assert.AreEqual("Hello, World!", simulator.ReadLn());
			} finally {
				simulator.Close();
			}
		}

		[Test]
		[ExpectedException(typeof(Exception), "More output data expected. (Size = 16, SimulationName = 'Core.SocketSimulator.TestMoreOutputDataExpected', Data = 'This isn't sent!')")]
		public void TestMoreOutputDataExpected() {
			IS.SocketSimulator simulator = GetSimulator("TestMoreOutputDataExpected");
			simulator.Open();
			try {
				simulator.WriteLn("Hello, World!");
			} finally {
				simulator.Close();
			}
		}

		[Test]
		[ExpectedException(typeof(IS.SocketSimulator.MessageDoesntMatchExpectedMessageException),
				 "Message doesn't match Expected Message.\r\n" +
				 "Simulation name: 'Core.SocketSimulator.TestWrongMessageSent'\r\n" +
				 "Message: 'Hello, World!\\r\\n'\r\n" +
				 "Length: 15\r\n" +
				 "Expected Message: 'This isn't sent'\r\n" +
				 "Expected Length: 15")]
		public void TestWrongMessageSent() {
			IS.SocketSimulator simulator = GetSimulator("TestWrongMessageSent");
			simulator.Open();
			simulator.WriteLn("Hello, World!");
		}

		[Test]
		public void TestNonExistentDataReader() {
			Assert.IsNull(IS.SocketSimulator.GetDataBlockReaderByType("NonExistentDataBlockReader"));
		}
	}
}
