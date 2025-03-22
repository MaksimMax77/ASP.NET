using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.DataAccess.Data;

public class DataBaseContext : DbContext
{ 
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }


    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source=helloapp.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        FakeDataFactory.Generate();
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasData(FakeDataFactory.Roles);
        });
        
        modelBuilder.Entity<Employee>(entity =>
        {
            entity
                .HasOne(r => r.Role)
                .WithMany(e => e.Employees)
                .HasForeignKey(r => r.RoleId)
                .IsRequired();
            
            entity.HasData(FakeDataFactory.Employees);
        });
    }
}