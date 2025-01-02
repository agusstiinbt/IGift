using System.Security.Claims;

namespace IGift.Client.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Email).Value.ToString();

        internal static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Name).Value.ToString();

        internal static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.Surname).Value.ToString();

        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirst(ClaimTypes.MobilePhone).Value.ToString();

        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal)
           => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
    }
}
