using Microsoft.AspNetCore.Authorization;

namespace ApartmentManagementSystem.Identity
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionAuthorizationHandler() { }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }
            var activeUser = context.User.Claims.Where(x => x.Type == "IsActive" &&
                                                                x.Value == "True" &&
                                                                x.Issuer == "ApartmentManagementSystem");
            if (!activeUser.Any())
            {
                context.Fail();
                return;
            }
            var permissionss = context.User.Claims.Where(x => x.Type == "Permission" &&
                                                                requirement.Permission == x.Value &&
                                                                x.Issuer == "ApartmentManagementSystem");
            if (permissionss.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
