using HungHd.Shared;

namespace Auth.Application.Shared;

public static class CacheKeys
{
    public static class Auth
    {
        public static readonly string Data = nameof(Data);
        public static readonly string IsssuedTime = nameof(IsssuedTime);

        public static string GetAuthKey(long sessionId)
        {
            return $"Creatalk.Api:Auth:{ZCode.Get(sessionId)}";
        }

        public static string GetSignInAttemptKey(long authId)
        {
            return $"Creatalk.Api:SignInAttempt:{ZCode.Get(authId)}";
        }
    }
}