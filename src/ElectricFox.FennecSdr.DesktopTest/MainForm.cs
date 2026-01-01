using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.Touch;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.App.Screens;
using ElectricFox.EmbeddedApplicationFramework.Touch;

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
            // Resources
            var resources = new ResourceManager();
            await resources.LoadAsync();

            // Screens
            var splashScreen = new SplashScreen(resources);
            splashScreen.Initialize();
            var mainMenuScreen = new MainMenuScreen(resources);
            mainMenuScreen.Initialize();
            var pmrSelectionScreen = new PmrChannelSelectScreen(resources);
            pmrSelectionScreen.Initialize();
            var ctcssScreen = new CtcssScreen(resources);
            ctcssScreen.Initialize();

            this.pictureBox1.Image = this.bitmapTarget.Bitmap;

            screenManager.NavigateTo(splashScreen);
            await Task.Delay(2000);

            var menuSelection = await screenManager.ShowAsync(mainMenuScreen);

            var channel = await screenManager.ShowAsync(pmrSelectionScreen);
            var freq = Constants.PmrChannelFrequencies[channel.Value];

            screenManager.NavigateTo(ctcssScreen);
        }

        private void PictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            TouchEventReceived?.Invoke(new TouchEvent(new TouchPoint(e.X, e.Y)));
        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            this.Text = $"Mouse at {e.X}, {e.Y}";
            //TouchEventReceived?.Invoke(new TouchEvent (TouchEventType.Move, new TouchPoint(e.X, e.Y)));
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            //TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Up, new TouchPoint(e.X, e.Y)));
        }
    }
}
