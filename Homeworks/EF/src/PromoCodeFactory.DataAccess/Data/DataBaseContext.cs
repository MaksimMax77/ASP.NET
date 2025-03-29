using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data;

public class DataBaseContext : DbContext
{
    private Dictionary<Type, ICollection> _entities = new();
    
    public DbSet<Role> Roles { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }

    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
        var promocodes = PromoCodes
            .Include(c => c.Preference)
            .Include(c => c.PartnerManager)
            .ToList();
        
        var employees = Employees
            .Include(e => e.Role)
            .ToList();

        var customers = Customers
            .Include(c => c.PromoCode)
            .Include(customer => customer.Preferences)
            .ToList();

        var preferences = Preferences.ToList();
        
        _entities.Add(typeof(PromoCode), promocodes);
        _entities.Add(typeof(Employee), employees);
        _entities.Add(typeof(Customer), customers); // todo весь код конструктора нужно перенести в отдельный класс, включая метод TryGetEntities
        _entities.Add(typeof(Preference), preferences);
    }
 
    public bool TryGetEntities<T>(out ICollection<T> entities)
    {
        entities = _entities[typeof(T)] as ICollection<T>;
        return entities != null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source=helloapp.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        FakeDataFactory.Generate();
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Role)
            .WithMany(e => e.Employees)
            .HasForeignKey(r => r.RoleId)
            .IsRequired();
        
        modelBuilder.Entity<Preference>()
            .HasMany(p => p.PromoCodes)
            .WithOne(p => p.Preference)
            .HasForeignKey(p => p.PreferenceId)
            .IsRequired();

        modelBuilder.Entity<PromoCode>()
            .HasOne(promoCode => promoCode.PartnerManager)
            .WithMany(e => e.PromoCodes)
            .HasForeignKey(promoCode => promoCode.PartnerManagerId)
            .IsRequired();
        
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.PromoCode)
            .WithMany(p => p.Customers)
            .HasForeignKey(c => c.PromoCodeId)
            .IsRequired();
        
        modelBuilder
            .Entity<Preference>()
            .HasMany(p => p.Customers)
            .WithMany(s => s.Preferences)
            .UsingEntity<CustomerPreference>(
                j => j
                    .HasOne(cp => cp.Customer)
                    .WithMany(c => c.CustomerPreference)
                    .HasForeignKey(cp => cp.CustomerId),
                j => j
                    .HasOne(cp => cp.Preference)
                    .WithMany(p => p.CustomerPreference)
                    .HasForeignKey(cp => cp.PreferenceId),
                j =>
                {
                    j.HasKey(cp => new { cp.PreferenceId, cp.CustomerId });
                });
        
        modelBuilder.Entity<Role>().HasData(FakeDataFactory.Roles);
        modelBuilder.Entity<Employee>().HasData(FakeDataFactory.Employees);
        modelBuilder.Entity<Preference>().HasData(FakeDataFactory.Preferences);
        modelBuilder.Entity<PromoCode>().HasData(FakeDataFactory.PromoCodes);
        modelBuilder.Entity<Customer>().HasData(FakeDataFactory.Customers);
        modelBuilder.Entity<CustomerPreference>().HasData(FakeDataFactory.CustomerPreferences);
    }
}