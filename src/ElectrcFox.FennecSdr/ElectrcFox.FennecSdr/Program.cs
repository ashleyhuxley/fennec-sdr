using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectrcFox.FennecSdr;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Fennec SDR Ready");

        var touchCal = new TouchCalibration(474, 3352, 332, 3900, true, false, false);

        var lcd = new Ili9341(0, 0);
        var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 17, touchCal);
        
        lcd.Init();

        var gfx = new GraphicsRenderer(lcd);
        gfx.Clear(Color.Black);
        gfx.DrawText("Fennec SDR", 20, 20, Color.White);
        gfx.Flush();

        //var ctcssUi = new CtcssUi("");
        //ctcssUi.Run(gfx);

        while (true)
        {
            var pos = touch.GetTouch();
            if (pos.HasValue)
            {
                Console.WriteLine($"Touch at {pos.Value.x},{pos.Value.y}");
                // Map to your buttons, e.g., frequency selection

                gfx.FillEllipse(pos.Value.x - 5, pos.Value.y - 5, 10, 10, Color.Red);
                gfx.Flush();
            }

            Thread.Sleep(10); // simple polling loop
        }
    }
}