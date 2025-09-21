using CostAdvisor.Core.Providers;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Services;
using CostAdvisor.Infrastructure.Data;
using CostAdvisor.Infrastructure.Providers;
using CostAdvisor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<ICostRepository, CostRepository>();
builder.Services.AddSingleton<ICloudBillingProvider, AWSBillingProviderDummy>();
builder.Services.AddSingleton<ICloudBillingProvider, AzureBillingProviderDummy>();
builder.Services.AddSingleton<ICloudBillingProvider, GCPBillingProviderDummy>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddDbContext<CostAdvisorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CostAdvisorDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapControllers();
app.UseHttpsRedirection();

app.Run();


