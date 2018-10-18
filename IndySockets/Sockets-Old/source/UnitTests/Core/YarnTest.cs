using System;
using System.Collections.Generic;
using System.Text;
using Indy.Sockets.Core;
using NUnit.Framework;

namespace Indy.Sockets.UnitTests.Core {
	[TestFixture]
	public class YarnTest {

		[Test]
		public void TestDispose() {
			using (new Yarn()) {
				Assert.IsTrue(true);
			}
		}
	}
}
