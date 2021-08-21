using System;
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
        private RectDrawer whiteRectangle;
        private RectDrawer background;
        private World world;
        private Character marshmallow;

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

            whiteRectangle = new RectDrawer(new Vector2(30, 60));
            background = new RectDrawer(new Vector2(DesiredWindowWidth, DesiredWindowHeight));
            marshmallow = new Character(new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight / 1.5f), whiteRectangle.Size);
            marshmallow.SetGravityModifier(5);

            world = new World(DesiredWindowWidth, DesiredWindowHeight, 5, 12);
            world.SpawnObject(marshmallow);

            // add bounds        
            world.SpawnObject(new StaticObject(new Vector2(-50, DesiredWindowHeight - 5), new Vector2(DesiredWindowWidth + 100, 200)));
            world.SpawnObject(new StaticObject(new Vector2(-200, -50), new Vector2(205, DesiredWindowHeight + 100)));
            world.SpawnObject(new StaticObject(new Vector2(DesiredWindowWidth - 5, -50), new Vector2(200, DesiredWindowHeight + 100)));

            g.PreferredBackBufferWidth = DesiredWindowWidth;
            g.PreferredBackBufferHeight = DesiredWindowHeight;
            g.ApplyChanges();

            //camera.SetPosition(new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight / 2));
            //camera.InitializeResolutionIndependence(Window.ClientBounds.Width, Window.ClientBounds.Height);
            base.Initialize();
        }

        protected override void LoadContent() {
            sb = new SpriteBatch(GraphicsDevice);
            whiteRectangle.Initialize(GraphicsDevice);
            background.Initialize(GraphicsDevice, Color.DeepSkyBlue);
            Logger.InitFont(Content.Load<SpriteFont>("DefaultFont"));
        }

        protected override void UnloadContent() {
            base.UnloadContent();
            sb.Dispose();
            whiteRectangle.Dispose();
            background.Dispose();
            Logger.DisposeTexture();
        }

        protected override void Update(GameTime gt) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            marshmallow.UpdateKeyboardState(keyboard);

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
            world.DrawAllSpawnedObjects(sb, Color.Red, (p) => !(p is Character));

            Color textColor = Color.AntiqueWhite;
            Logger.DrawText(sb, marshmallow.State.ToString(), new Vector2(50, 20), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Grounded: {marshmallow.Grounded}", new Vector2(50, 50), textColor, Vector2.One, false);
            Logger.DrawText(sb, $"Position: {marshmallow.Position}", new Vector2(50, 110), textColor, Vector2.One, false);
            // ==========================

            whiteRectangle.Draw(sb, marshmallow.Position);

            sb.End();

            base.Draw(gameTime);
        }

        private const float updateInterval = 0.00f;
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
