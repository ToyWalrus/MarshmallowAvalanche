using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class MainScene : Scene {
        private BlockSpawner blockSpawner;
        private Character marshmallow;
        private float blockSpawnInterval = 1f;
        private float blockSpawnTimer;
        private float sceneWidth = GameRoot.DesiredWindowWidth * 2;

        public MainScene() : base() { blockSpawnTimer = blockSpawnInterval; }
        public MainScene(float blockSpawnInterval) : base() {
            this.blockSpawnInterval = blockSpawnInterval;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.CornflowerBlue;
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAll);

            Vector2 marshmallowSize = new Vector2(30, 60);
            marshmallow = CreateEntity("marshmallow", new Vector2(0, GameRoot.DesiredWindowHeight - marshmallowSize.Y * 1.5f))
                .AddComponent(new Character(marshmallowSize));
            marshmallow.SetGravityModifier(4);
            marshmallow.JumpSpeed = 700;

            var renderer = marshmallow.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = Color.White;
            renderer.SetHeight(60);
            renderer.SetWidth(30);

            float maxBlockSize = 180;
            float minBlockSize = 80;
            blockSpawner = CreateEntity("block-spawner").AddComponent(new BlockSpawner(new Vector2(sceneWidth - maxBlockSize * 2, 40), minBlockSize, maxBlockSize));
            MoveWithCamera spawnerMover = blockSpawner.AddComponent<MoveWithCamera>();
            spawnerMover.SetFollowOnXAxis(false);

            Camera.Entity.AddComponent(new FollowCamera(marshmallow.Entity));
            CameraBounds camBounds = Camera.Entity.AddComponent(new CameraBounds(GameRoot.DesiredWindowHeight, -sceneWidth / 2, sceneWidth / 2));

            SetUpWorldBounds(camBounds);
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();
        }


        private void SetUpWorldBounds(CameraBounds camBounds) {
            int boundThickness = 10;
            CreateWall(new RectangleF(
                camBounds.MinX - camBounds.ExtraCamPadding.X,
                camBounds.MinY + camBounds.ExtraCamPadding.Y,
                camBounds.MaxX - camBounds.MinX + camBounds.ExtraCamPadding.X * 2,
                boundThickness
                ),
            "bot-wall"
            );

            var leftWall = CreateWall(new RectangleF(
                camBounds.MinX - camBounds.ExtraCamPadding.X - boundThickness,
                -camBounds.MinY / 2,
                boundThickness,
                camBounds.MinY * 1.5f
                ),
            "left-wall"
            );
            leftWall.AddComponent<MoveWithCamera>().SetFollowOnXAxis(false);

            var rightWall = CreateWall(new RectangleF(
                camBounds.MaxX + camBounds.ExtraCamPadding.X,
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

            var renderer = wall.AddComponent<PrototypeSpriteRenderer>();
            renderer.SetWidth(rect.Width);
            renderer.SetHeight(rect.Height);

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
