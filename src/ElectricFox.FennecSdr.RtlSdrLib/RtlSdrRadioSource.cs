using System.Diagnostics;

namespace ElectricFox.FennecSdr.RtlSdrLib;

public sealed class RtlSdrRadioSource : IRadioSource
{
    private const string RtlFmPath = "rtl_fm";

    public uint Frequency { get; set; } = 145_500_000;
    public int Gain { get; set; } = 40;
    public uint SampleRate { get; set; } = 12_000;

    public event Action<short[]>? SamplesAvailable;

    private Process? _process;
    private Task? _readerTask;
    private CancellationTokenSource? _cts;

    // Tunable chunk size (important!)
    private const int SamplesPerChunk = 512;

    public Task StartAsync(CancellationToken token)
    {
        if (_process != null)
        {
            throw new InvalidOperationException("Already started");
        }

        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var psi = new ProcessStartInfo
        {
            FileName = RtlFmPath,
            Arguments = $"-f {Frequency} -M fm -s {SampleRate} -g {Gain}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        _process.Start();

        _readerTask = Task.Run(
            () => ReadLoopAsync(_process, _cts.Token),
            _cts.Token);

        return Task.CompletedTask;
    }

    private async Task ReadLoopAsync(Process process, CancellationToken ct)
    {
        var stream = process.StandardOutput.BaseStream;

        byte[] byteBuffer = new byte[SamplesPerChunk * sizeof(short)];
        short[] sampleBuffer = new short[SamplesPerChunk];

        try
        {
            while (!ct.IsCancellationRequested)
            {
                int read = await stream.ReadAsync(byteBuffer, ct);
                if (read == 0)
                    break;

                int samplesRead = read / 2;
                if (samplesRead == 0)
                    continue;

                Buffer.BlockCopy(byteBuffer, 0, sampleBuffer, 0, read);

                // Fire event
                SamplesAvailable?.Invoke(sampleBuffer[..samplesRead]);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"RTL-SDR read error: {ex}");
        }
    }

    public async Task StopAsync()
    {
        if (_process == null)
            return;

        _cts?.CancelAsync();

        try
        {
            if (_readerTask != null)
                await _readerTask;
        }
        catch { /* swallow */ }

        if (!_process.HasExited)
            _process.Kill();

        _process.Dispose();
        _process = null;
    }

    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        _cts?.Dispose();
    }
}