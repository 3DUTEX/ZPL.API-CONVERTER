using System.Text.Json;
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
  public async Task<IActionResult> ZPLToPDF(IFormFile zpl)
  {
    try
    {
      using var sr = new StreamReader(zpl.OpenReadStream());

      var fileType = "application/pdf";

      var fileName = $"{Guid.NewGuid()}.pdf";

      var zplContent = await sr.ReadToEndAsync();

      return File(_zplConverterService.ZPLToPDF(zplContent), fileType, fileName);
    }
    catch (Exception ex)
    {
      var errorJSON = JsonSerializer.Serialize(new { Message = ex.Message, StackTrace = ex.StackTrace, Source = ex.Source });

      _logger.LogError(errorJSON);

      return BadRequest(errorJSON);
    }
  }
}