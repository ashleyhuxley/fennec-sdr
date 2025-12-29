using ElectrcFox.EmbeddedApplicationFramework.Graphics;
using ElectrcFox.FennecSdr.App;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.EmbeddedApplicationFramework;
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

            var resources = new ResourceManager();
            await resources.LoadAsync();

            screenManager.NavigateTo(new SplashScreen(resources));
            await Task.Delay(2000);
            var channel = await screenManager.ShowAsync(new PmrChannelSelectScreen(resources));
            var freq = Constants.PmrChannelFrequencies[channel.Value];

            screenManager.NavigateTo(new CtcssScreen(freq, resources));
        }

        private void PictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Down, new TouchPoint(e.X, e.Y)));
        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            this.Text = $"Mouse at {e.X}, {e.Y}";
            TouchEventReceived?.Invoke(new TouchEvent (TouchEventType.Move, new TouchPoint(e.X, e.Y)));
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Up, new TouchPoint(e.X, e.Y)));
        }
    }
}
