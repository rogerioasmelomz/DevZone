using System;
using System.Collections.Generic;
using System.Text;
using Indy.Sockets.Core;
using Indy.Sockets.UnitTests.Core;
using Indy.Sockets.Web;
using NUnit.Framework;

namespace Indy.Sockets.UnitTests.Web {
	[TestFixture]
	public class SimpleHttpServerTest {
		[Test]
		public void SimpleTest() {
			SimpleHttpServer shs = new SimpleHttpServer();
			ServerSocketSimulator sss = TestUtilities.GetSimulationServerSocket(shs, "SimpleTest");
			shs.ServerSocket = sss;
			shs.OnCommandGet += delegate(ContextRFC sender, HttpRequestInfo requestInfo, HttpResponseInfo responseInfo, ref bool handled) {
				if (requestInfo.Document.Equals("index", StringComparison.InvariantCultureIgnoreCase)) {
					handled = true;
					responseInfo.ContentText = "<html><body>Index</body></html>";
					responseInfo.ContentType = "text/html";
					responseInfo.Connection = "close";
					return;
				}
				responseInfo.Connection = "close";
				handled = false;
			};
			shs.Open();
			try {
				sss.WaitForFinish();
			} finally {
				shs.Close();
			}
		}
	}
}
