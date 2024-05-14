using BlazerWebApp.Server.Data;
using BlazerWebApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazerWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class InfoController : ControllerBase
    {
        private readonly SaleDbContext dbContext;
        public InfoController(SaleDbContext context)
        {
            dbContext = context;
        }
        [Route("GetItems")]
        [HttpGet]
        public async Task<IList<Item>> GetItems()
        {
            try
            {
                var _data = await dbContext.Items.ToListAsync();
                return _data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Route("GetItem/{pkId}")]
        [HttpGet]
        public async Task<Item> GetItem(string pkId)
        {
            try
            {
                var _data = dbContext.Items.FirstOrDefault(a => a.PkId == pkId);
                return _data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Route("SaveItem")]
        [HttpPost]
        public async Task<IActionResult> SaveItem(Item Item)
        {
            try
            {
                if (Item != null)
                {
                    dbContext.Add(Item);
                    await dbContext.SaveChangesAsync();
                    return Ok("Save Successfully!!");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return NoContent();
        }
        [Route("UpdateItem")]
        [HttpPost]
        public async Task<IActionResult> UpdateItem(Item Item)
        {
            dbContext.Entry(Item).State = EntityState.Modified;
            try
            {
                await dbContext.SaveChangesAsync();
                return Ok("Update Successfully!!");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ItemExists(Item.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw (ex);
                }
            }
        }
        [HttpDelete("DeleteItem/{pkId}")]
        public async Task<IActionResult> DeleteItem(string pkId)
        {
            if (dbContext.Items == null)
            {
                return NotFound();
            }
            var item = dbContext.Items.FirstOrDefault(a => a.PkId == pkId);
            if (item == null)
            {
                return NotFound();
            }
            dbContext.Items.Remove(item);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        private bool ItemExists(string pkId)
        {
            return (dbContext.Items?.Any(e => e.PkId == pkId)).GetValueOrDefault();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using BlazerWebApp.Server.Data;
//using BlazerWebApp.Shared;
//using Microsoft.AspNetCore.Mvc;

//namespace BlazerWebApp.Server.Controllers
//{
//    public class InfoController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}

