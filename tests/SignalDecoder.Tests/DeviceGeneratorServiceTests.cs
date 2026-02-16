using SignalDecoder.Application.Services;

namespace SignalDecoder.Tests;

public class DeviceGeneratorServiceTests
{
    private readonly DeviceGeneratorService _service = new();

    [Fact]
    public void GenerateDevices_ReturnsCorrectCount()
    {
        var devices = _service.GenerateDevices(5, 4, 9);

        Assert.Equal(5, devices.Count);
    }

    [Fact]
    public void GenerateDevices_AllPatternsHaveCorrectLength()
    {
        var devices = _service.GenerateDevices(5, 4, 9);

        Assert.All(devices.Values, signal => Assert.Equal(4, signal.Length));
    }

    [Fact]
    public void GenerateDevices_ValuesWithinRange()
    {
        var devices = _service.GenerateDevices(10, 6, 9);

        Assert.All(devices.Values, signal =>
            Assert.All(signal, value =>
            {
                Assert.InRange(value, 0, 9);
            })
        );
    }

    [Fact]
    public void GenerateDevices_DeviceIdsAreFormatted()
    {
        var devices = _service.GenerateDevices(3, 4, 9);

        Assert.Contains("D01", devices.Keys);
        Assert.Contains("D02", devices.Keys);
        Assert.Contains("D03", devices.Keys);
    }

    [Fact]
    public void GenerateDevices_SingleDevice()
    {
        var devices = _service.GenerateDevices(1, 2, 5);

        Assert.Single(devices);
        Assert.Contains("D01", devices.Keys);
    }
}