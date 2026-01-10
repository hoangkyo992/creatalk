using System.Security.Cryptography;
using System.Text;

namespace Common.Infrastructure;

public static class PkceGenerator
{
    // Generates a secure, random code verifier (43-128 characters long)
    public static string GenerateCodeVerifier()
    {
        // rfc-7636 recommends a minimum of 43 characters and a maximum of 128 characters.
        // We'll use 32 bytes of random data, which produces a 43-character Base64Url string.
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Base64UrlEncodeNoPadding(randomBytes);
    }

    // Generates the code challenge from the code verifier using SHA256
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        byte[] verifierBytes = Encoding.UTF8.GetBytes(codeVerifier);
        byte[] challengeBytes;

        using (var sha256 = SHA256.Create())
        {
            challengeBytes = sha256.ComputeHash(verifierBytes);
        }

        return Base64UrlEncodeNoPadding(challengeBytes);
    }

    // A helper method for Base64Url encoding without padding
    private static string Base64UrlEncodeNoPadding(byte[] buffer)
    {
        string base64 = Convert.ToBase64String(buffer);
        // Converts base64 to base64url
        base64 = base64.Replace("+", "-");
        base64 = base64.Replace("/", "_");
        // Strips padding
        base64 = base64.Replace("=", "");

        return base64;
    }
}