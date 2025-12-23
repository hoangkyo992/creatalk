using System.Text.Json;
using Auth.Application.Features.Settings.Dtos;
using Auth.Application.Shared.Settings;

namespace Auth.Application.Actions;

public class UpdateSettingValueAction() : IMappingAction<Setting, SettingResDto>
{
    public void Process(Setting source, SettingResDto destination, ResolutionContext context)
    {
        if (destination.Key == SettingKeys.General)
        {
            DoCheck<GeneralSetting>(destination, (jsonObject) =>
            {
                if (jsonObject != null)
                {
                    //.
                }
            });
        }
        else if (destination.Key == SettingKeys.Media)
        {
            DoCheck<MediaSetting>(destination, (jsonObject) =>
            {
                if (jsonObject != null)
                {
                    //.
                }
            });
        }
    }

    private static void DoCheck<T>(SettingResDto destination, Action<T> action)
    {
        if (!string.IsNullOrWhiteSpace(destination.Value))
        {
            try
            {
                var jsonObject = destination.Value.FromJson<T>();
                if (!object.Equals(jsonObject, default(T)))
                {
                    action.Invoke(jsonObject);
                    destination.Value = jsonObject.ToJson(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
            }
            catch
            {
                // Skip
            }
        }
        else
        {
            destination.Value = Activator.CreateInstance<T>().ToJson();
        }
    }
}