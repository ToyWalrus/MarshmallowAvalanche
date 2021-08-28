using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class GameRoot : Core {
        public const int DesiredWindowHeight = 850;
        public const int DesiredWindowWidth = 550;

        protected override void Initialize() {
            base.Initialize();

            Window.Title = "Marshmallow Avalanche";
            Window.AllowUserResizing = true;

            Screen.SetSize(DesiredWindowWidth, DesiredWindowHeight);

            Scene = new MainScene(3);
        }
    }
}
