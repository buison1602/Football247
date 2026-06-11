using System.Security.Cryptography;

namespace Football247.Application.Helper
{
    public class PasswordGeneratorHelper
    {
        private const string Lowercase = "abcdefghijkmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Digits = "23456789";
        private const string Specials = "!@#$%^&*";

        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly bool _includeSpecial;

        public PasswordGeneratorHelper(int minLength, int maxLength, bool includeSpecial = true)
        {
            if (minLength < 4)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength));
            }

            if (maxLength < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }

            _minLength = minLength;
            _maxLength = maxLength;
            _includeSpecial = includeSpecial;
        }

        public string Generate()
        {
            var length = RandomNumberGenerator.GetInt32(_minLength, _maxLength + 1);

            var required = new List<char>
            {
                GetRandomChar(Lowercase),
                GetRandomChar(Uppercase),
                GetRandomChar(Digits)
            };

            if (_includeSpecial)
            {
                required.Add(GetRandomChar(Specials));
            }

            var allChars = Lowercase + Uppercase + Digits + (_includeSpecial ? Specials : string.Empty);
            var result = new List<char>(required);

            while (result.Count < length)
            {
                result.Add(GetRandomChar(allChars));
            }

            Shuffle(result);

            return new string(result.ToArray());
        }

        private static char GetRandomChar(string source)
        {
            var index = RandomNumberGenerator.GetInt32(0, source.Length);
            return source[index];
        }

        private static void Shuffle(List<char> chars)
        {
            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(0, i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
        }
    }
}