using Common.Application.Common.Models;
using Common.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Services;

public class FeatureManager(IServiceProvider serviceProvider) : IFeatureManager
{
    public List<AppFeature> GetFeatures()
    {
        var features = new List<AppFeature>();
        var providers = serviceProvider.GetServices<IFeatureProvider>();
        foreach (var provider in providers)
        {
            features.AddRange(provider.GetFeatures());
        }
        return features;
    }
}