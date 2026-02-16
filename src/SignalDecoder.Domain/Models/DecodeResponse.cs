namespace SignalDecoder.Domain.Models
{
    public class DecodeResponse
    {
        public List<DecodeResult> Solutions { get; set; } = new();           // All valid solutions
        public int SolutionCount { get; set; }                      // Number of solutions
        public long SolveTimeMs { get; set; }                       // Time taken to solve
    }
}
