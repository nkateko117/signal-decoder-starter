using Microsoft.AspNetCore.Mvc;
using SignalDecoder.Domain.Interfaces;

namespace SignalDecoder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceGeneratorService _generatorService;

    public DevicesController(IDeviceGeneratorService generatorService)
    {
        _generatorService = generatorService;
    }

    [HttpGet("generate")]
    public IActionResult Generate( [FromQuery] int count = 5, [FromQuery] int signalLength = 4, [FromQuery] int maxStrength = 9)
    {
        if (count < 1 || count > 100)
            return BadRequest("count must be between 1 and 100.");

        if (signalLength < 1 || signalLength > 20)
            return BadRequest("signalLength must be between 1 and 20.");

        if (maxStrength < 1 || maxStrength > 100)
            return BadRequest("maxStrength must be between 1 and 100.");

        var devices = _generatorService.GenerateDevices(count, signalLength, maxStrength);
        return Ok(devices);
    }
}