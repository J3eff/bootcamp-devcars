using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace DevCars.API.Controllers
{
    [Route("api/cars")]
    public class CarsControllers : ControllerBase
    {
        private readonly DevCarsDbContext _dbContext;

        public CarsControllers(DevCarsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET api/cars
        [HttpGet]
        public IActionResult Get()
        {
            // Retorna lista de CarItemViewModel
            var cars = _dbContext.Cars;

            var carsViewModel = cars
                .Where(c => c.Status == CarStatusEnum.Available)
                .Select(c => new CarItemViewModel(c.Id, c.Brand, c.Model, c.Price))                
                .ToList();
            return Ok(carsViewModel);
        }

        //GET api/cars/2
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // SE CRRO DE INDENTIFICADO ID NÃO EXISTIR, RETORNA NOTFOOUND
            //SENÃO, OK 

            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            var carDetailsViewModel = new CarDetailsViewModel(
                car.Id,
                car.Brand,
                car.Model,
                car.VinCode,
                car.Year,
                car.Price,
                car.Color,  
                car.ProductionDate
                );
            return Ok(carDetailsViewModel);
        }

        //POST api/cars
        [HttpPost]
        public IActionResult Post([FromBody] AddCarInputModel model)
        {
            //SE O CADASTRO FUNCIONAR, RETORNA CREATED (201)
            //SE OS DADOS DE ENTRADA ESTIVER INCORRETOS, RETORNA BAD REQUEST (400)
            //SE O CADASTRO FUNCIOINAR, MAIS NÃO TIVER UMA API DE CONSULTA, RETORNA NOCONTENT(204)
            if(model.Model.Length > 50)
            {
                return BadRequest("Modelo nãop pode ter mais de 50 caracteres.");
            }

            var car = new Car(model.VinCode, model.Brand, model.Model, model.Year, model.Price, model.Color, model.ProductionDate);

            _dbContext.Cars.Add(car);
            _dbContext.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = car.Id },
                model
                );
        }

        //PUT api/cars/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateCarInputModel model)
        {
            //SE ATUALIZAÇÃO FUNCIONAR, RETORNA 204 NO CONTENT
            //SE DADOS ENTRADA ESTIVEREM INCORRETOS, RETORNA 400 BAD REQUEST
            //SE NÃO EXISTIR, RETORNA NOT FOUND 404
            
            if (model.Price <= 0)
            {
                return BadRequest("Valor do veiculo não pode ser menor ou igual 0.");
            }           

            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);
            if(car == null)
            {
                return NotFound();
            }

            car.Update(model.Color, model.Price);

            _dbContext.SaveChanges();

            return NoContent();
        }

        //DELET api/cars/2
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //SE NÃO EXISTIR, RETORNA NOT FOUND 404
            //SE FOR COM SUCESSO, RETORNA NO CONTENT 204

            var car = _dbContext.Cars.SingleOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            car.SetAsSuspended();
            _dbContext.SaveChanges();

            return NoContent();
        }

    }
}
