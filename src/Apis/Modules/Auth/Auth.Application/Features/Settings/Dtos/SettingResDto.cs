namespace Auth.Application.Features.Settings.Dtos;

public class SettingResDto : BaseDto
{
    public string Key { get; init; }
    public string Value { get; set; }
}