using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Dtos
{
    public class UpdateApartmentDto
    {
        public double Area { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
    }
}