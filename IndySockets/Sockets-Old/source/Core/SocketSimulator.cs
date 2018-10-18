using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Indy.Sockets.Core {
  public class SocketSimulator : SocketTLS {
    public class MessageDoesntMatchExpectedMessageException : IndyException {
      public MessageDoesntMatchExpectedMessageException(string Message, string ExpectedMessage, int MessageLength, int ExpectedMessageLength, string simName)
        : base(String.Format("Message doesn't match Expected Message.{5}" +
                             "Simulation name: '{0}'{5}" +
                             "Message: '{1}'{5}" +
                             "Length: {2}{5}" +
                             "Expected Message: '{3}'{5}" +
                             "Expected Length: {4}", simName, Message, MessageLength, ExpectedMessage, ExpectedMessageLength, "\r\n")) {
      }

      public MessageDoesntMatchExpectedMessageException(byte[] Message, byte[] ExpectedMessage, string simName, Encoding encoding)
        : this(TextWriterIntercept.BytesToBeautyString(Message, encoding), TextWriterIntercept.BytesToBeautyString(ExpectedMessage, encoding), Message.Length, ExpectedMessage.Length, simName) {
      }
    }

    public interface IDataBlockReader {
      void Initialize(XmlNode dataNode);
      byte[] ReadBlock();
			string GetPreviewOfBlock(byte[] data);
      void Close();
    }

    // Only initialize this list in the .cctor
    private static SortedList<string, Type> _DataBlockTypes = new SortedList<string, Type>(StringComparer.InvariantCultureIgnoreCase);
    static SocketSimulator() {
      // register all known datablockreaders
      // those should have a .ctor()
      _DataBlockTypes.Add("text", typeof(CSharpTextDataBlockReader));
    }

    public static IDataBlockReader GetDataBlockReaderByType(string type) {
			if (_DataBlockTypes.ContainsKey(type)) {
				return (IDataBlockReader)Activator.CreateInstance(_DataBlockTypes[type]);
			}
			return null;
    }

    public const int DataBlockSize = 1024; // take 12 bytes right now.

    private XmlDocument _SimFile;
    private XmlNode receiveDataNode;
    private IDataBlockReader receiveDataReader;
    private XmlNode sendDataNode;
    private IDataBlockReader sendDataReader;
    private Buffer expectedOutput = new Buffer();
    private string simulationName;

    private int ReadNewReceiveData() {
      byte[] bytesRead = receiveDataReader.ReadBlock();
      InterceptReceive(ref bytesRead);
      int byteCount = bytesRead.Length;
      if (byteCount > 0) {
        mInputBuffer.Write(bytesRead);
      }
      return byteCount;
    }

    private int ReadNewSendData() {
      byte[] bytesRead = sendDataReader.ReadBlock();
      if (bytesRead.Length > 0) {
        expectedOutput.Write(bytesRead);
      }
      return bytesRead.Length;
    }

    public override bool PassThrough {
      set {
        if (value != PassThrough) {
          base.PassThrough = value;
          if (!value) {
            byte[] bytes = Encoding.GetBytes("<<STARTTLS>>");
            Transmit(ref bytes);
          } else {
            byte[] bytes = Encoding.GetBytes("<<STOPTLS>>");
            Transmit(ref bytes);
          }
        }
      }
    }

    // watch out, the stream is being closed
    public SocketSimulator(Stream simulationStream, string simulationName)
      : this(simulationName) {
      XmlDocument Sim = new XmlDocument();
      Sim.Load(simulationStream);
      InitializeFromXmlDocument(Sim);
      simulationStream.Close();
    }

    internal SocketSimulator(string simulationName) {
      this.simulationName = simulationName;
    }

    public void InitializeFromXmlDocument(XmlDocument xmlDoc) {
      _SimFile = xmlDoc;
    }

    public override void Open() {
      base.Open();
      sendDataNode = _SimFile.SelectSingleNode("/simulation/sendData");
      receiveDataNode = _SimFile.SelectSingleNode("/simulation/receiveData");
      sendDataReader = GetDataBlockReaderByType(sendDataNode.Attributes["type"].Value);
      sendDataReader.Initialize(sendDataNode);
      receiveDataReader = GetDataBlockReaderByType(receiveDataNode.Attributes["type"].Value);
      receiveDataReader.Initialize(receiveDataNode);
    }

    public override void Close() {
      ReadNewSendData();
      ReadNewReceiveData();
      if (expectedOutput.Size > 0) {
				byte[] sampleData = expectedOutput.ExtractToByteArray(Math.Min(16, expectedOutput.Size));
        throw new Exception("More output data expected. (Size = " + (sampleData.Length + expectedOutput.Size).ToString() + ", SimulationName = '" + simulationName + "', Data = '" + sendDataReader.GetPreviewOfBlock(sampleData) + "')");
      }
      if (InputBuffer.Size > 0) {
				byte[] sampleData = InputBuffer.ExtractToByteArray(Math.Min(16, InputBuffer.Size));
				throw new Exception("Still data to read. (Size = " + (sampleData.Length + InputBuffer.Size).ToString() + ", SimulationName = '" + simulationName + "', Data = '" + receiveDataReader.GetPreviewOfBlock(sampleData) + "')");
      }
      receiveDataReader.Close();
      receiveDataReader = null;
      sendDataReader.Close();
      sendDataReader = null;
      base.Close();
    }

    private static void CheckByteArrays(byte[] Message, byte[] ExpectedMessage, string simName, Encoding encoding) {
      if (Message.Length != ExpectedMessage.Length) {
        throw new MessageDoesntMatchExpectedMessageException(Message, ExpectedMessage, simName, encoding);
      }

      for (int i = 0; i < Message.Length; i++) {
        if (Message[i].CompareTo(ExpectedMessage[i]) != 0) {
          throw new MessageDoesntMatchExpectedMessageException(Message, ExpectedMessage, simName, encoding);
        }
      }
    }

    public override void Transmit(ref byte[] data) {
      base.Transmit(ref data);
      while ((expectedOutput.Size < data.Length)
        && (ReadNewSendData() > 0)) {
      }
      int dataExpectedLength = Math.Min(data.Length, expectedOutput.Size);
      byte[] dataExpected = expectedOutput.ExtractToByteArray(dataExpectedLength);
      CheckByteArrays(data, dataExpected, simulationName, Encoding);
    }

    protected override int ReadFromSource(bool aThrowIfDisconnected, int aTimeOut, bool AThrowIfTimeout) {
      int dataRead = ReadNewReceiveData();
      if (dataRead == 0) {
        CloseGracefully();
      }
      return dataRead;
    }

    public override void CheckForDisconnect(bool throwExceptionIfDisconnected, bool ignoreBuffer) {
      bool xDisconnected = false;
      if (mClosedGracefully) {
        if (IsOpen()) {
          Close();
#warning Implement Status
          //DoStatus(StatusEnum.Disconnected);
        }
        xDisconnected = true;
      } else {
        xDisconnected = !IsOpen();
      }
      //LDisconnected = (!(BindingAllocated() && Binding.Handle.Connected));
      if (xDisconnected) {
        if (mInputBuffer != null) {
          if ((mInputBuffer.Size == 0 || ignoreBuffer)
            && throwExceptionIfDisconnected) {
            ThrowConnClosedGracefully();
          }
        }
      }
    }

    public override void CheckForDataOnSource(int aTimeOut) {
      if (IsOpen()) {
        ReadFromSource(false, aTimeOut, false);
      }
    }

    public override bool IsOpen() {
      return
        (receiveDataNode != null) &&
        (receiveDataReader != null) &&
        (sendDataNode != null) &&
        (sendDataReader != null);
    }
  }
}
