using System.Diagnostics;
using SignalDecoder.Domain.Interfaces;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Application.Services;

public class SignalDecoderService : ISignalDecoderService
{
    public DecodeResponse Decode(DecodeRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        var devices = request.Devices;
        int[] received = request.ReceivedSignal;
        int tolerance = request.Tolerance;
        int signalLength = received.Length;

        // convert to arrays for fast indexed access.
        var deviceKeys = devices.Keys.OrderBy(k => k).ToArray();
        var deviceSignals = deviceKeys.Select(k => devices[k]).ToArray();
        int deviceCount = deviceKeys.Length;

        // Precompute suffix sums: for each position j, suffixMax[i][j] is the sum of signals[i..n-1][j] — the maximum possible contribution from remaining devices
        int[][] suffixSums = new int[deviceCount + 1][];
        suffixSums[deviceCount] = new int[signalLength];
        for (int i = deviceCount - 1; i >= 0; i--)
        {
            suffixSums[i] = new int[signalLength];
            for (int j = 0; j < signalLength; j++)
            {
                suffixSums[i][j] = suffixSums[i + 1][j] + deviceSignals[i][j];
            }
        }

        var solutions = new List<DecodeResult>();
        int[] currentSum = new int[signalLength];

        // Backtracking with pruning
        Backtrack(0, currentSum, new List<int>());

        stopwatch.Stop();

        return new DecodeResponse
        {
            Solutions = solutions,
            SolutionCount = solutions.Count,
            SolveTimeMs = stopwatch.ElapsedMilliseconds
        };

        void Backtrack(int index, int[] currentSum, List<int> selected)
        {
            // Checking if current partial sum already exceeds target + tolerance at any position
            for (int j = 0; j < signalLength; j++)
            {
                if (currentSum[j] > received[j] + tolerance)
                    return; // Prune: already too high
            }

            // Check if even selecting ALL remaining devices can reach target - tolerance
            for (int j = 0; j < signalLength; j++)
            {
                if (currentSum[j] + suffixSums[index][j] < received[j] - tolerance)
                    return; // Prune: can't possibly reach target even with all remaining
            }

            // Leaf node — check if current sum is a valid solution
            if (index == deviceCount)
            {
                bool matches = true;
                for (int j = 0; j < signalLength; j++)
                {
                    if (Math.Abs(currentSum[j] - received[j]) > tolerance)
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    var result = new DecodeResult
                    {
                        TransmittingDevices = selected.Select(i => deviceKeys[i]).ToList(),
                        DecodedSignals = selected.ToDictionary(
                            i => deviceKeys[i],
                            i => deviceSignals[i]),
                        ComputedSum = (int[])currentSum.Clone(),
                        MatchesReceived = true
                    };
                    solutions.Add(result);
                }
                return;
            }

            // Branch 1: Skip device at 'index'
            Backtrack(index + 1, currentSum, selected);

            // Branch 2: Include device at 'index'
            for (int j = 0; j < signalLength; j++)
                currentSum[j] += deviceSignals[index][j];

            selected.Add(index);

            Backtrack(index + 1, currentSum, selected);

            // Undo (backtrack)
            selected.RemoveAt(selected.Count - 1);
            for (int j = 0; j < signalLength; j++)
                currentSum[j] -= deviceSignals[index][j];
        }
    }
}