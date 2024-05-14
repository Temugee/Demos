using System.Collections.Generic;
using System.Reflection.Emit;
//using BlazorSaleWeb.Shared;
using Microsoft.EntityFrameworkCore;
using WebApplication1;

namespace WebApplication1.Controllers
{
    public class SaleDbContext : DbContext
    {
        public SaleDbContext(DbContextOptions<SaleDbContext> options)
        : base(options)
        {
        }
        public virtual DbSet<Item> Items { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Item");
            base.OnModelCreating(modelBuilder);
        }
    }
}