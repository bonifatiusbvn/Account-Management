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

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<ItemMaster> ItemMasters { get; set; }

    public virtual DbSet<PodeliveryAddress> PodeliveryAddresses { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }

    public virtual DbSet<RolewiseFormPermission> RolewiseFormPermissions { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<SupplierInvoice> SupplierInvoices { get; set; }

    public virtual DbSet<SupplierInvoiceDetail> SupplierInvoiceDetails { get; set; }

    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }

    public virtual DbSet<UnitMaster> UnitMasters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__F2D21B7648F1A50D");

            entity.Property(e => e.CityId).ValueGeneratedNever();
            entity.Property(e => e.CityName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StateId).HasColumnName("State_Id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_state_id");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.Property(e => e.CompanyId).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Gstno)
                .HasMaxLength(17)
                .HasColumnName("GSTNo");
            entity.Property(e => e.PanNo).HasMaxLength(10);
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D1609F608A2BD0");

            entity.Property(e => e.CountryId).ValueGeneratedNever();
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValueSql("('')")
                .IsFixedLength();
            entity.Property(e => e.CountryName)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.ToTable("Form");

            entity.Property(e => e.FormGroup).HasMaxLength(10);
            entity.Property(e => e.FormName).HasMaxLength(20);
        });

        modelBuilder.Entity<ItemMaster>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.ToTable("ItemMaster");

            entity.Property(e => e.ItemId).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Gstamount)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("GSTAmount");
            entity.Property(e => e.Gstper)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("GSTPer");
            entity.Property(e => e.Hsncode)
                .HasMaxLength(10)
                .HasColumnName("HSNCode");
            entity.Property(e => e.IsWithGst).HasColumnName("IsWithGST");
            entity.Property(e => e.ItemName).HasMaxLength(50);
            entity.Property(e => e.PricePerUnit).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.UnitTypeNavigation).WithMany(p => p.ItemMasters)
                .HasForeignKey(d => d.UnitType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItemMaster_UnitMaster");
        });

        modelBuilder.Entity<PodeliveryAddress>(entity =>
        {
            entity.HasKey(e => e.Aid).HasName("PK_PODelevryAddress");

            entity.ToTable("PODeliveryAddress");

            entity.Property(e => e.Aid).HasColumnName("AId");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Poid).HasColumnName("POId");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("PurchaseOrder");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BillingAddress).HasMaxLength(500);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.DeliveryShedule).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Poid)
                .HasMaxLength(100)
                .HasColumnName("POId");
            entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalDiscount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalGstamount)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("TotalGSTAmount");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.FromSupplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.FromSupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrder_SupplierMaster");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PurchaseOrderDetails_1");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Gst)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("GST");
            entity.Property(e => e.Item).HasMaxLength(100);
            entity.Property(e => e.ItemTotal).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.PorefId).HasColumnName("PORefId");
            entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Poref).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.PorefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrderDetails_PurchaseOrder");

            entity.HasOne(d => d.UnitType).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.UnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrderDetails_UnitMaster");
        });

        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.Pid);

            entity.ToTable("PurchaseRequest");

            entity.Property(e => e.Pid)
                .ValueGeneratedNever()
                .HasColumnName("PId");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Item).HasMaxLength(250);
            entity.Property(e => e.PrNo).HasMaxLength(50);
            entity.Property(e => e.Quantity).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Site).WithMany(p => p.PurchaseRequests)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseRequest_Site");

            entity.HasOne(d => d.UnitType).WithMany(p => p.PurchaseRequests)
                .HasForeignKey(d => d.UnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseRequest_UnitMaster");
        });

        modelBuilder.Entity<RolewiseFormPermission>(entity =>
        {
            entity.ToTable("RolewiseFormPermission");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Form).WithMany(p => p.RolewiseFormPermissions)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolewiseFormPermission_UserRole");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.ToTable("Site");

            entity.Property(e => e.SiteId).ValueGeneratedNever();
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

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StatesId).HasName("PK__States__AC838DA89CEB4784");

            entity.Property(e => e.StatesId).ValueGeneratedNever();
            entity.Property(e => e.CountryId).HasColumnName("Country_id");
            entity.Property(e => e.StatesName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_country_id");
        });

        modelBuilder.Entity<SupplierInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId);

            entity.ToTable("SupplierInvoice");

            entity.Property(e => e.InvoiceId).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeliveryShedule).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Roundoff).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalDiscount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalGstamount)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("TotalGSTAmount");
            entity.Property(e => e.TotalPrice).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Site).WithMany(p => p.SupplierInvoices)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierInvoice_Site");
        });

        modelBuilder.Entity<SupplierInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailsId);

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Gst)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("GST");
            entity.Property(e => e.Gstamount)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("GSTAmount");
            entity.Property(e => e.Item)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.RefInvoice).WithMany(p => p.SupplierInvoiceDetails)
                .HasForeignKey(d => d.RefInvoiceId)
                .HasConstraintName("FK_SupplierInvoiceDetails_SupplierInvoice");

            entity.HasOne(d => d.UnitType).WithMany(p => p.SupplierInvoiceDetails)
                .HasForeignKey(d => d.UnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierInvoiceDetails_UnitMaster");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SupplierId);

            entity.ToTable("SupplierMaster");

            entity.Property(e => e.SupplierId).ValueGeneratedNever();
            entity.Property(e => e.AccountNo).HasMaxLength(25);
            entity.Property(e => e.Area).HasMaxLength(250);
            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.BuildingName).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Gstno)
                .HasMaxLength(20)
                .HasColumnName("GSTNo");
            entity.Property(e => e.Iffccode)
                .HasMaxLength(10)
                .HasColumnName("IFFCCode");
            entity.Property(e => e.Mobile).HasMaxLength(15);
            entity.Property(e => e.PinCode).HasMaxLength(7);
            entity.Property(e => e.SupplierName).HasMaxLength(30);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.CityNavigation).WithMany(p => p.SupplierMasters)
                .HasForeignKey(d => d.City)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierMaster_Cities");

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.SupplierMasters)
                .HasForeignKey(d => d.State)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierMaster_States");
        });

        modelBuilder.Entity<UnitMaster>(entity =>
        {
            entity.HasKey(e => e.UnitId);

            entity.ToTable("UnitMaster");

            entity.Property(e => e.UnitName).HasMaxLength(15);
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
