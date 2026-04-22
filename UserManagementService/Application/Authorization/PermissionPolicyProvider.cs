using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace UserManagementService.Application.Authorization
{
    /// <summary>
    /// Intercepts policy lookups whose name contains a dot and converts them into a
    /// <see cref="PermissionRequirement"/> policy on the fly, so that
    /// <c>[HasPermission("users.manage")]</c> works without pre-registering every policy.
    /// All other policy names fall through to the default provider.
    /// </summary>
    public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            => _fallback = new DefaultAuthorizationPolicyProvider(options);

        /// <summary>Builds a permission policy for dot-namespaced names; delegates everything else.</summary>
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.Contains('.'))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            _fallback.GetFallbackPolicyAsync();
    }
}
