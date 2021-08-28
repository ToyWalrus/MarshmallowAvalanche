using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace MarshmallowAvalanche {
    public class GameRoot : Core {
        public const int DesiredWindowHeight = 850;
        public const int DesiredWindowWidth = 550;

        protected override void Initialize() {
            base.Initialize();

            Window.Title = "Marshmallow Avalanche";
            Window.AllowUserResizing = true;

            Screen.SetSize(DesiredWindowWidth, DesiredWindowHeight);
            
            Scene = new BasicScene();
        }


        private Color GetRandomColor() {
            System.Random rand = new System.Random();
            int r = rand.Next(50, 235);
            int g = rand.Next(50, 235);
            int b = rand.Next(50, 235);
            return new Color(r, g, b);
        }
    }

    class BasicScene : Scene {

        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.Aqua;
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAllPixelPerfect);


            var tex = Content.LoadTexture("FallingBlock");
            var playerentity = CreateEntity("player", new Vector2(GameRoot.DesiredWindowWidth / 2, GameRoot.DesiredWindowHeight / 2));

            playerentity.AddComponent(new SpriteRenderer(tex));
        }
    }
}
