using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.App.Screens;

namespace ElectricFox.FennecSdr.DesktopTest
{
    public partial class MainForm : Form, ITouchController
    {
        private GraphicsRenderer gfx;

        private readonly BitmapScanlineTarget bitmapTarget;

        private readonly AppHost app;

        private readonly ScreenManager screenManager;

        public MainForm()
        {
            InitializeComponent();

            bitmapTarget = new BitmapScanlineTarget(320, 240);
            bitmapTarget.FrameCompleted += () =>
            {
                this.pictureBox1.Invalidate();
            };
            gfx = new GraphicsRenderer(bitmapTarget);
            app = new AppHost(gfx, this, new SixLabors.ImageSharp.Size(320, 240));
            screenManager = new ScreenManager(app);
        }

        public event Action<TouchEvent> TouchEventReceived;

        public void Start()
        {
            
        }

        private async void MainFormLoad(object sender, EventArgs e)
        {
            this.pictureBox1.Image = this.bitmapTarget.Bitmap;

            await app.Start();
            screenManager.NavigateTo(new SplashScreen(screenManager));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Down, new TouchPoint(e.X, e.Y)));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.Text = $"Mouse at {e.X}, {e.Y}";
        }
    }
}
