using AccountManagement.API;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.ItemMaster;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using AccountManagement.Repository.Interface.Repository.MasterList;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
using AccountManagement.Repository.Interface.Services.ItemMaster;
using AccountManagement.Repository.Interface.Services.FormPermissionMasterService;
using AccountManagement.Repository.Interface.Services.MasterList;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using AccountManagement.Repository.Repository.AuthenticationRepository;
using AccountManagement.Repository.Repository.ItemMasterRepository;
using AccountManagement.Repository.Repository.FormPermissionMasterRepository;
using AccountManagement.Repository.Repository.MasterListRepository;
using AccountManagement.Repository.Repository.SiteMasterRepository;
using AccountManagement.Repository.Services.Authentication;
using AccountManagement.Repository.Services.FormPermissionMaster;
using AccountManagement.Repository.Services.MasterList;
using AccountManagement.Repository.Services.SiteMaster;
using Microsoft.EntityFrameworkCore;
using AccountManagement.Repository.Services.ItemMaster;
using AccountManagement.Repository.Interface.Repository.Supplier;
using AccountManagement.Repository.Repository.SupplierRepository;
using AccountManagement.Repository.Interface.Services.SupplierService;
using AccountManagement.Repository.Services.Supplier;
using AccountManagement.Repository.Repository.CompanyRepository;
using AccountManagement.Repository.Interface.Repository.Company;
using AccountManagement.Repository.Interface.Services.CompanyService;
using AccountManagement.Repository.Services.Company;
using AccountManagement.Repository.Interface.Repository.PurchaseRequest;
using AccountManagement.Repository.Repository.PurchaseRequestRepository;
using AccountManagement.Repository.Interface.Services.PurchaseRequestService;
using AccountManagement.Repository.Services.PurchaseRequest;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Repository.InvoiceMasterRepository;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using AccountManagement.Repository.Services.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using AccountManagement.Repository.Repository.PurchaseOrderRepository;
using AccountManagement.Repository.Interface.Services.PurchaseOrderService;
using AccountManagement.Repository.Services.PurchaseOrder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DbaccManegmentContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("ACCDbconn")));


builder.Services.AddScoped<IAuthentication, UserAuthentication>();
builder.Services.AddScoped<IMasterList, MasterListRepo>();
builder.Services.AddScoped<IFormPermissionMaster, FormPermissionMasterRepo>();
builder.Services.AddScoped<ISiteMaster, SiteMasterRepo>();
builder.Services.AddScoped<IItemMaster, ItemMasterRepo>();
builder.Services.AddScoped<ISupplierMaster, SupplierMasterRepo>();
builder.Services.AddScoped<ICompany, CompanyRepo>();
builder.Services.AddScoped<IPurchaseRequest, PurchaseRequestRepo>();
builder.Services.AddScoped<IPurchaseOrder, PurchaseOrderRepo>();
builder.Services.AddScoped<IPurchaseOrderDetails, PurchaseOrderDetailsRepo>();
builder.Services.AddScoped<ISupplierInvoice, SupplierInvoiceRepo>();
builder.Services.AddScoped<ISupplierInvoiceDetails, SupplierInvoiceDetailsRepo>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IMasterListServices, MasterListService>();
builder.Services.AddScoped<ISiteMasterServices, SiteMasterService>();
builder.Services.AddScoped<IFormPermissionMasterService, FormPermissionMasterService>();
builder.Services.AddScoped<IItemMasterServices, ItemMasterServices>();
builder.Services.AddScoped<ISupplierServices, SupplierServices>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();
builder.Services.AddScoped<IPurchaseOrderServices, PurchaseOrderServices>();
builder.Services.AddScoped<IPurchaseOrderDetailsServices, PurchaseOrderDetailsServices>();
builder.Services.AddScoped<ISupplierInvoiceService, SupplierInvoiceService>();
builder.Services.AddScoped<ISupplierInvoiceDetailsService, SupplierInvoiceDetailsService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
