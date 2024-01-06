using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace idee5.Globalization.Web {
    internal static class ServiceCollectionExtensions {
        public static void RegisterHandlers(this IServiceCollection services, Type handlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) {
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes.Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType && !t.Name.Contains("Validat", StringComparison.Ordinal)
                && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)));
            foreach (var item in implementations) {
                var service = new ServiceDescriptor(item.GetInterfaces().Single(i => i.GetGenericTypeDefinition() == handlerType), item, serviceLifetime);
                services.Add(service);
            }
        }
    }
}