using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.InputModels
{
    public class AddOrderInputModel
    {
        public int IdCar { get; set; }
        public int IdCustomer { get; set; }
        public List<ExtraItemsInputModel> ExtraItems { get; set; }
    }

    public class ExtraItemsInputModel
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
