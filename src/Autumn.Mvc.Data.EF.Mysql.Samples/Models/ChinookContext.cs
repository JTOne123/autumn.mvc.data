using Microsoft.EntityFrameworkCore;

namespace Autumn.Mvc.Data.EF.Mysql.Samples.Models
{
    /// <summary>
    /// Entity DbContext fot chinook database
    /// </summary>
    public class ChinookContext : DbContext
    {
        public ChinookContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .ToTable("Artist")
                .Property(a => a.Id).HasColumnName("ArtistId");
            
            modelBuilder.Entity<Genre>()
                .ToTable("Genre")
                .Property(o=>o.Id).HasColumnName("Genreid");

            modelBuilder.Entity<Album>()
                .ToTable("Elbum")
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

            modelBuilder.Entity<InvoiceLine>()
                .ToTable("InvoiceLine")
                .Property(a => a.Id).HasColumnName("InvoiceLineId");

            modelBuilder.Entity<MediaType>()
                .ToTable("MediaType")
                .Property(a => a.Id).HasColumnName("MediaTypeId");

            modelBuilder.Entity<Playlist>()
                .ToTable("Playlist")
                .Property(a => a.Id).HasColumnName("PlaylistId");

            modelBuilder.Entity<Track>()
                .ToTable("Track")
                .Property(a => a.Id).HasColumnName("TrackId");

        }
    }
}