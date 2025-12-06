using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ApartmentManagementSystem.DbContext.Entity
{
    public class AppRole: IdentityRole
    {
        public string? AppartmentBuildingId { get; set; }
    }
}