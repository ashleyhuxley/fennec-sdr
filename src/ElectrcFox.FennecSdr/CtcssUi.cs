using SixLabors.ImageSharp;
using System.Diagnostics;

namespace ElectrcFox.FennecSdr
{
    public class CtcssUi
    {
        //const string RTL_FM_PATH = "rtl_fm";

        //private readonly string _frequency;

        //public CtcssUi(string frequency)
        //{
        //    _frequency = frequency;
        //}

        //public void Run(GraphicsRenderer gfx)
        //{
        //    ProcessStartInfo psi =
        //        new()
        //        {
        //            FileName = RTL_FM_PATH,
        //            Arguments = $"-f {_frequency} -M fm -s {Constatnts.RtlSdrSampleRate} -g 40",
        //            RedirectStandardOutput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //    using Process process = new() { StartInfo = psi };
        //    process.Start();

        //    using Stream stream = process.StandardOutput.BaseStream;
        //    using BinaryReader reader = new(stream);

        //    byte[] buffer = new byte[Constatnts.RtlSdrSampleRate * 2]; // 1-second audio buffer

        //    while (true)
        //    {
        //        int bytesRead = reader.Read(buffer, 0, buffer.Length);
        //        if (bytesRead == 0)
        //        {
        //            continue;
        //        }

        //        short[] samples = new short[bytesRead / 2];
        //        Buffer.BlockCopy(buffer, 0, samples, 0, bytesRead);

        //        var tone = Ctcss.DetectCTCSS(samples);

        //        gfx.Clear(Color.Black);
        //        gfx.DrawText("Fennec SDR", 20, 20, Color.White);
        //        gfx.DrawRect(10, 60, 300, 160, Color.Green);
        //        gfx.DrawText("CTCSS Tone:", 20, 80, Color.Yellow);
        //        gfx.DrawText(tone.HasValue ? tone.Value.ToString("0.###") : "(None)", 20, 120, Color.Cyan);

        //        gfx.Flush();
        //    }
        //}
    }
}
