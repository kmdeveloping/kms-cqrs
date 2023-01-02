using System.Diagnostics;
using cqrsCore;
using ExampleMvc.CommandContracts;
using Microsoft.AspNetCore.Mvc;
using ExampleMvc.Models;

namespace ExampleMvc.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly ICqrsManager _cqrsManager;

  public HomeController(ILogger<HomeController> logger, ICqrsManager cqrsManager)
  {
    _logger = logger;
    _cqrsManager = cqrsManager;
  }

  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [HttpGet]
  public async Task<IActionResult> TestCqrs()
  {
    await _cqrsManager.ExecuteAsync(new ExampleCommand
    {
      ExampleName = "MRBeast",
      TimeStamp = DateTime.Now,
      ExecuteAsNoOp = false,
      ContextData = new Dictionary<string, object>()
    }, CancellationToken.None);

    return Ok();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
  }
}