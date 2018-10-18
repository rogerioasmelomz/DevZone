using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
  [TestFixture]
  public class ReplyRFCTest {

    [Test(Description = "Tests the FormattedReply property (1)")]
    public void TestFormattedReply1() {
      ReplyRFC reply = new ReplyRFC();
      const string format = "200";
      reply.FormattedReply = Encoding.ASCII.GetBytes(format);
      Assert.AreEqual(200, reply.Code, "1");
      Assert.AreEqual(0, reply.Text.Count, "2");
    }   
    
    private const string TestFormattedReply2_Line1 = "Line 1";
    private const string TestFormattedReply2_Line2 = "Line 2";
    private const string TestFormattedReply2_Line3 = "Line 3";
    private static readonly string TestFormattedReply2_FormattedReply = String.Concat(new string[] {
        "200-" + TestFormattedReply2_Line1, Global.EOL,
        "200-" + TestFormattedReply2_Line2, Global.EOL,
        "200 " + TestFormattedReply2_Line3, Global.EOL,
        });

    [Test(Description = "Tests the FormattedReply property (2)")]
    public void TestFormattedReply2() {
      ReplyRFC reply = new ReplyRFC();
      reply.Code = 200;
      reply.Text.Add(TestFormattedReply2_Line1);
      reply.Text.Add(TestFormattedReply2_Line2);
      reply.Text.Add(TestFormattedReply2_Line3);
      Assert.AreEqual(3, reply.Text.Count, "1");
      string Result = Encoding.ASCII.GetString(reply.FormattedReply);
      Assert.AreEqual(TestFormattedReply2_FormattedReply, Result, "2");
      reply.FormattedReply = Encoding.ASCII.GetBytes(TestFormattedReply2_FormattedReply);
      Assert.AreEqual(200, reply.Code, "3");
      Assert.AreEqual(3, reply.Text.Count, "4");
      Assert.AreEqual(TestFormattedReply2_Line1, reply.Text[0], "5");
      Assert.AreEqual(TestFormattedReply2_Line2, reply.Text[1], "6");
      Assert.AreEqual(TestFormattedReply2_Line3, reply.Text[2], "7");
    }

    [Test]
    public void TestFormattedReply3() {
      ReplyRFC reply = new ReplyRFC();
      reply.Code = 100;
      reply.Text.Add("Done");
      reply.FormattedReply = reply.FormattedReply;
      Assert.AreEqual(100, reply.Code, "1");
      Assert.AreEqual(1, reply.Text.Count, "2");
      Assert.AreEqual("Done", reply.Text[0], "3");
    }
  }
}
