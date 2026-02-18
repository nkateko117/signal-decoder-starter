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

    /// <summary>
    /// Generate a set of random devices with signal patterns.
    /// </summary>
    /// <param name="count">Number of devices (1-100, default 5)</param>
    /// <param name="signalLength">Signal pattern length (1-20, default 4)</param>
    /// <param name="maxStrength">Maximum signal value (1-100, default 9)</param>
    [HttpGet("generate")]
    [ProducesResponseType(typeof(Dictionary<string, int[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Generate(
        [FromQuery] int count = 5,
        [FromQuery] int signalLength = 4,
        [FromQuery] int maxStrength = 9)
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