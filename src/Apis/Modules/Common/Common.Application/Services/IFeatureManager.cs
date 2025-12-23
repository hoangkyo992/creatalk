using Common.Application.Common.Models;

namespace Common.Application.Services;

public interface IFeatureManager
{
    List<AppFeature> GetFeatures();
}