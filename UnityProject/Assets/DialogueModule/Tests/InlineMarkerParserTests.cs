using System.Collections.Generic;
using NUnit.Framework;

namespace DialogueModule.Tests
{
    public class InlineMarkerParserTests
    {
        [Test]
        public void Parse_PlainText_ReturnsUnchangedWithNoMarkers()
        {
            var clean = InlineMarkerParser.Parse("hello world", out var markers);

            Assert.AreEqual("hello world", clean);
            Assert.AreEqual(0, markers.Count);
        }

        [Test]
        public void Parse_SpeedTag_StripsTagAndRecordsMarkerAtStripPosition()
        {
            var clean = InlineMarkerParser.Parse("slow<speed=0.2>fast", out var markers);

            Assert.AreEqual("slowfast", clean);
            Assert.AreEqual(1, markers.Count);
            Assert.AreEqual(4, markers[0].charIndex);
            Assert.AreEqual(MarkerKind.Speed, markers[0].kind);
            Assert.AreEqual("0.2", markers[0].value);
        }

        [Test]
        public void Parse_FxAndSfxTags_RecordBothInOrder()
        {
            var clean = InlineMarkerParser.Parse("a<fx=flashback_white>b<sfx=heartbeat>c", out var markers);

            Assert.AreEqual("abc", clean);
            Assert.AreEqual(2, markers.Count);
            Assert.AreEqual(1, markers[0].charIndex);
            Assert.AreEqual(MarkerKind.Fx, markers[0].kind);
            Assert.AreEqual("flashback_white", markers[0].value);
            Assert.AreEqual(2, markers[1].charIndex);
            Assert.AreEqual(MarkerKind.Sfx, markers[1].kind);
            Assert.AreEqual("heartbeat", markers[1].value);
        }

        [Test]
        public void Parse_NullInput_ReturnsEmptyStringAndNoMarkers()
        {
            var clean = InlineMarkerParser.Parse(null, out var markers);

            Assert.AreEqual(string.Empty, clean);
            Assert.AreEqual(0, markers.Count);
        }
    }
}
