namespace ElectricFox.FennecSdr.App;

public sealed class CtcssToneFinder
{
    public event Action<double>? ToneDetected;
    public event Action? ToneLost;
    public event Action<(double frequency, double power)[]>? HistogramAvailable;
    
    private const int SampleRate = Constants.RtlSdrSampleRate;

    // ~200 ms buffer (good compromise for low tones)
    private const int WindowDurationMs = 200;

    // Require stability across multiple frames
    private const int RequiredStableFrames = 4;

    // Neighbour dominance (20% stronger than adjacent tones)
    private const double NeighbourDominance = 1.2;

    // Minimum absolute energy gate
    private const double MinEnergy = 1e6;
    
    private readonly List<short> _sampleBuffer = [];
    private readonly int _windowSize;

    private readonly Dictionary<double, int> _hitCounts = new();

    private double? _currentTone;
    
    public CtcssToneFinder()
    {
        _windowSize = SampleRate * WindowDurationMs / 1000;
    }

    public void AddSamples(short[] samples)
    {
        _sampleBuffer.AddRange(samples);

        while (_sampleBuffer.Count >= _windowSize)
        {
            var window = _sampleBuffer
                .Take(_windowSize)
                .ToArray();

            _sampleBuffer.RemoveRange(0, _windowSize / 2); // 50% overlap

            ProcessWindow(window);
        }
    }
    
    private void ProcessWindow(short[] samples)
    {
        var totalEnergy = samples.Sum(s => (double)s * s);
        if (totalEnergy < MinEnergy)
        {
            ClearState();
            return;
        }

        // Apply Hann window
        var windowed = ApplyHann(samples);

        // Measure all tones
        var powers = Constants.CtcssTones
            .Select(f => (Freq: f, Power: Goertzel(windowed, f)))
            .ToArray();
        
        HistogramAvailable?.Invoke(powers);

        var bestIndex = Array.IndexOf(
            powers,
            powers.MaxBy(p => p.Power)
        );

        if (!IsLocalPeak(powers, bestIndex))
        {
            RegisterMiss();
            return;
        }

        var bestFreq = powers[bestIndex].Freq;
        RegisterHit(bestFreq);
    }
    
    private static bool IsLocalPeak(
        (double Freq, double Power)[] powers,
        int index)
    {
        var power = powers[index].Power;

        if (index > 0 && power < powers[index - 1].Power * NeighbourDominance)
        {
            return false;
        }

        return index >= powers.Length - 1 ||
               !(power < powers[index + 1].Power * NeighbourDominance);
    }
    
    private void RegisterHit(double freq)
    {
        if (!_hitCounts.TryAdd(freq, 1))
            _hitCounts[freq]++;

        // Decay others
        foreach (var key in _hitCounts.Keys.ToList().Where(key => key != freq))
        {
            _hitCounts[key] = Math.Max(0, _hitCounts[key] - 1);
        }

        if (_hitCounts[freq] < RequiredStableFrames)
        {
            return;
        }
        
        if (_currentTone == freq)
        {
            return;
        }
        
        _currentTone = freq;
        ToneDetected?.Invoke(freq);
    }
    
    private void RegisterMiss()
    {
        foreach (var key in _hitCounts.Keys.ToList())
        {
            _hitCounts[key] = Math.Max(0, _hitCounts[key] - 1);
        }

        if (_currentTone == null)
        {
            return;
        }
        
        if (_hitCounts.TryGetValue(_currentTone.Value, out var count) && count != 0)
        {
            return;
        }
        
        _currentTone = null;
        ToneLost?.Invoke();
    }


    private void ClearState()
    {
        _hitCounts.Clear();

        if (_currentTone == null)
        {
            return;
        }
        
        _currentTone = null;
        ToneLost?.Invoke();
    }
    
    private static double[] ApplyHann(short[] samples)
    {
        var n = samples.Length;
        var result = new double[n];

        for (var i = 0; i < n; i++)
        {
            var w = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (n - 1)));
            result[i] = samples[i] * w;
        }

        return result;
    }
    
    private static double Goertzel(
        double[] samples,
        double frequency)
    {
        var n = samples.Length;
        var k = (int)(0.5 + (n * frequency) / SampleRate);

        var omega = 2.0 * Math.PI * k / n;
        var coeff = 2.0 * Math.Cos(omega);

        double s1 = 0, s2 = 0;

        foreach (var sample in samples)
        {
            var s0 = sample + coeff * s1 - s2;
            s2 = s1;
            s1 = s0;
        }

        return s1 * s1 + s2 * s2 - coeff * s1 * s2;
    }
}