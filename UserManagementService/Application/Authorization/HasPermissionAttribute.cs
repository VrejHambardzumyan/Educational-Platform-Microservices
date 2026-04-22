using Microsoft.AspNetCore.Authorization;

namespace UserManagementService.Application.Authorization
{
    /// <summary>
    /// Requires the caller's JWT to contain the specified permission in its <c>permissions</c> claim.
    /// Translates to an ASP.NET Core named policy so it composes with <see cref="AuthorizeAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
