using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Producer;

public class AppDbContext : DbContext
{
    //public const string ConnectionString = "Server=localhost;Database=CapAppDB;User Id=sa;Password=Passw@rd;TrustServerCertificate=true;MultipleActiveResultSets=true";
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CapMessage> CapMessages { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder
    //         .EnableSensitiveDataLogging()
    //         .UseSqlServer(ConnectionString);
    // }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapMessage>()
            .Property(b => b.Message)
            .IsRequired();
    }
}