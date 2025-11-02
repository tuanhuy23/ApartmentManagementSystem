using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApartmentManagementSystem.Filters
{
    public class ApartmentBuildingValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accountInfo = IdentityHelper.GetIdentity(context.HttpContext);
            if (accountInfo == null) await next();

            if (!context.RouteData.Values.TryGetValue("appartmentBuildingId", out var apartmentBuildingIdValue) || apartmentBuildingIdValue == null) await next();

            var apartmentBuildingId = apartmentBuildingIdValue.ToString();

            if (!accountInfo.ApartmentBuildingId.Equals(apartmentBuildingId))
            {
                throw new DomainException(ErrorCodeConsts.NoPermissionAccessApartmentBuilding, ErrorCodeConsts.NoPermissionAccessApartmentBuilding, System.Net.HttpStatusCode.Forbidden);
            }
            await next();
        }
    }
}