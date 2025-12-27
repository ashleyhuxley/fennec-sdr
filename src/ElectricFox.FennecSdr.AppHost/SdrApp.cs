using ElectrcFox.EmbeddedApplicationFramework.Graphics;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Display;
using ElectricFox.FennecSdr.App.Screens;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App
{
    public class SdrApp
    {
        private const int SplashScreenTime = 2000;

        private readonly ITouchController _touchController;

        private readonly GraphicsRenderer _renderer;

        private Screen? _current;

        public SdrApp(IScanlineTarget scanlineTarget, ITouchController touchController)
        {
            _renderer = new GraphicsRenderer(scanlineTarget);
            _touchController = touchController;

            _touchController.TouchEventReceived += OnTouch;
        }

        private void OnTouch(TouchEvent e)
        {
            _current?.OnTouch(e);
        }

        public async Task RunAsync(CancellationToken token)
        {
            var resources = new ResourceManager();
            await resources.LoadAsync(token);

            SetScreen(new SplashScreen(resources));

            await Task.Delay(SplashScreenTime, token);

            SetScreen(new PmrChannelSelectScreen());

            while (!token.IsCancellationRequested)
            {

            }
        }

        private void SetScreen(Screen screen)
        {
            _current?.OnExit();
            _current = screen;
            _current.OnEnter();

            Render();
        }

        public void Render()
        {
            _renderer.Clear(Color.Black);
            _current?.Render(_renderer);
            _renderer.Flush();
        }
    }
}
