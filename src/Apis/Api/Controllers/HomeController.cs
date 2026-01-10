using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Index()
    {
        var htmlContent = "<html><head><title>Creatalk API</title><meta name=\"zalo-platform-site-verification\" content=\"ElQUTz_sFMi3-ArpoEnvVpJXyb_OboO5EJOu\" /></head><body><h1>Welcome!!!</h1></body></html>";
        return Content(htmlContent, MediaTypeNames.Text.Html);
    }
}