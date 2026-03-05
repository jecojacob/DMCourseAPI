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
                masked = pattern.Pattern.Replace(masked, pattern.Replacement);
            }

            return masked;
        }
    }
}
