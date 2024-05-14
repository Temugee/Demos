using System;
using Microsoft.EntityFrameworkCore;
using PhoneShopSharedLibrary.Models;

namespace PhoneShopServer.Data
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<UserAccount> UserAccounts { get; set; } = default!;
        public DbSet<SystemRole> SystemRoles { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
        public DbSet<TokenInfo> TokenInfo { get; set; } = default!;
    }
}

