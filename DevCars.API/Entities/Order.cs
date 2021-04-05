using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCars.API.Entities
{
    public class Order
    {
        protected Order() { }
        public Order(int idCar, int idCustomer, decimal price, List<ExtraOderItem> items)
        {
            IdCar = idCar;
            IdCustomer = idCustomer;
            TotalCost = items.Sum(i => i.Price) + price;

            ExtraItems = items;
        }

        public int Id { get; private set; }
        public int IdCar { get; private set; }
        public int IdCustomer { get; private set; }
        public decimal TotalCost { get; private set; }
        public Car Car { get; set; }        
        public Customer Customer  { get; private set; }
        public List<ExtraOderItem> ExtraItems { get; private set; }
    }

    public class ExtraOderItem
    {
        protected ExtraOderItem() { }
        public ExtraOderItem(string description, decimal price)
        {
            Description = description;
            Price = price;
        }

        public int Id { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int IdOrder { get; private set; }
    }
}
