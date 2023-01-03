
using Serilog;
using SimpleInjector;

var container = new Container();
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddLogging(b => b.AddSerilog());
services.AddSimpleInjector(container, opt =>
{
  opt.AddAspNetCore().AddControllerActivation();
  opt.AddLogging();
});


var app = builder.Build();

app.Services.UseSimpleInjector(container);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();