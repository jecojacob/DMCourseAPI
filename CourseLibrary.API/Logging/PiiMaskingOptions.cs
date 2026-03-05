using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CourseLibrary.API.Logging
{
    public class PiiMaskingOptions
    {
        public PiiMaskingOptions()
        {
            MaskingPatterns = new List<PiiMaskingPattern>();
            RedactionText = "[REDACTED]";
            MiddleMask = "****";
            ShowFirstAndLastCharacter = true;
        }

        public IList<PiiMaskingPattern> MaskingPatterns { get; }

        public string RedactionText { get; set; }

        public string MiddleMask { get; set; }

        public bool ShowFirstAndLastCharacter { get; set; }

        public void AddDefaultPatterns()
        {
            MaskingPatterns.Add(new PiiMaskingPattern(
                "email",
                new Regex(@"\b[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}\b",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase)));

            MaskingPatterns.Add(new PiiMaskingPattern(
                "phone",
                new Regex(@"\b(?:\+?\d{1,3}[-.\s]?)?(?:\(?\d{3}\)?[-.\s]?){2}\d{4}\b",
                    RegexOptions.Compiled)));

            MaskingPatterns.Add(new PiiMaskingPattern(
                "ssn",
                new Regex(@"\b\d{3}-\d{2}-\d{4}\b",
                    RegexOptions.Compiled)));

            MaskingPatterns.Add(new PiiMaskingPattern(
                "creditCard",
                new Regex(@"\b(?:\d[ -]*?){13,19}\b",
                    RegexOptions.Compiled)));

            MaskingPatterns.Add(new PiiMaskingPattern(
                "bearerToken",
                new Regex(@"(?i)(?<=bearer\s)[A-Z0-9\-\._~\+\/]+=*",
                    RegexOptions.Compiled)));

            MaskingPatterns.Add(new PiiMaskingPattern(
                "jwt",
                new Regex(@"eyJ[A-Za-z0-9_-]+\.[A-Za-z0-9._-]+\.[A-Za-z0-9._-]+",
                    RegexOptions.Compiled)));
        }
    }

    public class PiiMaskingPattern
    {
        public PiiMaskingPattern(string name, Regex pattern, string replacement = null)
        {
            Name = name;
            Pattern = pattern;
            Replacement = replacement;
        }

        public string Name { get; }

        public Regex Pattern { get; }

        public string Replacement { get; }
    }
}
