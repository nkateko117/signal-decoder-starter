using SignalDecoder.Domain.Interfaces;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Application.Services;

public class SignalSimulatorService : ISignalSimulatorService
{
    private readonly Random _random = new();

    public SimulateResponse Simulate(Dictionary<string, int[]> devices)
    {
        var deviceList = devices.ToList();
        int totalDevices = deviceList.Count;
        int signalLength = deviceList[0].Value.Length;
        
        int activeCount = _random.Next(1, totalDevices + 1); // Randomly select how many devices are active (1 to N)

        // Shuffle and pick the first 'activeCount' devices
        var shuffled = deviceList.OrderBy(_ => _random.Next()).ToList();
        var activeDevices = shuffled.Take(activeCount).ToList();

        // element-wise sum of active device signalsw
        int[] combinedSignal = new int[signalLength];
        foreach (var device in activeDevices)
        {
            for (int i = 0; i < signalLength; i++)
            {
                combinedSignal[i] += device.Value[i];
            }
        }

        return new SimulateResponse
        {
            ReceivedSignal = combinedSignal,
            ActiveDeviceCount = activeCount,
            SignalLength = signalLength,
            TotalDevices = totalDevices
        };
    }
}