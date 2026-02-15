using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Interfaces
{
    public interface IDeviceGeneratorService
    {
        Dictionary<string, int[]> GenerateDevices(int count, int signalLength, int maxStrength);
    }
}
