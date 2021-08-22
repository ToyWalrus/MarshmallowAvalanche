using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToyWalrus.Rendering;
using MarshmallowAvalanche.Utils;
using MarshmallowAvalanche.Physics;

namespace MarshmallowAvalanche {
    public class GameRoot : Game {
        private GraphicsDeviceManager g;
        private SpriteBatch sb;

        //private Camera camera;
        private RectDrawer background;
        private List<RectDrawer> rectDrawers;
        private Character marshmallow;
        private World world;

        private BlockSpawner blockSpawner;
        private const float blockSpawnInterval = 1.25f;
        private float blockSpawnTimer = blockSpawnInterval;

        private Texture2D fallingBlockImg;
        private Dictionary<FallingBlock, Color> blockColors;

        public const int DesiredWindowHeight = 850;
        public const int DesiredWindowWidth = 600;

        public GameRoot() {
            g = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Marshmallow Avalanche";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize() {
            //camera = new Camera(new ResolutionIndependentRenderer(this), true);
            //camera.ResolutionIndependentRenderer.VirtualHeight = DesiredWindowHeight;
            //camera.ResolutionIndependentRenderer.VirtualWidth = DesiredWindowWidth;

            blockColors = new Dictionary<FallingBlock, Color>();
            background = new RectDrawer(new Vector2(DesiredWindowWidth, DesiredWindowHeight), Color.CornflowerBlue);
            marshmallow = new Character(new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight / 1.5f), new Vector2(30, 60));
            marshmallow.SetGravityModifier(5);

            rectDrawers = new List<RectDrawer> {
                new RectDrawer(marshmallow)
            };

            world = new World(DesiredWindowWidth, DesiredWindowHeight, 5, 12);
            world.SpawnObject(marshmallow);

            // add bounds        
            world.SpawnObject(new StaticObject(new Vector2(-50, DesiredWindowHeight), new Vector2(DesiredWindowWidth + 100, 100)));
            world.SpawnObject(new StaticObject(new Vector2(-50, -50), new Vector2(50, DesiredWindowHeight + 100)));
            world.SpawnObject(new StaticObject(new Vector2(DesiredWindowWidth, -50), new Vector2(50, DesiredWindowHeight + 100)));

            blockSpawner = new BlockSpawner(new RectF(50, -50, DesiredWindowWidth - 50, 10), new Vector2(80, 80), new Vector2(200, 200));

            g.PreferredBackBufferWidth = DesiredWindowWidth;
            g.PreferredBackBufferHeight = DesiredWindowHeight;
            g.ApplyChanges();

            //camera.SetPosition(new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight / 2));
            //camera.InitializeResolutionIndependence(Window.ClientBounds.Width, Window.ClientBounds.Height);
            base.Initialize();
        }

        protected override void LoadContent() {
            sb = new SpriteBatch(GraphicsDevice);
            background.Initialize(GraphicsDevice);
            foreach (RectDrawer drawer in rectDrawers) {
                drawer.Initialize(GraphicsDevice);
            }

            Logger.InitFont(Content.Load<SpriteFont>("DefaultFont"));
            fallingBlockImg = Content.Load<Texture2D>("FallingBlock");
        }

        protected override void UnloadContent() {
            base.UnloadContent();
            sb.Dispose();
            background.Dispose();
            foreach (RectDrawer drawer in rectDrawers) {
                drawer.Dispose();
            }
            Logger.DisposeTexture();
        }

        protected override void Update(GameTime gt) {
            KeyboardState keyboard = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape)) {
                Exit();
            }

            marshmallow.UpdateKeyboardState(keyboard);

            UpdateKeyStateForDebugging(gt, keyboard);
            if (ShouldUpdate(gt)) {

                blockSpawnTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
                if (blockSpawnTimer <= 0 && !pauseBlockSpawn) {
                    blockSpawnTimer = blockSpawnInterval;

                    FallingBlock block = blockSpawner.SpawnBlock(keepSquare: true);
                    blockColors.Add(block, GetRandomColor());

                    world.SpawnObject(block);
                }

                world.Update(gt);
            }

            base.Update(gt);
        }

        protected override void Draw(GameTime gameTime) {
            //camera.BeforeDraw();

            sb.Begin(/*transformMatrix: camera.ViewTransformMatrix*/);

            background.Draw(sb, Vector2.Zero);

            // ======== Debug ===========
            world.DrawGrid(sb, 1f);
            //world.DrawAllSpawnedObjects(sb, Color.Red, (p) => p is FallingBlock);
            // ==========================

            foreach (FallingBlock block in blockColors.Keys) {
                sb.Draw(fallingBlockImg, block.Bounds, blockColors[block]);
            }

            // ======== Debug ===========
            Color textColor = Color.AntiqueWhite;
            Logger.DrawText(sb, marshmallow.State.ToString(), new Vector2(50, 20), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Game slow rate: {updateInterval:F3}", new Vector2(50, 60), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Position: {marshmallow.Position}", new Vector2(50, 100), textColor, Vector2.One, false);
            if (pauseBlockSpawn) {
                Logger.DrawText(sb, "Block spawning paused", new Vector2(DesiredWindowWidth / 2f, 140), Color.Red, Vector2.One, true);
            }
            // ==========================


            foreach (RectDrawer drawer in rectDrawers) {
                drawer.Draw(sb);
            }

            sb.End();

            base.Draw(gameTime);
        }

        #region Debugging
        private float updateInterval = 0.00f;
        private float updateTimer = 0;
        private bool ShouldUpdate(GameTime gt) {
            updateTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (updateTimer < 0) {
                updateTimer = updateInterval;
                return true;
            }
            return false;
        }

        private const float debounce = .5f;
        private float debounceTimer = debounce;
        private bool pauseBlockSpawn = false;
        private void UpdateKeyStateForDebugging(GameTime gt, KeyboardState keyboard) {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            if (debounceTimer <= 0) {
                if (keyboard.IsKeyDown(Keys.Space)) {
                    debounceTimer = debounce;
                    pauseBlockSpawn = !pauseBlockSpawn;
                }

                float updateIntervalStep = .002f;
                if (keyboard.IsKeyDown(Keys.OemMinus)) {
                    debounceTimer = debounce / 2f;
                    updateInterval = MathF.Max(0, updateInterval - updateIntervalStep);
                }
                if (keyboard.IsKeyDown(Keys.OemPlus)) {
                    debounceTimer = debounce / 2f;
                    updateInterval = MathF.Min(1, updateInterval + updateIntervalStep);
                }
            }

            debounceTimer -= deltaTime;
        }
        #endregion

        private Color GetRandomColor() {
            Random rand = new Random();
            int r = rand.Next(50, 235);
            int g = rand.Next(50, 235);
            int b = rand.Next(50, 235);
            return new Color(r, g, b);
        }

        private void OnResize(object sender, EventArgs e) {
            //camera.InitializeResolutionIndependence(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }
    }
}
