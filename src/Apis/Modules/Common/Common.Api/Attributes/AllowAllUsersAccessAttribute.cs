namespace Common.Api.Attributes;

/// <summary>
///
/// </summary>
/// <seealso cref="AllowAllUsersAccessAttribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class AllowAllUsersAccessAttribute : Attribute
{
}