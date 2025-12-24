using SixLabors.ImageSharp;
using System.Diagnostics;

namespace ElectrcFox.FennecSdr;

public class Program
{
    public static void Main(string[] args)
    {
        const string RTL_FM_PATH = "rtl_fm";

        Console.WriteLine("Fennec SDR Ready");

        if (args.Length != 1)
        {
            Console.WriteLine(
                "Incorrect number of arguments - specify either channel (P1 - P16) or frequency in MHz"
            );
            return;
        }

        double frequency;

        if (args[0].StartsWith('P'))
        {
            var channel = int.Parse(args[0].Substring(1));
            if (channel < 1 || channel > 16)
            {
                Console.WriteLine("Invalid channel number - must be between 1 and 16");
                return;
            }

            frequency = Constatnts.PmrChannelFrequencies[channel - 1];
        }
        else
        {
            if (!double.TryParse(args[0], out frequency))
            {
                Console.WriteLine("Invalid frequency - must be a number in MHz");
                return;
            }
        }

        string sfrequency = $"{frequency}M";

        ProcessStartInfo psi =
            new()
            {
                FileName = RTL_FM_PATH,
                Arguments = $"-f {sfrequency} -M fm -s {Constatnts.RtlSdrSampleRate} -g 40",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

        using Process process = new() { StartInfo = psi };
        process.Start();

        using Stream stream = process.StandardOutput.BaseStream;
        using BinaryReader reader = new(stream);

        byte[] buffer = new byte[Constatnts.RtlSdrSampleRate * 2]; // 1-second audio buffer

        var lcd = new Ili9341();
        lcd.Init();

        var gfx = new GraphicsRenderer(lcd);

        while (true)
        {
            int bytesRead = reader.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                continue;
            }

            short[] samples = new short[bytesRead / 2];
            Buffer.BlockCopy(buffer, 0, samples, 0, bytesRead);

            var tone = Ctcss.DetectCTCSS(samples);

            gfx.Clear(Color.Black);
            gfx.DrawText("Fennec SDR", 20, 20, Color.White);
            gfx.DrawRect(10, 60, 300, 160, Color.Green);
            gfx.DrawText("CTCSS Tone:", 20, 80, Color.Yellow);
            gfx.DrawText(tone.HasValue ? tone.Value.ToString("0.###") : "(None)", 20, 120, Color.Cyan);

            gfx.Flush();
        }
    }
}