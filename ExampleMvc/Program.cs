
using System.Reflection;
using cqrsCore.Decorators.Command;
using cqrsCore.Extensions;
using cqrsCore.Logging;
using cqrsCore.Validation;
using ExampleMvc.CommandHandlers;
using Serilog;
using SimpleInjector;

var container = new Container();
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Verbose()
  .Enrich.FromLogContext()
  .CreateLogger();

// Add services to the container.
services.AddControllersWithViews();
services.AddLogging(b => b.AddSerilog());
services.AddSimpleInjector(container, opt =>
{
  opt.AddAspNetCore();
  opt.AddLogging();
});

container.AddCqrs()
  .AddCommandHandlers(GetCommandAssemblies())
  .DecorateWith(typeof(AsyncCommandHandlerDecorator<>))
  .And()
  .WithCqrsValidation<DataAnnotationValidator>()
  .Build();

container.Register<cqrsCore.Logging.ILogger, SerilogAdapter>();
container.Register(typeof(cqrsCore.Logging.ILogger<>), typeof(SerilogAdapter<>));
container.RegisterInstance<Serilog.ILogger>(Log.Logger);

var app = builder.Build();

app.Services.UseSimpleInjector(container);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

container.Verify();

app.Run();

// Helpers
List<Assembly> GetCommandAssemblies()
{
  List<Assembly> assemblies = new List<Assembly>();
  
  assemblies.Add(typeof(ExampleCommandHandler).Assembly);

  return assemblies;
}