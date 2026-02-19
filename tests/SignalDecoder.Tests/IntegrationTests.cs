using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ── Generate Endpoint ──

    [Fact]
    public async Task Generate_DefaultParams_Returns5Devices()
    {
        var response = await _client.GetAsync("/api/devices/generate");
        response.EnsureSuccessStatusCode();

        var devices = await response.Content.ReadFromJsonAsync<Dictionary<string, int[]>>(JsonOptions);

        Assert.NotNull(devices);
        Assert.Equal(5, devices.Count);
    }

    [Fact]
    public async Task Generate_CustomParams_ReturnsCorrectCount()
    {
        var response = await _client.GetAsync("/api/devices/generate?count=10&signalLength=6&maxStrength=5");
        response.EnsureSuccessStatusCode();

        var devices = await response.Content.ReadFromJsonAsync<Dictionary<string, int[]>>(JsonOptions);

        Assert.NotNull(devices);
        Assert.Equal(10, devices.Count);
        Assert.All(devices.Values, signal => Assert.Equal(6, signal.Length));
    }

    [Theory]
    [InlineData(0, 4, 9)]   // count too low 
    [InlineData(101, 4, 9)] // count too high
    [InlineData(5, 0, 9)]   // signalLength too low
    [InlineData(5, 21, 9)]  // signalLength too high
    [InlineData(5, 4, 0)]   // maxStrength too low
    [InlineData(5, 4, 101)] // maxStrength too high
    public async Task Generate_InvalidParams_ReturnsBadRequest(int count, int signalLength, int maxStrength)
    {
        var response = await _client.GetAsync(
            $"/api/devices/generate?count={count}&signalLength={signalLength}&maxStrength={maxStrength}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Simulate Endpoint ──

    [Fact]
    public async Task Simulate_ValidRequest_ReturnsResponse()
    {
        var request = new SimulateRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [2, 4, 1, 3],
                ["D02"] = [7, 1, 5, 2],
                ["D03"] = [3, 6, 2, 8]
            }
        };

        var response = await _client.PostAsJsonAsync("/api/signal/simulate", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SimulateResponse>(JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(4, result.SignalLength);
        Assert.Equal(3, result.TotalDevices);
        Assert.InRange(result.ActiveDeviceCount, 1, 3);
    }

    [Fact]
    public async Task Simulate_EmptyDevices_ReturnsBadRequest()
    {
        var request = new SimulateRequest
        {
            Devices = new Dictionary<string, int[]>()
        };

        var response = await _client.PostAsJsonAsync("/api/signal/simulate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Decode Endpoint ──

    [Fact]
    public async Task Decode_FindsKnownSolution()
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

        var response = await _client.PostAsJsonAsync("/api/signal/decode", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DecodeResponse>(JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.SolutionCount >= 1);
        Assert.Contains(result.Solutions, s =>
            s.TransmittingDevices.OrderBy(d => d).SequenceEqual(new[] { "D01", "D03", "D05" }));
    }

    [Fact]
    public async Task Decode_InvalidTolerance_ReturnsBadRequest()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [1, 2]
            },
            ReceivedSignal = [1, 2],
            Tolerance = -1
        };

        var response = await _client.PostAsJsonAsync("/api/signal/decode", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Decode_MismatchedSignalLength_ReturnsBadRequest()
    {
        var request = new DecodeRequest
        {
            Devices = new Dictionary<string, int[]>
            {
                ["D01"] = [1, 2, 3]
            },
            ReceivedSignal = [1, 2],
            Tolerance = 0
        };

        var response = await _client.PostAsJsonAsync("/api/signal/decode", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── Full Workflow ──

    [Fact]
    public async Task FullWorkflow_GenerateSimulateDecode()
    {
        // Step 1: Generate
        var genResponse = await _client.GetAsync("/api/devices/generate?count=5&signalLength=4&maxStrength=9");
        genResponse.EnsureSuccessStatusCode();
        var devices = await genResponse.Content.ReadFromJsonAsync<Dictionary<string, int[]>>(JsonOptions);

        // Step 2: Simulate
        var simRequest = new SimulateRequest { Devices = devices! };
        var simResponse = await _client.PostAsJsonAsync("/api/signal/simulate", simRequest);
        simResponse.EnsureSuccessStatusCode();
        var simResult = await simResponse.Content.ReadFromJsonAsync<SimulateResponse>(JsonOptions);

        // Step 3: Decode
        var decRequest = new DecodeRequest
        {
            Devices = devices!,
            ReceivedSignal = simResult!.ReceivedSignal,
            Tolerance = 0
        };
        var decResponse = await _client.PostAsJsonAsync("/api/signal/decode", decRequest);
        decResponse.EnsureSuccessStatusCode();
        var decResult = await decResponse.Content.ReadFromJsonAsync<DecodeResponse>(JsonOptions);

        Assert.NotNull(decResult);
        Assert.True(decResult.SolutionCount >= 1);
        // Every solution should match
        Assert.All(decResult.Solutions, s => Assert.True(s.MatchesReceived));
    }
}