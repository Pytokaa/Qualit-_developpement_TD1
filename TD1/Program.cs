using TD1.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using TD1.DTO;
using TD1.Mapper;
using TD1.Models;
using TD1.Models.EntityFramework;
using TD1.Repository;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProduitDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProduitDb")));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<ProduitDbContext>();
builder.Services.AddScoped<ProduitManager>();
builder.Services.AddScoped<IDataRepository<Produit>, ProduitManager>();
builder.Services.AddScoped<TypeProduitManager>();
builder.Services.AddScoped<IDataRepository<TypeProduit>, TypeProduitManager>();
builder.Services.AddScoped<MarqueManager>();
builder.Services.AddScoped<IDataRepository<Marque>, MarqueManager>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});



var app = builder.Build();
app.UseCors("AllowBlazorDev");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();
