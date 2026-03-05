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
    }
}
