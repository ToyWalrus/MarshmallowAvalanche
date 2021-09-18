using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

/* 
 * TODO:
 * Make blocks random colors from set list
 * Make settings for adjusting rise rate & block spawn (maybe time scale?)
 * Adjust the rising rate more smartly
 * Add sound effects
 */

namespace MarshmallowAvalanche {
    public class MainScene : Scene {
        public readonly float SceneWidth = GameRoot.DesiredWindowWidth * 2;
        public const int BackgroundRenderLayer = 100;

        private BlockSpawner blockSpawner;
        private Character marshmallow;
        private RisingZone risingZone;
        private GameUIOverlay gameOverlay;

        // How often the rate of block spawns increases
        private readonly float blockSpawnIncreaseInterval = 5f;

        // The timer to count down the increase interval of block spawns
        private float blockSpawnIncreaseTimer;

        // The delay between the spawn of blocks
        private float blockSpawnInterval = 1f;

        private float blockFallSpeed = 200;

        // The timer to count down the spawn interval
        private float blockSpawnTimer;

        // The delay before the zone starts rising at the start of the game
        private readonly float risingZoneDelay = 7f;

        // How often the rate of the rising zone increases
        private readonly float risingZoneRateIncreaseInterval = 1.5f;

        // The timer to count down the delay for the rising zone
        private float risingZoneTimer;

        private bool isSpawningBlocks = true;
        private bool isGameOver = false;
        private bool haveSetStartingHeight = false;
        private Score score;
        private ParallaxEffect background;
        //private PrototypeSpriteRenderer scoreLine;

        public MainScene() : base() {
            blockSpawnIncreaseTimer = blockSpawnIncreaseInterval;
            blockSpawnTimer = blockSpawnInterval;
            risingZoneTimer = risingZoneDelay;
        }

        public MainScene(float sceneWidth = GameRoot.DesiredWindowWidth * 2, float blockSpawnInterval = 1) : this() {
            this.blockSpawnInterval = blockSpawnInterval;
            SceneWidth = sceneWidth;
            blockSpawnTimer = blockSpawnInterval;
        }

        public override void Initialize() {
            base.Initialize();

            AddRenderer(new RenderLayerExcludeRenderer(0, new int[] { GameUIOverlay.RenderLayer }));
            SetDesignResolution(GameRoot.DesiredWindowWidth, GameRoot.DesiredWindowHeight, SceneResolutionPolicy.ShowAll);
            ClearColor = Color.Black;

            score = Score.LoadData();

            Vector2 marshmallowSize = new Vector2(28, 60);
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

            gameOverlay = CreateEntity("game-overlay").AddComponent(new GameUIOverlay(score));

            background = CreateEntity("background").AddComponent(new ParallaxEffect(Content.LoadTexture("background/full-background"), new Vector2(20, 30)));
            background.RenderLayer = BackgroundRenderLayer;
            background.OriginNormalized = new Vector2(.5f, 1);
            background.Transform.Scale = new Vector2(1.65f, 1.65f);
            background.Transform.Position = new Vector2(0, GameRoot.DesiredWindowHeight);
            
            SpriteRenderer ground = CreateEntity("ground-layer").AddComponent(new SpriteRenderer(Content.LoadTexture("background/ground")));
            ground.RenderLayer = BackgroundRenderLayer - 1;
            ground.OriginNormalized = new Vector2(.5f, 1);
            ground.Transform.Scale = new Vector2(1.5f, 1);
            ground.Transform.Position = background.Transform.Position;

            // Music from Unity PlatformerMicrogame project
            var music = Content.LoadSoundEffect("Music");
            music.Play();

            //scoreLine = CreateEntity("score-line").AddComponent<PrototypeSpriteRenderer>();
            //scoreLine.Color = Color.Yellow;
            //scoreLine.SetWidth(60);
            //scoreLine.SetHeight(5);
        }

        public override void OnStart() {
            base.OnStart();
            AddRenderer(new ScreenSpaceRenderer(1, new int[] { GameUIOverlay.RenderLayer }));
        }

        public override void Update() {
            base.Update();
            BlockSpawnerTick();
            UpdateScore();
            RiseZone();
            CheckForGameOver();
        }

        private void BlockSpawnerTick() {
            blockSpawnTimer -= Time.DeltaTime;
            blockSpawnIncreaseTimer -= Time.DeltaTime;

            if (isSpawningBlocks && blockSpawnTimer < 0) {
                blockSpawnTimer = blockSpawnInterval;
                FallingBlock block = blockSpawner.SpawnBlock(blockFallSpeed);
                if (block != null) {
                    block.SetBlockColor(GetRandomColor());
                    risingZone.RegisterSpawnedBlock(block);
                }
            }

            if (isSpawningBlocks && blockSpawnIncreaseTimer < 0) {
                blockSpawnInterval -= .0075f;
                blockSpawnInterval = System.Math.Max(.15f, blockSpawnInterval);
                blockSpawnIncreaseTimer = blockSpawnIncreaseInterval;
            }
        }

        private void UpdateScore() {
            float marshmallowPosition = marshmallow.Bounds.Top;
            //float xPos = Camera.Bounds.Right - scoreLine.Width / 2;
            if (!haveSetStartingHeight) {
                score.SetStartingHeight(marshmallowPosition / 10f);
                //scoreLine.Transform.SetPosition(new Vector2(xPos, marshmallowPosition));
                haveSetStartingHeight = true;
            } else {
                if (score.UpdateScoreIfBetter(marshmallowPosition / 10f)) {
                    //    scoreLine.Transform.SetPosition(new Vector2(xPos, marshmallowPosition));
                } else {
                    //    scoreLine.Transform.SetPosition(new Vector2(xPos, scoreLine.Transform.Position.Y));
                }
            }
        }

        private void RiseZone() {
            risingZoneTimer -= Time.DeltaTime;
            if (!risingZone.IsRising && risingZoneTimer < 0) {
                risingZone.SetCharacter(marshmallow);
                risingZone.Entity.Position = new Vector2(0, GameRoot.DesiredWindowHeight);
                risingZone.Collider.SetWidth(SceneWidth * 2);
                risingZone.BeginRising();
                risingZoneTimer = risingZoneRateIncreaseInterval;
            } else if (risingZone.IsRising && risingZoneTimer < 0) {
                risingZoneTimer = risingZoneRateIncreaseInterval;
                risingZone.IncreaseRiseRate(.4f);
            }
        }

        private void CheckForGameOver() {
            if (marshmallow.IsDead && !isGameOver) {
                Camera.Entity.RemoveComponent<FollowCamera>();
                gameOverlay.OnGameOver();
                isSpawningBlocks = false;
                isGameOver = true;
            }
        }

        private void SetUpWorldBounds(CameraBounds camBounds) {
            int boundThickness = 45;
            CreateWall(new RectangleF(
                camBounds.MinX,
                camBounds.MinY - boundThickness / 2,
                camBounds.MaxX - camBounds.MinX,
                boundThickness * 2
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

        private Color GetRandomColor() {
            System.Random rand = new System.Random();
            int r = rand.Next(50, 235);
            int g = rand.Next(50, 235);
            int b = rand.Next(50, 235);
            return new Color(r, g, b);
        }
    }
}
