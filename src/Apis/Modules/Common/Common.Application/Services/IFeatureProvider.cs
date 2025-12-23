using Common.Application.Common.Models;

namespace Common.Application.Services;

public interface IFeatureProvider
{
    List<AppFeature> GetFeatures();
}