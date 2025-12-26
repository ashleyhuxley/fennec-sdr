using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.BdfSharp;

using Color = SixLabors.ImageSharp.Color;

namespace ElectricFox.FennecSdr.DesktopTest
{
    public partial class MainForm : Form, ITouchController
    {
        private GraphicsRenderer gfx;

        private readonly BitmapScanlineTarget bitmapTarget;

        private BdfFont tamzenBold;

        public MainForm()
        {
            InitializeComponent();

            bitmapTarget = new BitmapScanlineTarget(320, 240);
            gfx = new GraphicsRenderer(bitmapTarget);
        }

        public event Action<TouchEvent> TouchEventReceived;

        public void Start()
        {
            
        }

        private async void MainFormLoad(object sender, EventArgs e)
        {
            tamzenBold = await BdfFont.LoadAsync("D:\\Temp\\BdfFonts\\Tamzen8x15b.bdf");

            gfx.DrawRect(10, 10, 30, 20, Color.Red);

            gfx.FillEllipse(50, 50, 30, 20, Color.Blue);

            gfx.DrawText("Button Text", tamzenBold, 10, 100, Color.Cyan);

            gfx.Flush();

            this.pictureBox1.Image = this.bitmapTarget.Bitmap;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Down, new TouchPoint(e.X, e.Y)));
        }
    }
}
