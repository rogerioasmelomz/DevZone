using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Indy.Sockets.Core;
using IS = Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	[TestFixture]
	public class BufferTest {
		public readonly string TestData = "Test\r\n";
		public readonly int TestDataLength;
		public const int LoopCount = 100;
		private IS.Buffer _Buffer;

		public BufferTest() {
			TestDataLength = TestData.Length;
		}

		[SetUp]
		public void SetupBufferPlayground() {
			_Buffer = new IS.Buffer();
			_Buffer.GrowthFactor = 2;
		}

		[Test(Description = "Fill the buffer with some data, and read it empty again")]
		public void FillAndClearBuffer() {
			int i = 0;
			while (i < LoopCount) {
				_Buffer.Write(TestData);
				i++;
			}
			Assert.AreEqual(_Buffer.Size, TestDataLength * LoopCount);
			i = 0;
			while (i < (LoopCount / 2)) {
				Assert.AreEqual(TestData, _Buffer.Extract(TestDataLength), "1");
				i++;
			}
			Assert.AreEqual(_Buffer.Size, TestDataLength * i, "2");
			_Buffer.CompactHead();
			Assert.AreEqual(_Buffer.Size, TestDataLength * i, "3");
			_Buffer.Extract(-1);
			Assert.AreEqual(_Buffer.Size, 0, "4");
			_Buffer.CompactHead();
			Assert.AreEqual(_Buffer.Size, 0, "5");
		}

		[Test]
		[ExpectedException(typeof(TooMuchDataInBufferException))]
		public void TestBufferCheckSize() {
			using (IS.Buffer buffer = new Indy.Sockets.Core.Buffer()) {
				Assert.AreEqual(Int32.MaxValue, buffer.MaximumSize);
				buffer.MaximumSize = 10;
				buffer.Write("Chad");
				buffer.Write("Hadi");
				buffer.Write("Matthijs");
			}
		}

		[Test]
		[ExpectedException(typeof(IndyException), "Buffer terminator must be specified.")]
		public void TestIndexOfMissingTerminatorException() {
			using (IS.Buffer buffer = new Indy.Sockets.Core.Buffer()) {
				buffer.Write("Chad");
				buffer.IndexOf(new byte[0]);
			}
		}

		[Test(Description = "Test IndexOf on Buffer")]
		[ExpectedException(typeof(IndyException))]
		public void IndexOfLF() {
			_Buffer.Write(
				"This is a test\n" +
				"For Testing IndexOf\n" +
				"On Buffer");
			Assert.AreEqual(_Buffer.IndexOf("\n"), 14);
			Assert.AreEqual(_Buffer.IndexOf("\n", -1), 14);
			Assert.AreEqual(_Buffer.IndexOf("\n", _Buffer.Size), -1);
			_Buffer.Extract(-1);
			_Buffer.IndexOf("\n", 5);
		}

		[Test]
		[ExpectedException(typeof(NotEnoughDataInBufferException))]
		public void ExtractTest() {
			_Buffer.Write("Hello World!\r\n");
			Assert.AreEqual(14, _Buffer.Size, "1");
			Assert.AreEqual("Hello World!\r\n", _Buffer.Extract(-1), "2");
			string Temp = _Buffer.Extract(0);
			Assert.AreEqual("", Temp, "3");
			_Buffer.Write("Hello, World!\r\n");
			byte[] TempBuff = _Buffer.ExtractToByteArray();
			Assert.AreEqual("Hello, World!\r\n", Encoding.ASCII.GetString(TempBuff), "4");
			_Buffer.Write("Hello, World!\r\n");
			Array.Resize<byte>(ref TempBuff, 0);
			_Buffer.ExtractToByteArray(ref TempBuff);
			Assert.AreEqual("Hello, World!\r\n", Encoding.ASCII.GetString(TempBuff), "5");
			_Buffer.ExtractToByteArray(5);
		}

		[Test(Description = "Test IndexOf using a case of the POP3 tests")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void IndexOfTest() {
			_Buffer.Clear();
			_Buffer.Write("+OK POP3 server ready <13804.1151765601@mail2.atozed.com>\n+OK POP3 server signing off\n");
			Assert.AreEqual(_Buffer.IndexOf("\n"), 57, "1");
			_Buffer.PeekByte(_Buffer.Size + 1);
		}

		[TestFixtureTearDown]
		public void TeardownBufferPlayground() {
			_Buffer.Clear();
			_Buffer = null;
		}

		private static string ReadStringFromStream(Stream stream) {
			byte[] TempResult = (byte[])Array.CreateInstance(typeof(byte), stream.Length);
			stream.Read(TempResult, 0, TempResult.Length);
			return Encoding.ASCII.GetString(TempResult);
		}

		[Test]
		public void BufferToStream() {
			const string cString = "12345";
			IS.Buffer LBuffer = new IS.Buffer();
			using (MemoryStream TempStream = new MemoryStream()) {
				Assert.IsTrue(LBuffer.Encoding == Encoding.ASCII, "Buffer defaults to wrong encoding");
				LBuffer.Write(cString);
				LBuffer.ExtractToStream(TempStream);
				TempStream.Position = 0;
				Assert.AreEqual(cString, ReadStringFromStream(TempStream), "1");
				Assert.AreEqual(0, LBuffer.Size, "2");
				TempStream.SetLength(0);
				LBuffer.Write("Hello, World!");
				Assert.AreEqual("Hello, World!", LBuffer.Extract("Hello, World!".Length));
			}
		}

		[Test(Description = "Tests the events of the buffer")]
		public void EventsTest() {
			using (IS.Buffer buffer = new IS.Buffer()) {
				int byteCount = 0;
				EventHandler<BufferEventArgs> OnBytesAddedMethod = delegate(object sender, BufferEventArgs args) {
					byteCount += args.ByteCount;
				};
				buffer.BytesAdded += OnBytesAddedMethod;
				EventHandler<BufferEventArgs> OnBytesRemovedMethod = delegate(object sender, BufferEventArgs args) {
					byteCount -= args.ByteCount;
				};
				buffer.BytesRemoved += OnBytesRemovedMethod;
				buffer.Write("Hello, World!");
				Assert.AreEqual(buffer.Size, byteCount, "1");
				buffer.Remove(buffer.Size);
				Assert.AreEqual(0, byteCount, "2");
				buffer.BytesAdded -= OnBytesAddedMethod;
				buffer.BytesRemoved -= OnBytesRemovedMethod;
				buffer.Write("Hello");
				Assert.AreEqual(0, byteCount, "3");
				buffer.Remove(2);
				Assert.AreEqual(0, byteCount, "4");
			}
		}

		[Test]
		public void BufferToBuffer() {
			using (IS.Buffer buffer1 = new IS.Buffer()) {
				using (IS.Buffer buffer2 = new IS.Buffer()) {
					buffer1.Write("Hello, World!");
					buffer1.ExtractToBuffer(buffer2);
					Assert.AreEqual(0, buffer1.Size, "1");
					Assert.AreEqual(Encoding.ASCII.GetByteCount("Hello, World!"), buffer2.Size, "2");
					Assert.AreEqual("Hello, World!", buffer2.Extract(), "3");
				}
			}
		}

		[Test]
		public void PeekFromEmptyBuffer() {
			using (IS.Buffer buffer = new IS.Buffer()) {
				Assert.AreEqual(-1, buffer.PeekByte(0));
			}
		}
	}
}