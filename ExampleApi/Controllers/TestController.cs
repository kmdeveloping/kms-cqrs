using cqrsCore;
using ExampleApi.Commands;
using ExampleApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
  private ICqrsManager _cqrs;

  public TestController(ICqrsManager cqrs)
  {
    _cqrs = cqrs;
  }

  [HttpPost("/test")]
  public async Task<IActionResult> PostTest([FromBody] PostTestModel data)
  {
    ApiExampleCommand command = new ApiExampleCommand
    {
      inputName = data.Name,
      TimeStamp = DateTime.Now,
    };

    await _cqrs.ExecuteAsync(command, CancellationToken.None);
    
    return Ok(command.Result);
  }
}