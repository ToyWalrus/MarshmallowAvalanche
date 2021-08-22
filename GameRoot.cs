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
        private const float blockSpawnInterval = 2f;
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

            blockSpawner = new BlockSpawner(new RectF(50, -50, DesiredWindowWidth - 50, 10), new Vector2(100, 100), new Vector2(250, 250));

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            marshmallow.UpdateKeyboardState(keyboard);

            blockSpawnTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (blockSpawnTimer <= 0) {
                blockSpawnTimer = blockSpawnInterval;
                FallingBlock block = blockSpawner.SpawnBlock(keepSquare: true);
                blockColors.Add(block, GetRandomColor());
                world.SpawnObject(block);
            }

            if (ShouldUpdate(gt)) {
                world.Update(gt);
            }

            base.Update(gt);
        }

        protected override void Draw(GameTime gameTime) {
            //camera.BeforeDraw();

            sb.Begin(/*transformMatrix: camera.ViewTransformMatrix*/);

            background.Draw(sb, Vector2.Zero);

            // ======== Debug ===========
            //world.DrawGrid(sb, 1f);
            //world.DrawAllSpawnedObjects(sb, Color.Red, (p) => p is FallingBlock);

            Color textColor = Color.AntiqueWhite;
            Logger.DrawText(sb, marshmallow.State.ToString(), new Vector2(50, 20), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Grounded: {marshmallow.Grounded}", new Vector2(50, 50), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Position: {marshmallow.Position}", new Vector2(50, 110), textColor, Vector2.One, false);
            // ==========================

            foreach (FallingBlock block in blockColors.Keys) {
                sb.Draw(fallingBlockImg, block.Bounds, blockColors[block]);
            }

            foreach (RectDrawer drawer in rectDrawers) {
                drawer.Draw(sb);
            }

            sb.End();

            base.Draw(gameTime);
        }

        private const float updateInterval = 0.025f;
        private float updateTimer = updateInterval;
        private bool ShouldUpdate(GameTime gt) {
            updateTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (updateTimer < 0) {
                updateTimer = updateInterval;
                return true;
            }
            return false;
        }

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
