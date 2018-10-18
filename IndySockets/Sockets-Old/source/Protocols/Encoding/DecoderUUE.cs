using System;

namespace Indy.Sockets.Protocols.Encoding
{
	public class DecoderUUE: Decoder00E
	{
    public DecoderUUE()
    {
      _DecodeTable = UUE.DecodeTable;
      _FillChar = (char)UUE.DecodeTable[0];
    }
	}
}
