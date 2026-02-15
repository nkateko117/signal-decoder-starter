using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Models
{
    public class SimulateRequest
    {
        public Dictionary<string, int[]> Devices { get; set; }      // Devices to simulate
    }
}
