using Api;
using Api.Models;
using Api.Services;
using FluentFTP;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AdminContext>();

builder.Services.AddSwaggerGen(opts => opts.EnableAnnotations());

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IProductTypesService, ProductTypesService>();
builder.Services.AddScoped<IProductSubTypeService, ProductSubTypeService>();
builder.Services.AddScoped<IProductItemService, ProductItemService>();

builder.Services.AddFtpClient(builder.Configuration);

builder.Services.AddRouting(opts => opts.LowercaseUrls = true);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AdminContext>();
await db.Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
