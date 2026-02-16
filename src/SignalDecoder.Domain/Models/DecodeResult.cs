namespace SignalDecoder.Domain.Models
{
    public class DecodeResult
    {
        public List<string> TransmittingDevices { get; set; } = new();       // Device IDs in solution
        public Dictionary<string, int[]> DecodedSignals { get; set; } = new(); // Device patterns
        public int[] ComputedSum { get; set; } = Array.Empty<int>();                   // Sum of device patterns
        public bool MatchesReceived { get; set; }                   // Within tolerance?
    }
}
