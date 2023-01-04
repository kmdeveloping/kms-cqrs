using System.Reflection;
using cqrsCore.Decorators.Command;
using cqrsCore.Decorators.Event;
using cqrsCore.Decorators.Query;
using cqrsCore.Extensions;
using cqrsCore.Logging;
using cqrsCore.Validation;
using ExampleApi.CommandHandlers;
using Serilog;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace ExampleApi.Bootstrapper;

public static class BootstrapApi
{
  public static WebApplication BuildWithCqrs(this Container container, WebApplicationBuilder builder)
  {
    var services = builder.Services;
    
    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Verbose()
      .Enrich.FromLogContext()
      .WriteTo.Console()
      .CreateLogger();
    
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddLogging(b => b.AddSerilog());
    services.AddSimpleInjector(container, opt =>
    {
      opt
        .AddAspNetCore()
        .AddControllerActivation();
      opt.AddLogging();
    });

    List<Assembly> assemblies = GetAssemblies();

    container
      .AddCqrs()
      .AddDefaultCqrs(opt => opt.Assemblies = assemblies)
      .Build()
      .AddCqrsCoreLogging();

    var app = builder.Build();
    
    app.Services.UseSimpleInjector(container);
    
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    return app;
  }

  private static List<Assembly> GetAssemblies()
  {
    List<Assembly> assemblyList = new List<Assembly>();

    Assembly apiExampleCommandHandlerAssembly = typeof(ApiExampleCommandHandler).Assembly;
    
    assemblyList.Add(apiExampleCommandHandlerAssembly);

    return assemblyList;
  }
}