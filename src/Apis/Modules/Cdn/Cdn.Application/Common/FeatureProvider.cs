using Cdn.Application.Shared;

namespace Cdn.Application.Common;

public class FeatureProvider : IFeatureProvider
{
    public List<AppFeature> GetFeatures()
    {
        return
        [
            new(nameof(FeatureCodes.Cdn), FeatureCodes.Cdn.Albums, "Albums",
            [
                new(ActionCodes.View, "View albums"),
                new(ActionCodes.Create, "Create new albums"),
                new(ActionCodes.Update, "Update existing albums"),
                new(ActionCodes.Delete, "Delete albums"),
            ]),
            new(nameof(FeatureCodes.Cdn), FeatureCodes.Cdn.Library, "Library",
            [
                new(ActionCodes.View, "View files/folders"),
                new(ActionCodes.Delete, "Delete files/folders"),
                new(ActionCodes.Create, "Create new folders"),
                new(ActionCodes.Rename, "Rename existing files/folders"),
                new(ActionCodes.Move, "Move files/folders"),
                new(ActionCodes.Upload, "Upload files"),
                new(ActionCodes.Import, "Import files"),
                new(ActionCodes.GrantAccess, "Grant access")
            ])
        ];
    }
}