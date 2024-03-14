using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.API;

public partial class DbaccManegmentContext : DbContext
{
    public DbaccManegmentContext()
    {
    }

    public DbaccManegmentContext(DbContextOptions<DbaccManegmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<RolewiseFormPermission> RolewiseFormPermissions { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Company");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Gstno)
                .HasMaxLength(17)
                .IsFixedLength()
                .HasColumnName("GSTNo");
            entity.Property(e => e.PanNo)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.ToTable("Form");

            entity.Property(e => e.FormGroup).HasMaxLength(10);
            entity.Property(e => e.FormName).HasMaxLength(20);
        });

        modelBuilder.Entity<RolewiseFormPermission>(entity =>
        {
            entity.ToTable("RolewiseFormPermission");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Site");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.ContectPersonName).HasMaxLength(50);
            entity.Property(e => e.ContectPersonPhoneNo).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.ShippingAddress).HasMaxLength(100);
            entity.Property(e => e.ShippingArea).HasMaxLength(100);
            entity.Property(e => e.ShippingPincode).HasMaxLength(10);
            entity.Property(e => e.SiteName).HasMaxLength(250);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(40);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.PhoneNo).HasMaxLength(100);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(20);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("UserRole");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(10);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
