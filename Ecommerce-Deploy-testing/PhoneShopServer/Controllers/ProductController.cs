using Microsoft.AspNetCore.Mvc;
using PhoneShopServer.Repositories;
using PhoneShopSharedLibrary.Models;
using PhoneShopSharedLibrary.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct productService;
        public ProductController(IProduct productService)
        {
            this.productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts(bool featured)
        {
            var products = await productService.GetAllProducts(featured); return Ok(products);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddProduct(Product model)
        {
            if (model is null) return BadRequest("Model is null");
            var response = await productService.AddProduct(model);
            return Ok(response);
        }
    }
}

