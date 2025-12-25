namespace ElectrcFox.FennecSdr;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Fennec SDR Ready");

        var lcd = new Ili9341(0, 0);
        var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 25);
        
        lcd.Init();

        var gfx = new GraphicsRenderer(lcd);

        //var ctcssUi = new CtcssUi("");
        //ctcssUi.Run(gfx);

        while (true)
        {
            var pos = touch.GetTouch();
            if (pos.HasValue)
            {
                Console.WriteLine($"Touch at {pos.Value.x},{pos.Value.y}");
                // Map to your buttons, e.g., frequency selection
            }

            Thread.Sleep(10); // simple polling loop
        }
    }
}