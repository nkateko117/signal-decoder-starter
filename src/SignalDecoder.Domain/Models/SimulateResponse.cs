namespace SignalDecoder.Domain.Models
{
    public class SimulateResponse
    {
        public int[] ReceivedSignal { get; set; } = Array.Empty<int>();                  // Combined signal
        public int ActiveDeviceCount { get; set; }                  // How many transmitted
        public int SignalLength { get; set; }                       // Pattern length
        public int TotalDevices { get; set; }                       // Total devices available
    }
}
