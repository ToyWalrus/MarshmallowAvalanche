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

        public MainScene() : base() {
            blockSpawnTimer = blockSpawnInterval;
        }
        public MainScene(float blockSpawnInterval) : base() {
            this.blockSpawnInterval = blockSpawnInterval;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.Aqua;
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAllPixelPerfect);

            var playerEntity = CreateEntity("player", new Vector2(GameRoot.DesiredWindowWidth / 2, GameRoot.DesiredWindowHeight / 2));
            var renderer = playerEntity.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = Color.White;
            renderer.SetHeight(60);
            renderer.SetWidth(30);

            marshmallow = playerEntity.AddComponent(new Character(new Vector2(30, 60)));
            marshmallow.SetGravityModifier(4);
            marshmallow.JumpSpeed = 700;

            SetUpWorldBounds();

            blockSpawner = CreateEntity("block-spawner").AddComponent(new BlockSpawner(
                new RectangleF(GameRoot.DesiredWindowWidth / 2, GameRoot.DesiredWindowHeight / 2, GameRoot.DesiredWindowWidth, 40),
                new Vector2(80, 80),
                new Vector2(170, 170)
                ));
        }

        private void SetUpWorldBounds() {
            int boundThickness = 10;
            CreateWall(new Rectangle(-boundThickness, -boundThickness, boundThickness, GameRoot.DesiredWindowHeight + boundThickness * 2), "left");
            CreateWall(new Rectangle(GameRoot.DesiredWindowWidth, -boundThickness, boundThickness, GameRoot.DesiredWindowHeight + boundThickness * 2), "right");
            CreateWall(new Rectangle(-boundThickness, GameRoot.DesiredWindowHeight, GameRoot.DesiredWindowWidth + boundThickness * 2, boundThickness), "bot");
        }

        private void CreateWall(Rectangle rect, string name) {
            var wall = CreateEntity(name, rect.Center.ToVector2());
            wall.AddComponent(new StaticObject(rect.Size.ToVector2()));
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();

            Debug.DrawText(marshmallow.Transform.Position.ToString());
        }

        private void BlockSpawnerTick() {
            blockSpawnTimer -= Time.DeltaTime;
            if (blockSpawnTimer < 0) {
                blockSpawnTimer = blockSpawnInterval;
                FallingBlock block = blockSpawner.SpawnBlock(200, true);
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
