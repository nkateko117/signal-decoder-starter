using SignalDecoder.Application.Services;

namespace SignalDecoder.Tests;

public class SignalSimulatorServiceTests
{
    private readonly SignalSimulatorService _service = new();

    [Fact]
    public void Simulate_ReturnsValidResponse()
    {
        var devices = new Dictionary<string, int[]>
        {
            ["D01"] = [2, 4, 1, 3],
            ["D02"] = [7, 1, 5, 2],
            ["D03"] = [3, 6, 2, 8]
        };

        var response = _service.Simulate(devices);

        Assert.NotNull(response.ReceivedSignal);
        Assert.Equal(4, response.SignalLength);
        Assert.Equal(3, response.TotalDevices);
        Assert.InRange(response.ActiveDeviceCount, 1, 3);
    }

    [Fact]
    public void Simulate_SignalLengthMatchesDevicePatterns()
    {
        var devices = new Dictionary<string, int[]>
        {
            ["D01"] = [1, 2, 3],
            ["D02"] = [4, 5, 6]
        };

        var response = _service.Simulate(devices);

        Assert.Equal(3, response.ReceivedSignal.Length);
    }

    [Fact]
    public void Simulate_CombinedSignalIsNonNegative()
    {
        var devices = new Dictionary<string, int[]>
        {
            ["D01"] = [2, 4, 1],
            ["D02"] = [7, 1, 5],
            ["D03"] = [3, 6, 2]
        };

        var response = _service.Simulate(devices);

        Assert.All(response.ReceivedSignal, v => Assert.True(v >= 0));
    }

    [Fact]
    public void Simulate_SingleDevice_ReturnsDeviceSignal()
    {
        var devices = new Dictionary<string, int[]>
        {
            ["D01"] = [5, 10, 15]
        };

        var response = _service.Simulate(devices);

        // With only one device, the combined signal must equal that device's signal
        Assert.Equal(1, response.ActiveDeviceCount);
        Assert.Equal([5, 10, 15], response.ReceivedSignal);
    }
}