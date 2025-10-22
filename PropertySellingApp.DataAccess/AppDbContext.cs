using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropertySellingApp.Models.Entities;

namespace PropertySellingApp.DataAccess
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

   

    public DbSet<User> Users => Set<User>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<VisitRequest> VisitRequests => Set<VisitRequest>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
        modelBuilder.Entity<Property>()
.HasOne(p => p.Seller)
.WithMany(u => u.Properties)
.HasForeignKey(p => p.SellerId)
.OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<VisitRequest>()
        .HasOne(v => v.Buyer)
        .WithMany(u => u.VisitRequests)
        .HasForeignKey(v => v.BuyerId)
        .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<VisitRequest>()
        .HasOne(v => v.Property)
        .WithMany(p => p.VisitRequests)
        .HasForeignKey(v => v.PropertyId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}


    }
