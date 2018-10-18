using System;
using System.Collections.Generic;
using System.Text;

using Indy.Sockets.Core;
using Indy.Sockets.UnitTests.Core;
using Indy.Sockets.Web;
using NUnit.Framework;

namespace Indy.Sockets.UnitTests.Web {
	[TestFixture]
	public class HTTPClientTest {
		[Test]
		public void SimpleTest() {
			Socket theSocket = TestUtilities.GetSimulationSocket(typeof(HttpClient), "SimpleTest");
			HttpClient h = new HttpClient();
			h.Socket = theSocket;
			Assert.AreEqual("<html><body>Hello, World!</body></html>", h.Get("http://localhost/"));
			theSocket.Close();
		}
	}
}
