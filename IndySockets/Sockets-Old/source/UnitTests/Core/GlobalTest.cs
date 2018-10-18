using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	[TestFixture]
	public class GlobalTest {
		[Test]
		public void TestStrToInt16Def() {
			Assert.AreEqual(5, Global.StrToInt16Def("non-int", 5));
			Assert.AreEqual(5, Global.StrToInt16Def("5", 6));
		}

		[Test]
		public void TestStrToInt32Def() {
			Assert.AreEqual(5, Global.StrToInt32Def("non-int", 5));
			Assert.AreEqual(5, Global.StrToInt32Def("5", 6));
		}

		[Test]
		public void TestStrToInt64Def() {
			Assert.AreEqual(5, Global.StrToInt64Def("non-int", 5));
			Assert.AreEqual(5, Global.StrToInt64Def("5", 6));
		}
	}
}
