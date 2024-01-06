using idee5.AspNetCore;
using idee5.Globalization.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace idee5.Globalization.WebApi {
    public static class MvcServiceCollectionExtensions {
        /// <summary>
        /// Add the idee5 globalization web api to the builder
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" /> to add builder to</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC builder</returns>
        public static IMvcBuilder AddLocalizationControllers(this IMvcBuilder builder) {

            // install the globalization controllers
            Assembly asm = typeof(AResourceRepository).Assembly;
            return builder
                .AddMvcOptions(opt => {
                    opt.Conventions.Add(new QueryHandlerRouteConvention());
                    opt.Conventions.Add(new CommandHandlerRouteConvention());
                })
                .ConfigureApplicationPartManager(apm => {
                    apm.FeatureProviders.Add(new QueryControllerProvider(asm));
                    apm.FeatureProviders.Add(new CommandControllerProvider(asm));
                });
        }
    }
}
