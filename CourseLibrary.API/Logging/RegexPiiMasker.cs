using System;

namespace CourseLibrary.API.Logging
{
    public class RegexPiiMasker : IPiiMasker
    {
        private readonly PiiMaskingOptions _options;

        public RegexPiiMasker(PiiMaskingOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string Mask(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var masked = input;

            foreach (var pattern in _options.MaskingPatterns)
            {
                if (!string.IsNullOrWhiteSpace(pattern.Replacement))
                {
                    masked = pattern.Pattern.Replace(masked, pattern.Replacement);
                    continue;
                }

                if (string.Equals(pattern.Name, "email", StringComparison.OrdinalIgnoreCase))
                {
                    masked = pattern.Pattern.Replace(masked, match => MaskEmail(match.Value));
                    continue;
                }

                masked = pattern.Pattern.Replace(masked, match => MaskMatchedValue(match.Value));
            }

            return masked;
        }

        private string MaskMatchedValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (!_options.ShowFirstAndLastCharacter)
            {
                return _options.RedactionText;
            }

            if (value.Length == 1)
            {
                return _options.MiddleMask;
            }

            return string.Concat(value[0], _options.MiddleMask, value[value.Length - 1]);
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return email;
            }

            var atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex == email.Length - 1)
            {
                return MaskMatchedValue(email);
            }

            var firstCharacter = email[0];
            var charactersToMask = atIndex - 1;
            if (charactersToMask <= 0)
            {
                return email;
            }

            var hiddenLocalPart = new string('*', charactersToMask);
            var domainPart = email.Substring(atIndex);
            return string.Concat(firstCharacter, hiddenLocalPart, domainPart);
        }
    }
}
