using SignalDecoder.Application.Services;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Tests;

public class SignalDecoderServiceTests
{
    private readonly SignalDecoderService _service = new();

    [Fact]
    public void Decode_FindsExactMatch()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [2, 4, 1, 3],
                ["D02"] = [7, 1, 5, 2],
                ["D03"] = [3, 6, 2, 8],
                ["D04"] = [1, 0, 9, 4],
                ["D05"] = [5, 2, 3, 1]
            },
            ReceivedSignal = [10, 12, 6, 12],
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.True(response.SolutionCount >= 1);
        Assert.Contains(response.Solutions, s =>
            s.TransmittingDevices.OrderBy(d => d).SequenceEqual(new[] { "D01", "D03", "D05" }));
    }

    [Fact]
    public void Decode_NoSolution_ReturnsEmpty()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [1, 1],
                ["D02"] = [2, 2]
            },
            ReceivedSignal = [99, 99],
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.Equal(0, response.SolutionCount);
        Assert.Empty(response.Solutions);
    }

    [Fact]
    public void Decode_EmptySignal_AllZeros()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [1, 2],
                ["D02"] = [3, 4]
            },
            ReceivedSignal = [0, 0],
            Tolerance = 0
        };

        var response = _service.Decode(request);

        // The empty set sums to [0,0], which matches
        Assert.True(response.SolutionCount >= 1);
        Assert.Contains(response.Solutions, s => s.TransmittingDevices.Count == 0);
    }

    [Fact]
    public void Decode_WithTolerance_FindsFuzzyMatches()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [5, 5],
                ["D02"] = [3, 3]
            },
            ReceivedSignal = [6, 6],
            Tolerance = 2
        };

        var response = _service.Decode(request);

        // D01=[5,5] is within ±2 of [6,6], and D02=[3,3] is within ±2 of [6,6]
        // Also D01+D02=[8,8] is within ±2 of [6,6]
        Assert.True(response.SolutionCount >= 2);
    }

    [Fact]
    public void Decode_SingleDevice_FindsIt()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [5, 10, 15]
            },
            ReceivedSignal = [5, 10, 15],
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.Equal(1, response.SolutionCount);
        Assert.Equal(["D01"], response.Solutions[0].TransmittingDevices);
    }

    [Fact]
    public void Decode_PerformanceWith15Devices_Under3Seconds()
    {
        // Generate 15 devices with small signals to create a realistic scenario
        var devices = new Dictionary<string, int[]>();
        var random = new Random(42); // Fixed seed for reproducibility
        for (int i = 1; i <= 15; i++)
        {
            devices[$"D{i:D2}"] = Enumerable.Range(0, 4).Select(_ => random.Next(0, 10)).ToArray();
        }

        // Create a signal that's the sum of a few devices
        int[] signal = new int[4];
        foreach (var key in new[] { "D01", "D05", "D10" })
        {
            for (int j = 0; j < 4; j++)
                signal[j] += devices[key][j];
        }

        var request = new DecodeRequest
        {
            Devices = devices,
            ReceivedSignal = signal,
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.True(response.SolveTimeMs < 3000, $"Took {response.SolveTimeMs}ms, expected < 3000ms");
        Assert.True(response.SolutionCount >= 1);
    }

    [Fact]
    public void Decode_AllDevicesActive()
    {
        var devices = new Dictionary<string, int[]>
        {
            ["D01"] = [1, 2],
            ["D02"] = [3, 4],
            ["D03"] = [5, 6]
        };

        int[] signal = [9, 12]; // 1+3+5, 2+4+6

        var request = new DecodeRequest
        {
            Devices = devices,
            ReceivedSignal = signal,
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.Contains(response.Solutions, s =>
            s.TransmittingDevices.OrderBy(d => d).SequenceEqual(new[] { "D01", "D02", "D03" }));
    }

    [Fact]
    public void Decode_SolveTimeIsPopulated()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [1, 2],
                ["D02"] = [3, 4]
            },
            ReceivedSignal = [4, 6],
            Tolerance = 0
        };

        var response = _service.Decode(request);

        Assert.True(response.SolveTimeMs >= 0);
    }
}