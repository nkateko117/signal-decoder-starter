using SignalDecoder.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Interfaces
{
    public interface ISignalSimulatorService
    {
        SimulateResponse Simulate(Dictionary<string, int[]> devices);
    }
}
