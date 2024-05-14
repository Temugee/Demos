using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using BlazorSaleWeb.Server.Data;
//using BlazorSaleWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using WebApplication1;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private IConfiguration _configuration;

        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //private readonly SaleDbContext dbContext;
        //public InfoController(SaleDbContext context)
        //{
        //    dbContext = context;
        //}
        //[Route("GetItems")]
        //[HttpGet]
        //public async Task<IList<Item>> GetItems()
        //{
        //    try
        //    {
        //        var _data = await dbContext.Items.ToListAsync();
        //        return _data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}
        [HttpGet]
        [Route("GetItems")]
        public JsonResult GetItem()
        {
            string query = "select * from dbo.Item";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
                return new JsonResult(table);
            }
        }
        [HttpGet]
        [Route("GetItems/{PkId}")]
        public JsonResult GetItempkId(int PkId)
        {
            string query = "select * from dbo.Item where PkId=@PkId";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@PkId", PkId); // Corrected parameter name
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
                return new JsonResult(table); // Return the data
            }
        }
        [HttpDelete]
        [Route("DeleteItems")]
        public JsonResult DeleteNotes(int id)
        {
            string query = "delete from dbo.Item where id=@id";
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id); // Corrected parameter name
                    myCommand.ExecuteNonQuery(); // Use ExecuteNonQuery for DELETE operation
                }
                myCon.Close();
            }
            return new JsonResult("Deleted Successfully");
        }
        [HttpPost]
        [Route("UpdateItem")]
        public JsonResult UpdateItem(Item item)
        {
            try
            {
                string query = "UPDATE dbo.Item SET Id=@Id, Name=@Name, Barcode=@Barcode, Price=@Price WHERE PkId=@PkId";
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        // Add parameters for the update query
                        myCommand.Parameters.AddWithValue("@Id", item.Id);
                        myCommand.Parameters.AddWithValue("@Name", item.Name);
                        myCommand.Parameters.AddWithValue("@Barcode", item.Barcode);
                        myCommand.Parameters.AddWithValue("@Price", item.Price);
                        myCommand.Parameters.AddWithValue("@PkId", item.PkId);

                        myCommand.ExecuteNonQuery();
                    }
                    myCon.Close();
                }

                return new JsonResult("Updated Successfully");
            }
            catch (Exception ex)
            {
                return new JsonResult($"Error updating item: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("SaveItem")]
        public JsonResult SaveItem(Item item)
        {
            try
            {
                // Generate a unique identifier for PkId
                item.PkId = Guid.NewGuid().ToString();

                string query = "INSERT INTO dbo.Item (PkId, Id, Name, Barcode, Price) VALUES (@PkId, @Id, @Name, @Barcode, @Price)";
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        // Add parameters for the insert query
                        myCommand.Parameters.AddWithValue("@PkId", item.PkId);
                        myCommand.Parameters.AddWithValue("@Id", item.Id);
                        myCommand.Parameters.AddWithValue("@Name", item.Name);
                        myCommand.Parameters.AddWithValue("@Barcode", item.Barcode);
                        myCommand.Parameters.AddWithValue("@Price", item.Price);

                        myCommand.ExecuteNonQuery();
                    }
                    myCon.Close();
                }

                return new JsonResult("Saved Successfully");
            }
            catch (Exception ex)
            {
                return new JsonResult($"Error saving item: {ex.Message}");
            }
        }




    }
}
