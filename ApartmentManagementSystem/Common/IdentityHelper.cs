using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
            var claimApartmentBuildingId = identity.FindFirst("ApartmentBuildingId");
            if (claimApartmentBuildingId != null)
            {
                accountInfo.ApartmentBuildingId = claimApartmentBuildingId.Value;
            }
            var claimRole = identity.FindFirst(ClaimTypes.Role);
            if (claimRole != null)
            {
                accountInfo.RoleId = claimRole.Value;
            }
            var claimRoleName = identity.FindFirst("RoleName");
            if (claimRoleName != null)
            {
                accountInfo.RoleName = claimRoleName.Value;
            }
             var claimIsActive = identity.FindFirst("IsActive");
            if (claimIsActive != null)
            {
                accountInfo.IsActive = claimIsActive.Value;
            }
            var claimPermission = identity.Claims.Where(c => c.Type == "Permission").Select(c => c.Value);
            if (claimPermission != null) 
            {
                accountInfo.Permissions = claimPermission;
            }
            var apartmentId = identity.FindFirst("ApartmentId");
            if (apartmentId != null)
            {
                accountInfo.ApartmentId = apartmentId.Value;
            }
            return accountInfo;
        }

        public static string GenerateRandomPassword(int length = 8)
    {
        if (length < 4) throw new ArgumentException("Length must be at least 4 to include all required types.");

        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specials = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        StringBuilder password = new StringBuilder();
        
        password.Append(GetRandomChar(upperCase));
        password.Append(GetRandomChar(specials));
        password.Append(GetRandomChar(digits));
        password.Append(GetRandomChar(lowerCase));

        string allChars = upperCase + lowerCase + digits + specials;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(GetRandomChar(allChars));
        }

        return new string(password.ToString().ToCharArray().OrderBy(s => Guid.NewGuid()).ToArray());
    }

    private static char GetRandomChar(string charSet)
    {
        byte[] randomByte = new byte[1];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomByte);
        }
        int index = randomByte[0] % charSet.Length;
        return charSet[index];
    }
    }
}