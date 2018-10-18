using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Indy.Sockets.Core;
using IS = Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	[TestFixture]
	public class ExplicitTLSClientTest {
		private class SimpleExplicitTLSClient: ExplicitTLSClient<int, ReplyRFC> {
			public SimpleExplicitTLSClient(UseTLSEnum useTLSValue) {
				UseTLS = useTLSValue;
			}
			public override bool SupportsTLS {
				get {
					return true;
				}
			}

			public void StartTLS() {
				CheckIfCanUseTLS();
				base.TLSHandshake();
			}

			public void FailNegCmd() {
				ProcessTLSNegCmdFailed();
			}

			public void FailNotAvail() {
				ProcessTLSNotAvailable();
			}

			public void StopTLS() {
				base.SocketTLS.PassThrough = true;
			}
		}
		[Test]
		public void DoSuccessfulTest() {
			IS.SocketTLS socket = TestUtilities.GetSimulationSocket(typeof(IS.ExplicitTLSClient<int, IS.ReplyRFC>), "SimpleTest");
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.UseRequireTLS);
			client.Connect(socket);
			socket.WriteLn("Hello, Unsecure World!");
			client.StartTLS();
			socket.WriteLn("Hello, Secure World!");
			client.StopTLS();
			socket.WriteLn("Hello, Unsecure World!");
			client.StartTLS();
			socket.WriteLn("Hello, Secure World!");
			Assert.AreEqual("Done", socket.ReadLn());
			client.Disconnect();
		}

		[Test]
		[ExpectedException(typeof(IndyException), ExpectedMessage = "SocketTLS required!")]
		public void TestException1() {
			Socket s = new LoopbackSocket();
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.UseRequireTLS);
			client.Connect(s);
			client.StartTLS();
		}

		[Test]
		[ExpectedException(typeof(IndyException), ExpectedMessage = "TLS Handshake failed!")]
		public void TestException2() {
			Socket s = new LoopbackSocketTLS(true);
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.UseRequireTLS);
			client.Connect(s);
			client.StartTLS();
		}

		[Test]
		[ExpectedException(typeof(IndyException), ExpectedMessage = "TLS Handshake failed!")]
		public void TestException3() {
			Socket s = new LoopbackSocketTLS(true);
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(s);
			client.StartTLS();
		}

		[Test]
		public void TestException4() {
			Socket s = new LoopbackSocketTLS(true);
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(s);
			client.OnTLSHandshakeFailed += delegate(object sender, ref bool canContinue) {
				canContinue = true;
			};
			client.StartTLS();
			Assert.IsTrue(true);
		}

		[Test]
		[ExpectedException(typeof(IndyException), "TLS command failed!")]
		public void TestException5() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(new LoopbackSocketTLS(false));
			client.FailNegCmd();
		}

		[Test]
		[ExpectedException(typeof(IndyException), "TLS command failed!")]
		public void TestException6() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.UseRequireTLS);
			client.Connect(new LoopbackSocketTLS(false));
			client.FailNegCmd();
		}

		[Test]
		public void TestException7() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(new LoopbackSocketTLS(false));
			client.OnTLSNegCmdFailed += delegate(object sender, ref bool canContinue) {
				canContinue = true;
			};
			client.FailNegCmd();
		}

		[Test]
		[ExpectedException(typeof(IndyException), "TLS not available")]
		public void TestException8() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(new LoopbackSocketTLS(false));
			client.FailNotAvail();
		}

		[Test]
		[ExpectedException(typeof(IndyException), "TLS not available")]
		public void TestException9() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.UseRequireTLS);
			client.Connect(new LoopbackSocketTLS(false));
			client.FailNotAvail();
		}

		[Test]
		public void TestException10() {
			SimpleExplicitTLSClient client = new SimpleExplicitTLSClient(UseTLSEnum.NoTLSSupport);
			client.Connect(new LoopbackSocketTLS(false));
			client.OnTLSNotAvailable += delegate(object sender, ref bool canContinue) {
				canContinue = true;
			};
			client.FailNotAvail();
		}
	}
}