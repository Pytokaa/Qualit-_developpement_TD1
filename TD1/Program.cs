using TD1.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;
using TD1.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProduitDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProduitDb")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProduitDbContext>();
builder.Services.AddScoped<ProduitManager>();
builder.Services.AddScoped<IDataRepository<Produit>, ProduitManager>();




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
