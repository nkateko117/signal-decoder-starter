using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Models
{
    public class SimulateResponse
    {
        public int[] ReceivedSignal { get; set; }                   // Combined signal
        public int ActiveDeviceCount { get; set; }                  // How many transmitted
        public int SignalLength { get; set; }                       // Pattern length
        public int TotalDevices { get; set; }                       // Total devices available
    }
}
