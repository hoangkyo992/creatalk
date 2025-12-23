using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Common;

public static class PasswordBuilder
{
    public class PUser
    {
    }

    public static bool Validate(string password, string hashedPassword)
    {
        var passwordHasher = new PasswordHasher<PUser>();
        return passwordHasher.VerifyHashedPassword(new PUser { }, hashedPassword, password) == PasswordVerificationResult.Success;
    }

    public static string Create(string password)
    {
        var passwordHasher = new PasswordHasher<PUser>();
        return passwordHasher.HashPassword(new PUser { }, password);
    }
}