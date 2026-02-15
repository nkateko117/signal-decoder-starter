using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Models
{
    public class DecodeRequest
    {
        public Dictionary<string, int[]> Devices { get; set; }      // All available devices
        public int[] ReceivedSignal { get; set; }                   // Signal to decode
        public int Tolerance { get; set; } = 0;                     // Fuzzy match tolerance
    }
}
