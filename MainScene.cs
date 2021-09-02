using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

/* 
 * TODO:
 * Make blocks random colors from set list
 * Create marshmallow animated sprite
 * Record height
 * Make liquid rise faster as you get higher
 * Add interesting background
 * Add sound (maybe music too?)
 */

namespace MarshmallowAvalanche {
    public class MainScene : Scene {
        public readonly float SceneWidth = GameRoot.DesiredWindowWidth * 2;

        private BlockSpawner blockSpawner;
        private Character marshmallow;
        private RisingZone risingZone;
        private GameUIOverlay gameOverlay;
        private readonly float blockSpawnInterval = 1f;
        private float risingZoneDelay = 5f;
        private float blockSpawnTimer;
        private bool isSpawningBlocks = true;
        private bool isGameOver = false;

        public MainScene() : base() {
            blockSpawnTimer = blockSpawnInterval;
        }

        public MainScene(float sceneWidth = GameRoot.DesiredWindowWidth * 2, float blockSpawnInterval = 1) : base() {
            this.blockSpawnInterval = blockSpawnInterval;
            SceneWidth = sceneWidth;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            Vector2 marshmallowSize = new Vector2(30, 60);
            marshmallow = CreateEntity("marshmallow", new Vector2(0, GameRoot.DesiredWindowHeight - marshmallowSize.Y * 1.5f))
                .AddComponent(new Character(marshmallowSize));
            marshmallow.SetGravityModifier(4);
            marshmallow.JumpSpeed = 700;

            float maxBlockSize = 180;
            float minBlockSize = 80;
            Vector2 spawnerSize = new Vector2(SceneWidth - maxBlockSize * 2, 40);
            blockSpawner = CreateEntity("block-spawner", new Vector2(-spawnerSize.X / 2, 0))
                .AddComponent(new BlockSpawner(spawnerSize, minBlockSize, maxBlockSize));
            MoveWithCamera spawnerMover = blockSpawner.AddComponent<MoveWithCamera>();
            spawnerMover.SetFollowOnXAxis(false);

            risingZone = CreateEntity("rising-zone").AddComponent<RisingZone>();
            risingZone.SetRiseRate(10);

            Camera.Entity.AddComponent(new FollowCamera(marshmallow.Entity));
            CameraBounds camBounds = Camera.Entity.AddComponent(new CameraBounds(GameRoot.DesiredWindowHeight, -SceneWidth / 2, SceneWidth / 2));
            SetUpWorldBounds(camBounds);            

            gameOverlay = CreateEntity("game-overlay").AddComponent<GameUIOverlay>();

            AddRenderer(new RenderLayerExcludeRenderer(0, new int[] { GameUIOverlay.RenderLayer }));
            AddRenderer(new ScreenSpaceRenderer(1, new int[] { GameUIOverlay.RenderLayer }));
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAll);
            ClearColor = Color.CornflowerBlue;
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();

            if (!risingZone.IsRising) {
                risingZoneDelay -= Time.DeltaTime;
                if (risingZoneDelay < 0) {
                    risingZone.SetCharacter(marshmallow);
                    risingZone.Entity.Position = new Vector2(0, GameRoot.DesiredWindowHeight);
                    risingZone.Collider.SetWidth(SceneWidth * 2);
                    risingZone.BeginRising();
                }
            }

            if (marshmallow.IsDead && !isGameOver) {
                Camera.Entity.RemoveComponent<FollowCamera>();
                gameOverlay.OnGameOver();
                isSpawningBlocks = false;
                isGameOver = true;
            }
        }


        private void SetUpWorldBounds(CameraBounds camBounds) {
            int boundThickness = 10;
            CreateWall(new RectangleF(
                camBounds.MinX,
                camBounds.MinY,
                camBounds.MaxX - camBounds.MinX,
                boundThickness
                ),
            "bot-wall"
            );

            var leftWall = CreateWall(new RectangleF(
                camBounds.MinX - boundThickness,
                -camBounds.MinY / 2,
                boundThickness,
                camBounds.MinY * 1.5f
                ),
            "left-wall"
            );
            leftWall.AddComponent<MoveWithCamera>().SetFollowOnXAxis(false);

            var rightWall = CreateWall(new RectangleF(
                camBounds.MaxX,
                -camBounds.MinY / 2,
                boundThickness,
                camBounds.MinY * 1.5f
                ),
            "right-wall"
            );
            rightWall.AddComponent<MoveWithCamera>().SetFollowOnXAxis(false);
        }

        private Entity CreateWall(RectangleF rect, string name) {
            var wall = CreateEntity(name, rect.Center);
            wall.AddComponent(new StaticObject(rect.Size));

            //var renderer = wall.AddComponent<PrototypeSpriteRenderer>();
            //renderer.SetWidth(rect.Width);
            //renderer.SetHeight(rect.Height);

            return wall;
        }

        private void BlockSpawnerTick() {
            blockSpawnTimer -= Time.DeltaTime;
            if (isSpawningBlocks && blockSpawnTimer < 0) {
                blockSpawnTimer = blockSpawnInterval;
                FallingBlock block = blockSpawner.SpawnBlock(250);
                block?.SetBlockColor(GetRandomColor());
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
