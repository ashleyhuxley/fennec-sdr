namespace ElectricFox.FennecSdr.App;

public class Ctcss
{
    public static readonly double[] CtcssTones =
    [
        67.0,
        71.9,
        77.0,
        82.5,
        88.5,
        94.8,
        100.0,
        107.2,
        114.8,
        123.0,
        131.8,
        141.3,
        151.4,
        162.2,
        173.8,
        186.2,
        203.5,
        218.1,
        233.6,
        250.3
    ];

    public static void DisplayHistogram(Dictionary<double, double> ctcssTones)
    {
        Console.Clear();

        const int maxBarLength = 50; // Maximum length of the histogram bar

        double maxPower = ctcssTones.Max(t => t.Value);

        foreach (var tone in ctcssTones)
        {
            double frequency = tone.Key;
            double power = tone.Value;
            int barLength = (int)(power / maxPower * maxBarLength);

            Console.WriteLine($"{frequency, 8:F2} Hz: {new string('#', barLength)}");
        }
    }

    public static Dictionary<double, double> GetToneValues(short[] samples)
    {
        var result = new Dictionary<double, double>();

        foreach (var tone in CtcssTones)
        {
            double power = Goertzel(samples, tone, Constants.RtlSdrSampleRate);
            result.Add(tone, power);
        }

        return result;
    }

    public static double? DetectCTCSS(
        short[] samples,
        double minEnergy = 1e9,
        double dominanceRatio = 6.0,
        double minConfidence = 0.01
    )
    {
        double totalEnergy = 0;
        foreach (short s in samples)
            totalEnergy += (double)s * s;

        if (totalEnergy < minEnergy)
            return null;

        double maxPower = 0;
        double secondPower = 0;
        double? bestMatch = null;

        foreach (double freq in CtcssTones)
        {
            double power = Goertzel(samples, freq, Constants.RtlSdrSampleRate);

            if (power > maxPower)
            {
                secondPower = maxPower;
                maxPower = power;
                bestMatch = freq;
            }
            else if (power > secondPower)
            {
                secondPower = power;
            }
        }

        if (maxPower < secondPower * dominanceRatio)
            return null;

        double confidence = maxPower / totalEnergy;
        if (confidence < minConfidence)
            return null;

        return bestMatch;
    }

    private static double Goertzel(short[] samples, double frequency, int sampleRate)
    {
        int k = (int)(0.5 + ((samples.Length * frequency) / sampleRate));
        double omega = (2.0 * Math.PI * k) / samples.Length;
        double coeff = 2.0 * Math.Cos(omega);

        double s1 = 0,
            s2 = 0;
        foreach (short sample in samples)
        {
            double s0 = sample + coeff * s1 - s2;
            s2 = s1;
            s1 = s0;
        }

        return s1 * s1 + s2 * s2 - coeff * s1 * s2;
    }
}
