using Microsoft.EntityFrameworkCore;
using apbd_kolokwium_probne_2.Services;
using apbd_kolokwium_probne_2.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddDbContext<OrdersContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{ app.MapOpenApi();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/openapi/v1.json", "v1");
    }); 
}

app.UseAuthorization();

app.MapControllers();

app.Run();