using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CourseLibrary.API.Logging
{
    public class MaskingLogger : ILogger
    {
        private readonly ILogger _innerLogger;
        private readonly IPiiMasker _piiMasker;

        public MaskingLogger(ILogger innerLogger, IPiiMasker piiMasker)
        {
            _innerLogger = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
            _piiMasker = piiMasker ?? throw new ArgumentNullException(nameof(piiMasker));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _innerLogger.BeginScope(MaskValue(state));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _innerLogger.IsEnabled(logLevel);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var originalMessage = formatter != null
                ? formatter(state, exception)
                : state?.ToString();

            var maskedMessage = _piiMasker.Mask(originalMessage);

            if (exception != null)
            {
                var maskedException = _piiMasker.Mask(exception.ToString());
                if (!string.IsNullOrWhiteSpace(maskedException))
                {
                    maskedMessage = string.IsNullOrWhiteSpace(maskedMessage)
                        ? maskedException
                        : string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}{2}",
                            maskedMessage,
                            Environment.NewLine,
                            maskedException);
                }
            }

            _innerLogger.Log(logLevel, eventId, maskedMessage, null, (renderedMessage, _) => renderedMessage);
        }

        private object MaskValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string stringValue)
            {
                return _piiMasker.Mask(stringValue);
            }

            if (value is IEnumerable<KeyValuePair<string, object>> properties)
            {
                var maskedProperties = new Dictionary<string, object>();
                foreach (var property in properties)
                {
                    maskedProperties[property.Key] = MaskValue(property.Value);
                }

                return maskedProperties;
            }

            return _piiMasker.Mask(value.ToString());
        }
    }
}
