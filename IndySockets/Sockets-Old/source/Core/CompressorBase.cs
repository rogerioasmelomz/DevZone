using System;
using System.IO;

namespace Indy.Sockets.Core {
	public abstract class CompressorBase {
		public abstract void DecompressDeflateStream(Stream stream);
		public abstract void DecompressGZipStream(Stream stream);
	}
}