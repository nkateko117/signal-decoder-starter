using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDecoder.Domain.Models
{
    public class DecodeResult
    {
        public List<string> TransmittingDevices { get; set; }       // Device IDs in solution
        public Dictionary<string, int[]> DecodedSignals { get; set; } // Device patterns
        public int[] ComputedSum { get; set; }                      // Sum of device patterns
        public bool MatchesReceived { get; set; }                   // Within tolerance?
    }
}
