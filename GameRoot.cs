using System;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
            var renderer = playerentity.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = Color.HotPink;
            renderer.SetHeight(60);
            renderer.SetWidth(30);

            var character = playerentity.AddComponent(new Character(playerentity.Position, new Vector2(30, 60)));
            character.SetGravityModifier(4);
            character.JumpSpeed = 700;

            SetUpWorldBounds();
        }

        private void SetUpWorldBounds() {
            CreateWall(new Rectangle(-10, -10, 20, GameRoot.DesiredWindowHeight + 20), "left");
            CreateWall(new Rectangle(GameRoot.DesiredWindowWidth - 10, -10, 20, GameRoot.DesiredWindowHeight + 20), "right");
            CreateWall(new Rectangle(-10, GameRoot.DesiredWindowHeight - 10, GameRoot.DesiredWindowWidth + 20, 20), "bot");
        }

        private void CreateWall(Rectangle rect, string name) {
            var wall = CreateEntity(name, rect.Center.ToVector2());
            wall.AddComponent(new StaticObject(rect.Size.ToVector2()));

            var renderer = wall.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = Color.Blue;
            renderer.SetHeight(rect.Height);
            renderer.SetWidth(rect.Width);
        }
    }
}
