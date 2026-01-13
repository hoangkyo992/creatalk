using System.Security.Cryptography;
using System.Text;
using Common.Application.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

        // Normalize input
        string input = data.ToString() ?? string.Empty;
        input = input.Replace("%2B", "+").Replace("%20", " ");
        byte[] cipherBytes = Convert.FromBase64String(input);

        // Derive key and IV using PBKDF2
        byte[] salt = Encoding.ASCII.GetBytes(_encryptionKey);
        byte[] key = KeyDerivation.Pbkdf2(
            password: _encryptionKey,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        byte[] iv = KeyDerivation.Pbkdf2(
            password: _encryptionKey,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 16);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs, Encoding.Unicode);

        return reader.ReadToEnd();
    }

    public string Encrypt(in object data)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Convert input to bytes
        byte[] clearBytes = Encoding.Unicode.GetBytes(data.ToString() ?? string.Empty);

        // Derive key and IV using PBKDF2
        byte[] salt = Encoding.ASCII.GetBytes(_encryptionKey);
        byte[] key = KeyDerivation.Pbkdf2(
            password: _encryptionKey,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        byte[] iv = KeyDerivation.Pbkdf2(
            password: _encryptionKey,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 16);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(clearBytes, 0, clearBytes.Length);
        }

        return Convert.ToBase64String(ms.ToArray())
                     .Replace("+", "%2B")
                     .Replace(" ", "%20");
    }
}