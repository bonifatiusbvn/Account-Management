using AccountManagement.API;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
using AccountManagement.Repository.Repository.AuthenticationRepository;
using AccountManagement.Repository.Services.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DbaccManegmentContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("ACCDbconn")));


builder.Services.AddScoped<IAuthentication, UserAuthentication>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


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
