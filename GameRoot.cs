using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToyWalrus.Rendering;


namespace MarshmallowAvalanche {
    public class GameRoot : Game {
        private GraphicsDeviceManager g;
        private SpriteBatch sb;

        //private Camera camera;
        private RectDrawer whiteRectangle;
        private RectDrawer background;
        private Vector2 rectPosition;

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
            rectPosition = new Vector2(DesiredWindowWidth / 2, DesiredWindowHeight - 60);

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

        }

        protected override void UnloadContent() {
            base.UnloadContent();
            sb.Dispose();
            whiteRectangle.Dispose();
            background.Dispose();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float speed = 200;
            float delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Right)) {
                rectPosition.X += delta;
            }
            if (keyboard.IsKeyDown(Keys.Left)) {
                rectPosition.X -= delta;
            }
            if (keyboard.IsKeyDown(Keys.Up)) {
                rectPosition.Y -= delta;
            }
            if (keyboard.IsKeyDown(Keys.Down)) {
                rectPosition.Y += delta;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            //camera.BeforeDraw();

            sb.Begin(/*transformMatrix: camera.ViewTransformMatrix*/);
            background.Draw(sb, Vector2.Zero);
            whiteRectangle.Draw(sb, rectPosition);
            sb.End();

            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e) {
            //camera.InitializeResolutionIndependence(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }
    }
}
