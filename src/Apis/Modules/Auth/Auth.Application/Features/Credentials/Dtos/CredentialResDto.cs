namespace Auth.Application.Features.Credentials.Dtos;

public class CredentialResDto : BaseDto
{
    public string Key { get; init; }
    public string Value { get; set; }
}