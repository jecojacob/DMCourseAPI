using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace CourseLibrary.API.Logging
{
    public static class PiiMaskingLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddGlobalPiiMaskingLogging(
            this IServiceCollection services,
            Action<PiiMaskingOptions> configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new PiiMaskingOptions();
            options.AddDefaultPatterns();
            configureOptions?.Invoke(options);

            services.AddSingleton(options);
            services.TryAddSingleton<IPiiMasker, RegexPiiMasker>();

            services.Replace(ServiceDescriptor.Singleton<ILoggerFactory, MaskingLoggerFactory>());

            return services;
        }
    }
}
