using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddJsonFile("appsettings.json");

builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = false;
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "DefaultPolicy",
                      policy =>
                      {
                          policy
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});

var app = builder.Build();

app.UseCors("DefaultPolicy");

var routeGroups = app
    .MapGroup("api/v1")
    .WithOpenApi();

app.MapEndpoints(routeGroups);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();