using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Logging
{
    public class MaskingLoggerFactory : ILoggerFactory
    {
        private readonly LoggerFactory _innerFactory;
        private readonly IPiiMasker _piiMasker;

        public MaskingLoggerFactory(
            IEnumerable<ILoggerProvider> providers,
            IOptionsMonitor<LoggerFilterOptions> filterOptions,
            IOptions<LoggerFactoryOptions> factoryOptions,
            IPiiMasker piiMasker,
            IExternalScopeProvider externalScopeProvider = null)
        {
            _innerFactory = new LoggerFactory(providers, filterOptions, factoryOptions, externalScopeProvider);
            _piiMasker = piiMasker ?? throw new ArgumentNullException(nameof(piiMasker));
        }

        public ILogger CreateLogger(string categoryName)
        {
            var innerLogger = _innerFactory.CreateLogger(categoryName);
            return new MaskingLogger(innerLogger, _piiMasker);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _innerFactory.AddProvider(provider);
        }

        public void Dispose()
        {
            _innerFactory.Dispose();
        }
    }
}
