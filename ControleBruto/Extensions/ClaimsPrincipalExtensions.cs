using System.Security.Claims;

namespace ControleBruto.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            //return user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        }
    }
}
