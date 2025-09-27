using CostAdvisor.Core.Providers;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Services;
using CostAdvisor.Infrastructure.Data;
using CostAdvisor.Infrastructure.Providers;
using CostAdvisor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddScoped<ICostRepository, CostRepository>();
builder.Services.AddSingleton<ICloudBillingProvider, AWSBillingProviderDummy>();
builder.Services.AddSingleton<ICloudBillingProvider, AzureBillingProviderDummy>();
builder.Services.AddSingleton<ICloudBillingProvider, GCPBillingProviderDummy>();

builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>(); 
builder.Services.AddScoped<BillingService>();
builder.Services.AddDbContext<CostAdvisorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<OpenAIClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});
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



