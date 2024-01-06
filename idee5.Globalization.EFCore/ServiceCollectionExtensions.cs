using idee5.Globalization.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace idee5.Globalization.EFCore;
public static class ServiceCollectionExtensions {
    public static void AddEFCoreLocalization(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptions) {
        ArgumentNullException.ThrowIfNull(dbOptions);

        services.AddSingleton<IStringLocalizerFactory, EFCoreStringLocalizerFactory>();
        services.AddDbContextFactory<GlobalizationDbContext>(dbOptions);
        // add the data access implementations to the DI container
        services.AddScoped<IResourceQueryRepository, ResourceQueryRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>()
            .AddScoped<IResourceUnitOfWork, ResourceUnitOfWork>();
    }
}
