
using ExampleApi.Bootstrapper;
using SimpleInjector;

var container = new Container();
var builder = WebApplication.CreateBuilder(args);

container.BuildWithCqrs(builder).Run();