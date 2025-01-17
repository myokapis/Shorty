using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Shorty.Services
{
    public class EncodingService : IEncodingService
    {
        private readonly Func<byte[]> keyProvider;
        public EncodingService(Func<byte[]> keyProvider)
        {
            this.keyProvider = keyProvider;
        }

        public string CreateUrlTag()
        {
            // NOTE: Documentation is unclear whether the encode overload that accepts a byte array
            //       suppresses padding and substitutes URL-friendly characters for / and +.
            //       Playing with some data looks like it does.
            var tag = Base64UrlEncoder.Encode(keyProvider());

            return urlRegex.IsMatch(tag) ? string.Concat(ReplaceChars(tag)) : tag;
        }

        /// <summary>
        /// Makes tag conform to the requirement that all characters are alphanumeric.
        /// It replaces dashes with two of a random upper case character.
        /// Underscores are replaced with two of a random lower case character.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>An enumerable containing the original string with dashes and underscores replaced</returns>
        public IEnumerable<char> ReplaceChars(string tag)
        {
            var random = new Random();
            var dashStandIn = Convert.ToChar(random.Next(65, 90));
            var underscoreStandIn = Convert.ToChar(random.Next(97, 122));

            foreach (var c in tag.ToCharArray())
            {
                if (c == '-')
                {
                    yield return dashStandIn;
                    yield return dashStandIn;
                }
                else if (c == '_')
                {
                    yield return underscoreStandIn;
                    yield return underscoreStandIn;
                }
                else
                    yield return c;
            };
        }

        static protected Regex urlRegex = new Regex("[-_]");
    }
}
