using System.Security.Cryptography;
using System.Text;
using Common.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Common.Infrastructure.Services;

internal sealed class CryptographyService : ICryptographyService
{
    private readonly string _encryptionKey;

    public CryptographyService(IConfiguration configuration)
    {
        _encryptionKey = configuration.GetValue("EncryptionSettings:EncryptionKey", string.Empty);
        if (string.IsNullOrEmpty(_encryptionKey))
            throw new ArgumentException("Missing EncryptionKey in configuration settings");
    }

    public string Decrypt(in object data)
    {
        ArgumentNullException.ThrowIfNull(data);
        byte[] cipherBytes = Convert.FromBase64String((data.ToString() ?? string.Empty).Replace("%2B", "+").Replace("%20", " "));
        using var encryptor = Aes.Create();
#pragma warning disable SYSLIB0041 // Type or member is obsolete
        using var encryptionData = new Rfc2898DeriveBytes(_encryptionKey, Encoding.ASCII.GetBytes(_encryptionKey));
#pragma warning restore SYSLIB0041 // Type or member is obsolete
        encryptor.IV = encryptionData.GetBytes(16);
        encryptor.Key = encryptionData.GetBytes(32);
        encryptor.Mode = CipherMode.CBC;
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherBytes, 0, cipherBytes.Length);
        cs.Close();
        return Encoding.Unicode.GetString(ms.ToArray());
    }

    public string Encrypt(in object data)
    {
        ArgumentNullException.ThrowIfNull(data);
        byte[] clearBytes = Encoding.Unicode.GetBytes(data.ToString() ?? string.Empty);
        using var encryptor = Aes.Create();
#pragma warning disable SYSLIB0041 // Type or member is obsolete
        using var encryptionData = new Rfc2898DeriveBytes(_encryptionKey, Encoding.ASCII.GetBytes(_encryptionKey));
#pragma warning restore SYSLIB0041 // Type or member is obsolete
        encryptor.Key = encryptionData.GetBytes(32);
        encryptor.IV = encryptionData.GetBytes(16);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(clearBytes, 0, clearBytes.Length);
        cs.Close();
        return Convert.ToBase64String(ms.ToArray()).Replace("+", "%2B").Replace(" ", "%20");
    }
}