using Cms.Application.Shared;

namespace Cms.Application.Common;

public class FeatureProvider : IFeatureProvider
{
    public List<AppFeature> GetFeatures()
    {
        var features = new List<AppFeature>
        {
            new AppFeature(nameof(FeatureCodes.Cms), FeatureCodes.Cms.Attendees, "Attendees", new List<AppFeatureAction>
            {
                new(ActionCodes.View, "View attendees"),
                new(ActionCodes.Create, "Create new attendees"),
                new(ActionCodes.Update, "Update existing attendees"),
                new(ActionCodes.Delete, "Delete attendees")
            }),
            new AppFeature(nameof(FeatureCodes.Cms), FeatureCodes.Cms.MessageProviders, "MessageProviders", new List<AppFeatureAction>
            {
                new(ActionCodes.View, "View message providers"),
                new(ActionCodes.Create, "Create new message providers"),
                new(ActionCodes.Update, "Update existing message providers"),
                new(ActionCodes.Delete, "Delete message providers"),
            })
        };
        return features;
    }
}