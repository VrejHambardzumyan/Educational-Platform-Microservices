using Microsoft.AspNetCore.Authorization;

namespace UserManagementService.Application.Authorization
{
    /// <summary>Authorization requirement that demands a specific permission claim in the JWT.</summary>
    public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
