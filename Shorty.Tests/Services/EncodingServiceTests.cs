using System.Text.RegularExpressions;
using FluentAssertions;
using Shorty.Services;

namespace Shorty.Tests.Services
{
    public class EncodingServiceTests
    {
        [Fact]
        public void TestCreateUrlTag_Alpha()
        {
            var expectedResult = "AHKUAaAOeX2F5q4a5B6rGw";
            var guid = new Guid("01947200-0ea0-7d79-85e6-ae1ae41eab1b");
            var testService = new EncodingService(MockKeyProvider(guid.ToByteArray()));
            var result = testService.CreateUrlTag();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void TestCreateUrlTag_NonAlpha()
        {
            var matchRegex = new Regex("[A-Z]{2}XGUAbcKlHajIGgN[a-z]{2}pDH7w");
            var guid = new Guid("019471f9-0ab7-7694-a320-680dfe90c7ef");
            var testService = new EncodingService(MockKeyProvider(guid.ToByteArray()));
            var result = testService.CreateUrlTag();

            matchRegex.IsMatch(result).Should().BeTrue();
        }

        private static Func<byte[]> MockKeyProvider(byte[] result)
        {
            return new Func<byte[]>(() => result);
        }
    }
}
