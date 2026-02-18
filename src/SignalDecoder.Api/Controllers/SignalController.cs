using Microsoft.AspNetCore.Mvc;
using SignalDecoder.Domain.Interfaces;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SignalController : ControllerBase
{
    private readonly ISignalSimulatorService _simulatorService;
    private readonly ISignalDecoderService _decoderService;

    public SignalController(
        ISignalSimulatorService simulatorService,
        ISignalDecoderService decoderService)
    {
        _simulatorService = simulatorService;
        _decoderService = decoderService;
    }

    /// <summary>
    /// Simulate signal transmission by randomly selecting active devices.
    /// </summary>
    [HttpPost("simulate")]
    [ProducesResponseType(typeof(SimulateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Simulate([FromBody] SimulateRequest request)
    {
        if (request.Devices == null || request.Devices.Count == 0)
            return BadRequest("Devices must not be null or empty.");

        var patterns = request.Devices.Values.ToList();
        int expectedLength = patterns[0].Length;

        if (patterns.Any(p => p.Length != expectedLength))
            return BadRequest("All signal patterns must have the same length.");

        if (patterns.Any(p => p.Any(v => v < 0)))
            return BadRequest("All signal values must be non-negative.");

        var response = _simulatorService.Simulate(request.Devices);
        return Ok(response);
    }

    /// <summary>
    /// Decode a received signal to identify which devices transmitted it.
    /// </summary>
    [HttpPost("decode")]
    [ProducesResponseType(typeof(DecodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Decode([FromBody] DecodeRequest request)
    {
        if (request.Devices == null || request.Devices.Count == 0)
            return BadRequest("Devices must not be null or empty.");

        if (request.ReceivedSignal == null || request.ReceivedSignal.Length == 0)
            return BadRequest("ReceivedSignal must not be null or empty.");

        if (request.Tolerance < 0)
            return BadRequest("Tolerance must be non-negative.");

        var patterns = request.Devices.Values.ToList();
        int expectedLength = patterns[0].Length;

        if (patterns.Any(p => p.Length != expectedLength))
            return BadRequest("All signal patterns must have the same length.");

        if (request.ReceivedSignal.Length != expectedLength)
            return BadRequest("ReceivedSignal length must match device pattern length.");

        var response = _decoderService.Decode(request);
        return Ok(response);
    }
}