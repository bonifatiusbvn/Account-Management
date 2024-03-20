using AccountManagement.API;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using AccountManagement.Repository.Interface.Repository.MasterList;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
using AccountManagement.Repository.Interface.Services.FormPermissionMasterService;
using AccountManagement.Repository.Interface.Services.MasterList;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using AccountManagement.Repository.Repository.AuthenticationRepository;
using AccountManagement.Repository.Repository.FormPermissionMasterRepository;
using AccountManagement.Repository.Repository.MasterListRepository;
using AccountManagement.Repository.Repository.SiteMasterRepository;
using AccountManagement.Repository.Services.Authentication;
using AccountManagement.Repository.Services.FormPermissionMaster;
using AccountManagement.Repository.Services.MasterList;
using AccountManagement.Repository.Services.SiteMaster;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DbaccManegmentContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("ACCDbconn")));


builder.Services.AddScoped<IAuthentication, UserAuthentication>();
builder.Services.AddScoped<IMasterList, MasterListRepo>();
builder.Services.AddScoped<IFormPermissionMaster, FormPermissionMasterRepo>();
builder.Services.AddScoped<ISiteMaster, SiteMasterRepo>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IMasterListServices, MasterListService>();
builder.Services.AddScoped<ISiteMasterServices, SiteMasterService>();
builder.Services.AddScoped<IFormPermissionMasterService, FormPermissionMasterService>();


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
