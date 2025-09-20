using CostAdvisor.Core.Providers;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Services;
using CostAdvisor.Infrastructure.Providers;
using CostAdvisor.Infrastructure.Repositories;

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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapControllers();
app.UseHttpsRedirection();

app.Run();


