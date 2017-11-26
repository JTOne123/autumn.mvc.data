using Microsoft.EntityFrameworkCore;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// Entity DbContext fot chinook database
    /// </summary>
    public class ChinookContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .ToTable("artist")
                .Property(a => a.Id).HasColumnName("artistid")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Genre>()
                .ToTable("genre")
                .Property(o=>o.Id).HasColumnName("genreid")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Album>()
                .ToTable("album")
                .Property(a => a.Id).HasColumnName("albumid")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Customer>()
                .ToTable("customer")
                .Property(a => a.Id).HasColumnName("customerid")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Employee>()
                .ToTable("employee")
                .Property(a => a.Id).HasColumnName("employeeid")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Invoice>()
                .ToTable("invoice")
                .Property(a => a.Id).HasColumnName("invoiceid")
                .ValueGeneratedOnAdd();
            
            
        }
    }
}