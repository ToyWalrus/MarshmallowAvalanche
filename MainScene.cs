using System;
using System.Collections.Generic;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class MainScene : Scene {
        private BlockSpawner blockSpawner;
        private Character marshmallow;
        private float blockSpawnInterval = 1f;
        private float blockSpawnTimer;

        public MainScene() : base() { blockSpawnTimer = blockSpawnInterval; }
        public MainScene(float blockSpawnInterval) : base() {
            this.blockSpawnInterval = blockSpawnInterval;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.CornflowerBlue;
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAllPixelPerfect);

            marshmallow = CreateEntity("marshmallow", new Vector2(GameRoot.DesiredWindowWidth / 2, GameRoot.DesiredWindowHeight / 2))
                .AddComponent(new Character(new Vector2(30, 60)));
            marshmallow.SetGravityModifier(4);
            marshmallow.JumpSpeed = 700;

            var renderer = marshmallow.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = Color.White;
            renderer.SetHeight(60);
            renderer.SetWidth(30);

            blockSpawner = CreateEntity("block-spawner").AddComponent(new BlockSpawner(new RectangleF(0, 0, GameRoot.DesiredWindowWidth, 40), 80, 180));

            var spawnerRenderer = blockSpawner.AddComponent<PrototypeSpriteRenderer>();
            spawnerRenderer.Color = Color.Yellow;
            spawnerRenderer.SetHeight(blockSpawner.SpawnBounds.Height);
            spawnerRenderer.SetWidth(blockSpawner.SpawnBounds.Width);

            SetUpWorldBounds();
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();
        }


        private void SetUpWorldBounds() {
            int boundThickness = 10;
            //CreateWall(new Rectangle(-boundThickness, -boundThickness, boundThickness, GameRoot.DesiredWindowHeight + boundThickness * 2), "left");
            //CreateWall(new Rectangle(GameRoot.DesiredWindowWidth, -boundThickness, boundThickness, GameRoot.DesiredWindowHeight + boundThickness * 2), "right");
            CreateWall(new Rectangle(-GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, GameRoot.DesiredWindowWidth * 3, boundThickness), "bot");
        }

        private void CreateWall(Rectangle rect, string name) {
            var wall = CreateEntity(name, rect.Center.ToVector2());
            wall.AddComponent(new StaticObject(rect.Size.ToVector2()));
        }

        private void BlockSpawnerTick() {
            blockSpawnTimer -= Time.DeltaTime;
            if (blockSpawnTimer < 0) {
                blockSpawnTimer = blockSpawnInterval;
                FallingBlock block = blockSpawner.SpawnBlock(200);
                block.SetBlockColor(GetRandomColor());
            }
        }

        private Color GetRandomColor() {
            System.Random rand = new System.Random();
            int r = rand.Next(50, 235);
            int g = rand.Next(50, 235);
            int b = rand.Next(50, 235);
            return new Color(r, g, b);
        }
    }
}
