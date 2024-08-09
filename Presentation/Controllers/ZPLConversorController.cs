using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ZPLConversorController : ControllerBase
{
  private readonly ILogger<ZPLConversorController> _logger;
  private readonly IZPLConverterService _zplConverterService;
  public ZPLConversorController(ILogger<ZPLConversorController> logger, IZPLConverterService zplConverterService)
  {
    _logger = logger;
    _zplConverterService = zplConverterService;
  }

  [HttpPost]
  public IActionResult ZPLToPDF(IFormFile zpl)
  {
    using var sr = new StreamReader(zpl.OpenReadStream());

    var fileType = "application/pdf";

    var fileName = $"{Guid.NewGuid()}.pdf";

    return File(_zplConverterService.ZPLToPDF(sr.ReadToEnd()), fileType, fileName);
  }
}