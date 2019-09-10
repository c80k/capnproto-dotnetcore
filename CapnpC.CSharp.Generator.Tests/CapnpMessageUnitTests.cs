using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapnpC.CSharp.Generator.Tests
{
    [TestClass]
    public class CapnpMessageUnitTests
    {
        [TestMethod]
        public void ParseError()
        {
            var msg = new CapnpMessage(@"f:\code\invalid.capnp:5:1: error: Parse error.");
            Assert.AreEqual(@"f:\code\invalid.capnp:5:1: error: Parse error.", msg.FullMessage);
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual(@"f:\code\invalid.capnp", msg.FileName);
            Assert.AreEqual(5, msg.Line);
            Assert.AreEqual(1, msg.Column);
            Assert.AreEqual(0, msg.EndColumn);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("Parse error.", msg.MessageText);
        }

        [TestMethod]
        public void ColumnSpan()
        {
            var msg = new CapnpMessage(@"f:\code\invalid.capnp:10:7-8: error: Duplicate ordinal number.");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual(@"f:\code\invalid.capnp", msg.FileName);
            Assert.AreEqual(10, msg.Line);
            Assert.AreEqual(7, msg.Column);
            Assert.AreEqual(8, msg.EndColumn);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("Duplicate ordinal number.", msg.MessageText);
        }

        [TestMethod]
        public void NoSuchFile()
        {
            var msg = new CapnpMessage(@"C:\ProgramData\chocolatey\lib\capnproto\tools\capnproto-tools-win32-0.7.0\capnp.exe compile: doesnotexist.capnp: no such file");
            Assert.IsFalse(msg.IsParseSuccess);
            Assert.AreEqual(@"C:\ProgramData\chocolatey\lib\capnproto\tools\capnproto-tools-win32-0.7.0\capnp.exe compile: doesnotexist.capnp: no such file", msg.FullMessage);
        }

        [TestMethod]
        public void NoId()
        {
            var msg = new CapnpMessage(@"empty.capnp:1:1: error: File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual("empty.capnp", msg.FileName);
            Assert.AreEqual(1, msg.Line);
            Assert.AreEqual(1, msg.Column);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;", msg.MessageText);
        }

        [TestMethod]
        public void AnnoyingNTFSAlternateDataStream1()
        {
            var msg = new CapnpMessage(@"3:2:1:1: error: File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual("3:2", msg.FileName);
            Assert.AreEqual(1, msg.Line);
            Assert.AreEqual(1, msg.Column);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;", msg.MessageText);
        }

        [TestMethod]
        public void AnnoyingNTFSAlternateDataStream2()
        {
            var msg = new CapnpMessage(@"c:\3:2:1:1: error: File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual(@"c:\3:2", msg.FileName);
            Assert.AreEqual(1, msg.Line);
            Assert.AreEqual(1, msg.Column);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;", msg.MessageText);
        }

        [TestMethod]
        public void AnnoyingNTFSAlternateDataStream3()
        {
            var msg = new CapnpMessage(@"\\?\c:\3:2:1:1: error: File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual(@"\\?\c:\3:2", msg.FileName);
            Assert.AreEqual(1, msg.Line);
            Assert.AreEqual(1, msg.Column);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;", msg.MessageText);
        }

        [TestMethod]
        public void AnnoyingNTFSAlternateDataStream4()
        {
            var msg = new CapnpMessage(@"1:2-3:10:7-8: error: Duplicate ordinal number.");
            Assert.IsTrue(msg.IsParseSuccess);
            Assert.AreEqual(@"1:2-3", msg.FileName);
            Assert.AreEqual(10, msg.Line);
            Assert.AreEqual(7, msg.Column);
            Assert.AreEqual(8, msg.EndColumn);
            Assert.AreEqual("error", msg.Category);
            Assert.AreEqual("Duplicate ordinal number.", msg.MessageText);
        }
    }
}
