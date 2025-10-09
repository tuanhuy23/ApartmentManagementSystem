using ApartmentManagementSystem.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ApartmentManagementSystem.Identity
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        Dictionary<string, string> _permissionMap = new Dictionary<string, string>();
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _permissionMap.GetPermissions(typeof(RolePermissions));
            _permissionMap.GetPermissions(typeof(UserPermissions));
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (_permissionMap.TryGetValue(policyName, out var permission))
            {

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
            }
            throw new UnauthorizedAccessException();
        }
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    }
}
