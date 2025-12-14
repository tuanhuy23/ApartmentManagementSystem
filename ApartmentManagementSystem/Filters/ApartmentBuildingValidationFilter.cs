using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApartmentManagementSystem.Filters
{
    public class ApartmentBuildingValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accountInfo = IdentityHelper.GetIdentity(context.HttpContext);
 
            if (accountInfo == null) {
                await next();
                return;
            }

            if (!context.RouteData.Values.TryGetValue("apartmentBuildingId", out var apartmentBuildingIdValue) || apartmentBuildingIdValue == null) {
                await next();
                return;
            }

            var apartmentBuildingId = apartmentBuildingIdValue.ToString();

            if (!accountInfo.ApartmentBuildingId.Equals(apartmentBuildingId))
            {
                throw new DomainException(ErrorCodeConsts.NoPermissionAccessApartmentBuilding, ErrorMessageConsts.NoPermissionAccessApartmentBuilding, System.Net.HttpStatusCode.Forbidden);
            }
            if ("root".Equals(apartmentBuildingId))
            {
                await next();
                return;
            }
            var apartmentBuildingData = context.HttpContext.RequestServices.GetRequiredService<ApartmentBuildingData>();
            if (apartmentBuildingData == null)
            {
                await next();
                return;
            } 
            if (!apartmentBuildingData.CheckIsExist(apartmentBuildingId))
            {
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
            }
            await next();
        }
    }
}