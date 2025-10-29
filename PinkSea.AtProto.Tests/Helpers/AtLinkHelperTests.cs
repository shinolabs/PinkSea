using PinkSea.AtProto.Helpers;

namespace PinkSea.AtProto.Tests.Helpers;

    [TestFixture]
    public class AtLinkHelperTests
    {
        [TestCase("at://did:plc:123/app.bsky.feed.post/456", "did:plc:123", "app.bsky.feed.post", "456")]
        [TestCase("at://example.com/collection/record", "example.com", "collection", "record")]
        [TestCase("at://user.bsky.social/app.bsky.feed.post/12345", "user.bsky.social", "app.bsky.feed.post", "12345")]
        public void Parse_ValidUri_ReturnsExpectedAtUri(string uri, string authority, string collection, string recordKey)
        {
            var result = AtLinkHelper.Parse(uri);
            Assert.Multiple(() =>
            {
                Assert.That(result.Authority, Is.EqualTo(authority));
                Assert.That(result.Collection, Is.EqualTo(collection));
                Assert.That(result.RecordKey, Is.EqualTo(recordKey));
            });
        }

        [TestCase("at://did:plc:123/app.bsky.feed.post/456", "did:plc:123", "app.bsky.feed.post", "456")]
        [TestCase("at://example.com/collection/record", "example.com", "collection", "record")]
        [TestCase("at://user.bsky.social/app.bsky.feed.post/12345", "user.bsky.social", "app.bsky.feed.post", "12345")]
        public void TryParse_ValidUri_ReturnsTrueAndExpectedAtUri(string uri, string authority, string collection, string recordKey)
        {
            bool success = AtLinkHelper.TryParse(uri, out var result);
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True);
                Assert.That(result.Authority, Is.EqualTo(authority));
                Assert.That(result.Collection, Is.EqualTo(collection));
                Assert.That(result.RecordKey, Is.EqualTo(recordKey));
            });
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("not a uri")]
        [TestCase("at:/missing/parts")]
        [TestCase("at://authority/missingRecordKey")]
        [TestCase("at://authority/collection/")]
        [TestCase("at:///collection/record")]
        [TestCase("at://authority//record")]
        public void Parse_InvalidUri_ThrowsFormatException(string uri)
        {
            Assert.Throws<FormatException>(() => AtLinkHelper.Parse(uri));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("not a uri")]
        [TestCase("at:/missing/parts")]
        [TestCase("at://authority/missingRecordKey")]
        [TestCase("at://authority/collection/")]
        [TestCase("at:///collection/record")]
        [TestCase("at://authority//record")]
        public void TryParse_InvalidUri_ReturnsFalseAndDefaultAtUri(string uri)
        {
            bool success = AtLinkHelper.TryParse(uri, out var result);
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.False);
                Assert.That(result, Is.EqualTo(default(AtLinkHelper.AtUri)));
            });
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            var uri = new AtLinkHelper.AtUri("did:plc:123", "app.bsky.feed.post", "456");
            Assert.That(uri.ToString(), Is.EqualTo("at://did:plc:123/app.bsky.feed.post/456"));
        }
    }