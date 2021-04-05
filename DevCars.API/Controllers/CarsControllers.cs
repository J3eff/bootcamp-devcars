using Dapper;
using DevCars.API.Entities;
using DevCars.API.InputModels;
using DevCars.API.Persistence;
using DevCars.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;


namespace DevCars.API.Controllers
{
    [Route("api/cars")]
    public class CarsControllers : ControllerBase
    {
        private readonly DevCarsDbContext _dbContext;
        private readonly string _connectionString;

        public CarsControllers(DevCarsDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            
            // SE EU UTILIZAR O DBCONTEXT, E UTILZIAR O INMEMORY - VAI DAR ERRO
            //_connectionString = _dbContext.Database.GetDbConnection().ConnectionString; 
            _connectionString = configuration.GetConnectionString("DevCarsCs");
        }

        //GET api/cars
        [HttpGet]
        public IActionResult Get()
        {
            // Retorna lista de CarItemViewModel
            //var cars = _dbContext.Cars;

            //var carsViewModel = cars
            //    .Where(c => c.Status == CarStatusEnum.Available)
            //    .Select(c => new CarItemViewModel(c.Id, c.Brand, c.Model, c.Price))                
            //    .ToList();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Id, Brand, Model, Price FROM Cars WHERE Status = 0";

                var carsViewModel = sqlConnection.Query<CarItemViewModel>(query);

                return Ok(carsViewModel);
            }
                
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
        /// <summary>
        /// Cadastrar um carro
        /// </summary>
        /// <remarks>
        /// Requisição de exemplo:
        /// {
        ///     "brand": "Honda",
        ///     "model": "Civic",
        ///     "vinCode": "abc123",
        ///     "year": 2021,
        ///     "color": "Cinza",
        ///     "productionDate": "2021-04-05"
        /// } 
        /// </remarks>
        /// <param name="model">Dados de um novo Carro</param>
        /// <returns>Objeto recém-criados</returns>
        /// <response code="201">Objeto criado com sucesso.</response>
        /// <response code="400">Dados invaldos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
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
        /// <summary>
        /// Atualizar dados de um Carro
        /// </summary>
        /// <remarks>
        /// Requisição de Exemplo:
        /// {
        ///     "color": "Vermelho,
        ///     "price": 100000
        /// }
        /// </remarks>
        /// <param name="id">Identificado de um carro</param>
        /// <param name="model">Dados de alteração</param>
        /// <returns>Não tem retorno.</returns>
        /// <response code="204">Atualização bem-sucedida</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Carro não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Cars SET Color = @color, Price = @price WHERE Id = @id";

                sqlConnection.Execute(query, new { color = car.Color, price = car.Price, car.Id });
            }

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
