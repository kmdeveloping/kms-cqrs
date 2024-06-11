using System.Diagnostics;
using CqrsFramework;
using ExampleMvc.CommandContracts;
using Microsoft.AspNetCore.Mvc;
using ExampleMvc.Models;
using ExampleMvc.QueryContracts;

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
  
  public async Task<IActionResult> Testing()
  {
    ExampleQuery query = new ExampleQuery
    {
      Name = "johnny"
    };

    var task = _cqrsManager.ExecuteAsync(query, CancellationToken.None);

    TestingViewModel v = new TestingViewModel
    {
      EmailAddress = task.Result
    };
    
    return View(v);
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
  }
}