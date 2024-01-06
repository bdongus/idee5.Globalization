using idee5.Globalization.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace idee5.Globalization.EFCore;

public class EFCoreStringLocalizerFactory : DatabaseStringLocalizerFactory {
    private readonly IDbContextFactory<GlobalizationDbContext> _contextFactory;

    public EFCoreStringLocalizerFactory(IOptions<LocalizationParlanceOptions> options, IDbContextFactory<GlobalizationDbContext> contextFactory) : base(options) {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override DatabaseStringLocalizer CreateResourceManagerStringLocalizer(string resourceSet) {
        ArgumentNullException.ThrowIfNull(resourceSet);

        var databaseResourceManager = new DatabaseResourceManager(new ResourceRepository(_contextFactory.CreateDbContext()), resourceSet, _options.Value.Industry, _options.Value.Customer);
        return new EFCoreStringLocalizer(_contextFactory, databaseResourceManager, _options);
    }
}
