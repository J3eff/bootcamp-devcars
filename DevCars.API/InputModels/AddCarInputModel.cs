using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.InputModels
{
    public class AddCarInputModel
    {        
        public string Brand { get; set; }
        public string Model { get; set; }
        public string VinCode { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}
