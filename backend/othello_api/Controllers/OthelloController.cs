using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using othello_api.Models;
using othello_api.Services;

namespace othello_api.Controllers;

[ApiController]
[Route("[controller]")]
public class OthelloController : ControllerBase
{
    private readonly ILogger<OthelloController> _logger;
    private readonly OthelloService _othelloService;
    private readonly IHubContext<GameHub> _hubContext;

    public OthelloController(
        ILogger<OthelloController> logger,
        OthelloService othelloService,
        IHubContext<GameHub> hubContext
    )
    {
        _logger = logger;
        _othelloService = othelloService;
        _hubContext = hubContext;
    }

    [HttpGet("test")]
    public ActionResult<string> Test() => Ok("testing api 2");
}
