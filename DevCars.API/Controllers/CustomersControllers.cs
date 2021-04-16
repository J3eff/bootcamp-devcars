using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace DevCars.API.Controllers
{
    [Route("api/customers")]
    public class CustomersControllers : ControllerBase
    {
        private readonly DevCarsDbContext _dbContext;
        public CustomersControllers(DevCarsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // api/customers
        /// <summary>
        /// Cadastra um cliente
        /// </summary>
        /// <remarks>
        /// Requisição de Exemplo:
        /// {
        ///     "fullName": "Jefferson,
        ///     "document": 47136322851,
        ///     "birthDate": "1999-03-08T11:00:49.752Z"
        /// }
        /// </remarks>
        /// <returns>Não tem retorno.</returns>
        /// <response code="204">Cliente cadastrado.</response>
        /// <response code="500">Parametros incorretos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] AddCustomerInputModel model)
        {
            var customer = new Customer(model.FullName, model.Document, model.BirthDate);

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();

            return NoContent();
        }

        // api/customers/2/orders
        /// <summary>
        /// Cadastra um pedido 
        /// </summary>
        /// <remarks>
        /// Requisição de Exemplo:
        /// {
        ///    "idCar": 2,
        ///    "idCustomer": 1,
        ///     "extraItems": [
        ///         {
        ///          "description": "Teto Solar",
        ///          "price": 5000
        ///          }
        ///        ]
        ///  }
        /// </remarks>
        /// <param name="id"> Identificador de um cliente </param>
        /// <param name="model"> Identificador de um carro </param>
        /// <returns>Não tem retorno.</returns>
        /// <response code="201">Pedido cadastrado</response>
        /// <response code="500">Parametros incorretos.</response>
        [HttpPost("{id}/orders")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostOrder(int id, [FromBody] AddOrderInputModel model)
        {
            var extraItems = model.ExtraItems
                .Select(e => new ExtraOderItem(e.Description, e.Price))
                .ToList();

            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == model.IdCar);

            var order = new Order(model.IdCar, model.IdCustomer, car.Price, extraItems);

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.IdCustomer, orderid = order.Id },
                model
                );
        }

        //GET api/customters/1/orders/3
        [HttpGet("{id}/orders/{orderid}")]
        public IActionResult GetOrder(int id, int orderid)
        {
            var order = _dbContext.Orders
                .Include(o => o.ExtraItems)
                .SingleOrDefault(o => o.Id == orderid);

            if (order == null)
            {
                return NotFound();
            }

            var extraItems = order
                .ExtraItems
                .Select(e => e.Description)
                .ToList();

            var orderViewModel = new OrderDetailsViewModel(order.Id, order.IdCustomer, order.TotalCost, extraItems);
            return Ok(orderViewModel);
        }

    }
}
