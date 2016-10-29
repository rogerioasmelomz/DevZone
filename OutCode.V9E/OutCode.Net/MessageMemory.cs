using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace OutCode.Net
{
    public class MessageMemory : Message {
        protected List<string> mBody = new List<string>();
        public List<string> Body {
            get { return mBody; }
        }

        public override void WriteBody(TextWriter aWriter, bool aRFCFormat) {
            foreach (var s in mBody) {
                if (s.StartsWith(".") && aRFCFormat) {
                    aWriter.Write(".");
                }
                aWriter.WriteLine(s);
            }
        }

        public override void ReadBody(TextReader aReader) {
            Body.Clear();
            aReader.ReadRFCLines(Body);
        }

    }
}
