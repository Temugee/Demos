using Microsoft.AspNetCore.Mvc;
using PhoneShopServer.Repositories;
using PhoneShopSharedLibrary.Models;
using PhoneShopSharedLibrary.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhoneShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory categoryService;
        public CategoryController(ICategory categoryService)
        {
            this.categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var categories = await categoryService.GetAllCategories();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddCategory(Category model)
        {
            if (model is null) return BadRequest("Model is null");
            var response = await categoryService.AddCategory(model);
            return Ok(response);
        }
    }
}

