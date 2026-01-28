using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricFox.FennecSdr.RtlSdrLib;

namespace ElectricFox.FennecSdr.DesktopTestAvalonia
{
    public class FakeRadioSource : IRadioSource
    {
        public uint Frequency { get; set; } = 145_500_000;
        public int Gain { get; set; } = 40;
        public uint SampleRate { get; set; } = 12_000;

        public event Action<short[]>? SamplesAvailable;

        private Timer? _sampleTimer;

        public FakeRadioSource()
        {
            _sampleTimer = new Timer(Timer_Callback);
            _sampleTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Timer_Callback(object? state)
        {
            // Generate fake samples
            short[] samples = new short[1024];
            var rand = new Random();
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (short)rand.Next(short.MinValue, short.MaxValue);
            }

            SamplesAvailable?.Invoke(samples);
        }

        public Task StartAsync(CancellationToken token)
        {
            // Start timer to generate samples every 100ms
            _sampleTimer?.Change(0, 100);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            // Stop timer
            _sampleTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}
