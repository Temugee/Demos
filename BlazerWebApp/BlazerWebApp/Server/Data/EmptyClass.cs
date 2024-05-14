using BlazerWebApp.Shared;
using Microsoft.EntityFrameworkCore;
namespace BlazerWebApp.Server.Data
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

//using System;
//using BlazerWebApp.Shared;
//using Microsoft.EntityFrameworkCore;

//namespace BlazerWebApp.Server.Data
//{
//    public class EmptyClass
//    {
//        public EmptyClass()
//        {
//        }
//    }
//}
