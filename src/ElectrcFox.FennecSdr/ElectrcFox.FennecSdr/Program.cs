using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectrcFox.FennecSdr;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Fennec SDR Ready");

        var touchCal = new TouchCalibration(369, 3538, 332, 3900, true, false, true);

        var spiLock = new object();

        var lcd = new Ili9341(0, 0, spiLock);
        var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 17, touchCal, spiLock);
        
        touch.Start();

        lcd.Init();

        var gfx = new GraphicsRenderer(lcd);
        gfx.Clear(Color.Black);
        gfx.DrawText("Fennec SDR", 20, 20, Color.White);
        gfx.Flush();


        touch.TouchEventReceived += (te) =>
        {
            Console.WriteLine($"Touch Event: {te.Type} at {te.Point.X},{te.Point.Y}");
            if (te.Type == TouchEventType.Down)
            {
                gfx.FillEllipse(te.Point.X - 5, te.Point.Y - 5, 10, 10, Color.Red);
                gfx.Flush();
            }

            if (te.Type == TouchEventType.Up)
            {
                gfx.FillEllipse(te.Point.X - 5, te.Point.Y - 5, 10, 10, Color.Blue);
                gfx.Flush();
            }

        };


        //var ctcssUi = new CtcssUi("");
        //ctcssUi.Run(gfx);

        while (true)
        {
            await Task.Delay(10);
        }
    }
}