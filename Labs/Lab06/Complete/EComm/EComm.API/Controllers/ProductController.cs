using EComm.Core;
using EComm.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EComm.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRepository _repository;

        public ProductController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _repository.GetAllProducts();
        }
    }
}
