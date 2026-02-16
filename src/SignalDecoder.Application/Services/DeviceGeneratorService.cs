using SignalDecoder.Domain.Interfaces;

namespace SignalDecoder.Application.Services;

public class DeviceGeneratorService : IDeviceGeneratorService
{
    private readonly Random _random = new();

    public Dictionary<string, int[]> GenerateDevices(int count, int signalLength, int maxStrength)
    {
        var devices = new Dictionary<string, int[]>();

        for (int i = 1; i <= count; i++)
        {
            // Format device ID as D01, D02, ..., D99
            string deviceId = $"D{i:D2}";

            int[] signal = new int[signalLength];
            for (int j = 0; j < signalLength; j++)
            {
                signal[j] = _random.Next(0, maxStrength + 1);
            }

            devices[deviceId] = signal;
        }

        return devices;
    }
}