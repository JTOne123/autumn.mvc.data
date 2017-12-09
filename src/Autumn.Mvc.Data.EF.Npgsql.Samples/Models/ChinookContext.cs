using Microsoft.EntityFrameworkCore;

namespace Autumn.Mvc.Data.EF.Npgsql.Samples.Models
{
    public class ChinookContext : DbContext
    {
        public ChinookContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .ToTable("Artist")
                .Property(a=>a.Id).HasColumnName("ArtistId");
            
            modelBuilder.Entity<Genre>()
                .ToTable("Genre")
                .Property(o=>o.Id).HasColumnName("GenreId");

            modelBuilder.Entity<Album>()
                .ToTable("Album")
                .Property(a => a.Id).HasColumnName("AlbumId");
            
            modelBuilder.Entity<Customer>()
                .ToTable("Customer")
                .Property(a => a.Id).HasColumnName("CustomerId");
            
            modelBuilder.Entity<Employee>()
                .ToTable("Employee")
                .Property(a => a.Id).HasColumnName("EmployeeId");
            
            modelBuilder.Entity<Invoice>()
                .ToTable("Invoice")
                .Property(a => a.Id).HasColumnName("InvoiceId");
        }
    }
}