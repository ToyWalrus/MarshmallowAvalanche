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

        public const int DesiredWindowHeight = 800;
        public const int DesiredWindowWidth = 500;

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

            background = new RectDrawer(new Vector2(DesiredWindowWidth, DesiredWindowHeight), Color.CornflowerBlue);
            marshmallow = new Character(new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight / 1.5f), new Vector2(30, 60));
            marshmallow.SetGravityModifier(5);

            rectDrawers = new List<RectDrawer>();
            rectDrawers.Add(new RectDrawer(marshmallow));

            world = new World(DesiredWindowWidth, DesiredWindowHeight, 5, 12);
            world.SpawnObject(marshmallow);

            // add bounds        
            world.SpawnObject(new StaticObject(new Vector2(-50, DesiredWindowHeight), new Vector2(DesiredWindowWidth + 100, 100)));
            world.SpawnObject(new StaticObject(new Vector2(-100, -50), new Vector2(100, DesiredWindowHeight + 100)));
            world.SpawnObject(new StaticObject(new Vector2(DesiredWindowWidth, -50), new Vector2(100, DesiredWindowHeight + 100)));

            blockSpawner = new BlockSpawner(new RectF(50, -50, DesiredWindowWidth - 50, 50), new Vector2(100, 100), new Vector2(250, 250));

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
                world.SpawnObject(blockSpawner.SpawnBlock());
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
            world.DrawAllSpawnedObjects(sb, Color.Red, (p) => p is FallingBlock);

            Color textColor = Color.AntiqueWhite;
            Logger.DrawText(sb, marshmallow.State.ToString(), new Vector2(50, 20), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Grounded: {marshmallow.Grounded}", new Vector2(50, 50), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Position: {marshmallow.Position}", new Vector2(50, 110), textColor, Vector2.One, false);
            // ==========================

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

        private void OnResize(object sender, EventArgs e) {
            //camera.InitializeResolutionIndependence(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }
    }
}
