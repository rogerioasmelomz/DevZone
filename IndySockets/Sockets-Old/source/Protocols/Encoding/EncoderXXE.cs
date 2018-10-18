using System;

namespace Indy.Sockets.Protocols.Encoding
{
	public class EncoderXXE: Encoder00E
	{
		public EncoderXXE()
		{
		  _CodingTable = XXE.CodeTable;
		  _FillChar = _CodingTable[0];
		}
	}
}
