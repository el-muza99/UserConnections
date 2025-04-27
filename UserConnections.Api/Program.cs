using System.Reflection;
using UserConnections.Application;
using UserConnections.Infrastructure;
using UserConnections.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services including MediatR handlers
builder.Services.AddApplicationServices();

// Register infrastructure services including DbContext
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Apply migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserConnectionDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();