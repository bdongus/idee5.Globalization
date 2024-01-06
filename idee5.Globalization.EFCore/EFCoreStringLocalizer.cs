using idee5.Globalization.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Globalization.EFCore;
public class EFCoreStringLocalizer : DatabaseStringLocalizer {
    private readonly IDbContextFactory<GlobalizationDbContext> _contextFactory;

    public EFCoreStringLocalizer(IDbContextFactory<GlobalizationDbContext> contextFactory, DatabaseResourceManager resourceManager, IOptions<LocalizationParlanceOptions> options) : base(resourceManager, options) {
        _contextFactory = contextFactory;
    }

    protected override List<string> GetResourceNames(string resourceSet, string languageId, bool includeParentCultures) {
        ArgumentNullException.ThrowIfNull(resourceSet);
        ArgumentNullException.ThrowIfNull(languageId);

        ASpec<Models.Resource> languageSpec = includeParentCultures
            ? Specifications.OfLanguageOrFallback(languageId)
            : Specifications.OfLanguage(languageId);

        var predicate = Specifications.InResourceSet(resourceSet) & languageSpec & Specifications.CustomerParlance(_customer) & Specifications.IndustryParlance(_industry);
        var context = _contextFactory.CreateDbContext();
        return context.Resources.AsNoTracking().Where(predicate).Select(r => r.Id).Distinct().ToList();
    }
}
