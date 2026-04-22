using Microsoft.AspNetCore.Authorization;

namespace UserManagementService.Application.Authorization
{
    /// <summary>
    /// Grants access when the authenticated user's JWT contains a <c>permissions</c> claim
    /// whose value matches the <see cref="PermissionRequirement.Permission"/>.
    /// </summary>
    public sealed class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var hasPermission = context.User.Claims
                .Any(c => c.Type == "permissions" && c.Value == requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
