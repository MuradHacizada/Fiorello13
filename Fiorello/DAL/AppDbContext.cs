﻿using Fiorello.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
}
