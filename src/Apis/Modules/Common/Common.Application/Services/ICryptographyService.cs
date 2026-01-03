namespace Common.Application.Services;

public interface ICryptographyService
{
    string Decrypt(in object data);

    string Encrypt(in object data);
}