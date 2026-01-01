namespace ElectricFox.FennecSdr.RtlSdrLib
{
    public interface IRadioSource
    {
        public event Action<short[]>? SamplesAvailable;

        public Task StartAsync(CancellationToken token);
        public Task StopAsync();
        public uint Frequency { get; set; }
        public int Gain { get; set; }
        public uint SampleRate { get; set; }
    }
}
