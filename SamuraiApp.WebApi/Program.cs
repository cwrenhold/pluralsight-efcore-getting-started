using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<SamuraiContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("SamuraiConnex"))
        .EnableSensitiveDataLogging()
        // Default to not tracking to improve performance
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
