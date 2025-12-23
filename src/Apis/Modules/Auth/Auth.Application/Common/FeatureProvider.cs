namespace Auth.Application.Common;

public class FeatureProvider : IFeatureProvider
{
    public List<AppFeature> GetFeatures()
    {
        var features = new List<AppFeature>();

        features.Add(new AppFeature(nameof(FeatureCodes.Administration), FeatureCodes.Administration.Users, "Users", new List<AppFeatureAction>
        {
            new(ActionCodes.View, "View users"),
            new(ActionCodes.Create, "Create new users"),
            new(ActionCodes.Update, "Update existing users"),
            new(ActionCodes.Delete, "Delete users"),
            new(ActionCodes.ResetPassword, "Reset user's password")
        }));
        features.Add(new AppFeature(nameof(FeatureCodes.Administration), FeatureCodes.Administration.Roles, "Roles", new List<AppFeatureAction>
        {
            new(ActionCodes.View, "View roles"),
            new(ActionCodes.Create, "Create new roles"),
            new(ActionCodes.Update, "Update existing roles"),
            new(ActionCodes.Delete, "Delete roles"),
            new(ActionCodes.GrantAccess, "Grant access"),
        }));
        features.Add(new AppFeature(nameof(FeatureCodes.Administration), FeatureCodes.Administration.UserSessions, "User sessions", new List<AppFeatureAction>
        {
            new(ActionCodes.View, "View user sessions"),
        }));
        features.Add(new AppFeature(nameof(FeatureCodes.Administration), FeatureCodes.Administration.Activities, "User activities", new List<AppFeatureAction>
        {
            new(ActionCodes.View, "View user's activities"),
        }));
        features.Add(new AppFeature(nameof(FeatureCodes.Administration), FeatureCodes.Administration.Settings, "Settings", new List<AppFeatureAction>
        {
            new(ActionCodes.View, "View settings"),
            new(ActionCodes.Create, "Create new settings"),
            new(ActionCodes.Update, "Update existing settings"),
            new(ActionCodes.Delete, "Delete settings"),
        }));

        return features;
    }
}