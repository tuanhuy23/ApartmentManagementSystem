using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Common
{
    public static class IdentityHelper
    {
        public static AccountInfoResponseDto GetIdentity(HttpContext httpContext)
        {
            if (httpContext == null) return null;
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return null;
            var claimUserId = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (claimUserId == null) return null;
            var accountInfo = new AccountInfoResponseDto()
            {
                Id = claimUserId.Value
            };
            var claimUserName = identity.FindFirst(ClaimTypes.Name);
            if (claimUserName != null) 
            {
                accountInfo.UserName = claimUserName.Value;
            }
            var claimEmail = identity.FindFirst(ClaimTypes.Email);
            if (claimEmail != null)
            {
                accountInfo.Email = claimEmail.Value;
            }
            var claimDisplayName = identity.FindFirst("DisplayName");
            if (claimDisplayName != null)
            {
                accountInfo.DisplayName = claimDisplayName.Value;
            }
            var claimRole = identity.FindFirst(ClaimTypes.Role);
            if (claimRole != null)
            {
                accountInfo.Role = claimRole.Value;
            }
            var claimPermission = identity.Claims.Where(c => c.Type == "Permission").Select(c => c.Value);
            if (claimPermission != null) 
            {
                accountInfo.Permissions = claimPermission;
            }
            return accountInfo;
        }
    }
}