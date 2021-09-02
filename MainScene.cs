using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class MainScene : Scene {
        private BlockSpawner blockSpawner;
        private Character marshmallow;
        private RisingZone risingZone;
        private readonly float blockSpawnInterval = 1f;
        private readonly float sceneWidth = GameRoot.DesiredWindowWidth * 2;
        private float risingZoneDelay = 5f;
        private float blockSpawnTimer;

        public MainScene() : base() { blockSpawnTimer = blockSpawnInterval; }
        public MainScene(float sceneWidth, float blockSpawnInterval = 1) : base() {
            this.blockSpawnInterval = blockSpawnInterval;
            this.sceneWidth = sceneWidth;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            AddRenderer(new DefaultRenderer());
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAll);
            ClearColor = Color.CornflowerBlue;

            Vector2 marshmallowSize = new Vector2(30, 60);
            marshmallow = CreateEntity("marshmallow", new Vector2(0, GameRoot.DesiredWindowHeight - marshmallowSize.Y * 1.5f))
                .AddComponent(new Character(marshmallowSize));
            marshmallow.SetGravityModifier(4);
            marshmallow.JumpSpeed = 700;

            float maxBlockSize = 180;
            float minBlockSize = 80;
            Vector2 spawnerSize = new Vector2(sceneWidth - maxBlockSize * 2, 40);
            blockSpawner = CreateEntity("block-spawner", new Vector2(-spawnerSize.X / 2, 0))
                .AddComponent(new BlockSpawner(spawnerSize, minBlockSize, maxBlockSize));
            MoveWithCamera spawnerMover = blockSpawner.AddComponent<MoveWithCamera>();
            spawnerMover.SetFollowOnXAxis(false);

            risingZone = CreateEntity("rising-zone").AddComponent<RisingZone>();
            risingZone.SetRiseRate(10);

            Camera.Entity.AddComponent(new FollowCamera(marshmallow.Entity));
            CameraBounds camBounds = Camera.Entity.AddComponent(new CameraBounds(GameRoot.DesiredWindowHeight, -sceneWidth / 2, sceneWidth / 2));

            SetUpWorldBounds(camBounds);
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();

            if (!risingZone.IsRising) {
                risingZoneDelay -= Time.DeltaTime;
                if (risingZoneDelay < 0) {
                    risingZone.SetCharacter(marshmallow);
                    risingZone.Entity.Position = new Vector2(0, GameRoot.DesiredWindowHeight);
                    risingZone.Collider.SetWidth(sceneWidth * 2);
                    risingZone.BeginRising();
                }
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
            if (blockSpawnTimer < 0) {
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
