using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Shorty.Configs;
using Shorty.Exceptions;
using Shorty.Models;
using Shorty.Services;

namespace Shorty.Tests.Services
{
    public class UrlServiceTests
    {
        #region ExpandUrl Tests

        [Fact]
        public async Task TestExpandUrl_BadUrl()
        {
            var testService = new UrlService(MockConfig(), MockDataService().Object, MockEncodingService());
            await Assert.ThrowsAsync<InvalidUrlException>(async () => await testService.ExpandUrl(badUrl));
        }

        [Fact]
        public async Task TestExpandUrl_KnownUrl()
        {
            var shortUrl = String.Concat(baseUrl, tag);
            var mockDataService = MockDataService(null, GetUrlDocument(goodFullUrl, tag));
            var testService = new UrlService(MockConfig(), mockDataService.Object, MockEncodingService());
            var result = await testService.ExpandUrl(shortUrl);

            result.Should().Be(goodFullUrl);
        }

        [Fact]
        public async Task TestExpandUrl_UnknownUrl()
        {
            var unknownUrl = String.Concat(baseUrl, "bogus_tag");
            var testService = new UrlService(MockConfig(), MockDataService().Object, MockEncodingService());
            await Assert.ThrowsAsync<UnknownUrlException>(async () => await testService.ExpandUrl(unknownUrl));
        }

        #endregion

        #region ShortenUrl Tests

        [Fact]
        public async Task TestShortenUrl_BadUrl()
        {
            var testService = new UrlService(MockConfig(), MockDataService().Object, MockEncodingService());
            await Assert.ThrowsAsync<InvalidUrlException>(async () => await testService.ShortenUrl(badUrl));
        }

        [Fact]
        public async Task TestShortenUrl_CreateUrl()
        {
            var expectedUrl = String.Concat(baseUrl, tag);
            var saveValue = GetUrlDocument(goodFullUrl, tag);
            var mockDataService = MockDataService(null, null, saveValue);
            var testService = new UrlService(MockConfig(), mockDataService.Object, MockEncodingService());
            var result = await testService.ShortenUrl(goodFullUrl);

            result.Should().Be(expectedUrl);
            mockDataService.Verify();
        }

        [Fact]
        public async Task TestShortenUrl_FindUrl()
        {
            var expectedUrl = String.Concat(baseUrl, tag);
            var findResult = GetUrlDocument(goodFullUrl, tag);
            var mockDataService = MockDataService(findResult);
            var testService = new UrlService(MockConfig(), mockDataService.Object, MockEncodingService());
            var result = await testService.ShortenUrl(goodFullUrl);

            result.Should().Be(expectedUrl);
        }

        #endregion

        #region Setup Helpers

        private static IOptions<EncoderConfig> MockConfig()
        {
            var settings = new EncoderConfig(){ BaseUrl = baseUrl };
            return Options.Create(settings);
        }

        private static Mock<IDataService> MockDataService(UrlDocument? findResult=null, UrlDocument? findTagResult = null, UrlDocument? saveValue=null)
        {
            var mock = new Mock<IDataService>();
            mock.Setup(m => m.FindUrl(It.IsAny<Uri>())).Returns(Task.FromResult(findResult));
            mock.Setup(m => m.FindUrlDocumentByTag(It.IsAny<string>())).Returns(Task.FromResult(findTagResult));

            if (saveValue != null)
                mock.Setup(m => m.SaveUrl(It.IsAny<UrlDocument>())).Verifiable();
            
            return mock;
        }

        private static IEncodingService MockEncodingService()
        {
            var mock = new Mock<IEncodingService>();
            mock.Setup(m => m.CreateUrlTag()).Returns(tag);
            return mock.Object;
        }

        private static UrlDocument GetUrlDocument(string fullUrl, string tag)
        {
            return new UrlDocument(){ FullUrl = fullUrl, Tag = tag };
        }

        private static readonly string badUrl = "some/stringy/garbage";

        private static readonly string baseUrl = "https://shorty.com/";

        private static readonly string goodFullUrl = "https://goodone.com/open/doors/stay/open";

        private static readonly byte[] key = BitConverter.GetBytes(10000000);

        private static readonly string tag = Base64UrlEncoder.Encode(key);

        #endregion
    }
}
