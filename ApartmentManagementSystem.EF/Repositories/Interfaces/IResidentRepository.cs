using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;

namespace ApartmentManagementSystem.EF.Repositories.Interfaces
{
    public interface IResidentRepository: IRepository<Resident>
    {
        
    }
}