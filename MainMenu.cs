using Nez;
using Nez.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche {
    public class MainMenu : Scene {
        private UICanvas canvas;
        private Table table;
        private Nez.BitmapFonts.BitmapFont font;
        private bool hasBeenInitialized = false;

        public override void Initialize() {
            base.Initialize();
            AddRenderer(new DefaultRenderer());

            DrawMenu();
            hasBeenInitialized = true;
        }

        private void DrawMenu() {
            Score score = Score.LoadData();

            if (!hasBeenInitialized) {
                font = Content.LoadBitmapFont("Content/Marshmallow.fnt", true);
                canvas = CreateEntity("ui-canvas").AddComponent<UICanvas>();
                table = canvas.Stage.AddElement(new Table());
            } else {
                table.Clear();
            }


            Texture2D buttonTex = Content.LoadTexture("ui/ButtonTexture");
            Texture2D buttonTexHover = Content.LoadTexture("ui/ButtonTexture-Hover");
            Texture2D buttonTexPressed = Content.LoadTexture("ui/ButtonTexture-Pressed");

            Skin buttonSkin = new Skin();
            buttonSkin.Add("default", new TextButtonStyle
            {
                Up = new SpriteDrawable(buttonTex),
                Over = new SpriteDrawable(buttonTexHover),
                Down = new SpriteDrawable(buttonTexPressed),
                Font = font,
                FontScale = 1,
                FontColor = Color.DeepPink,
                DownFontColor = Color.White,
            });
            buttonSkin.Add("quiet", new TextButtonStyle
            {
                Font = font,
                FontScale = .75f,
                FontColor = Color.DeepPink,
                OverFontColor = Color.Pink,
                DownFontColor = Color.White,
            });

            table.SetFillParent(true).Top().PadTop(20);
            table.Add(new Label("Marshmallow\n Avalanche!", new LabelStyle
            {
                Font = font,
                FontColor = Color.White,
                FontScale = 1.25f,
            })).Top().Height(400);

            table.Row();
            table.Add(new Label($"Top score: {score.TopScore:F1}\"", new LabelStyle
            {
                Font = font,
                FontColor = Color.White,
                FontScale = .75f,
            })).Top().SetFillY();

            TextButton playButton = new TextButton("play", buttonSkin);
            TextButton quitButton = new TextButton("quit", buttonSkin);
            TextButton resetScoreButton = new TextButton("reset score", buttonSkin.Get<TextButtonStyle>("quiet"));

            table.Row().SetPadTop(10);
            table.Add(playButton)
                 .Bottom()
                 .SetMinWidth(300)
                 .SetFillY();

            table.Row().SetPadTop(10);
            table.Add(quitButton)
                .Top()
                .SetMinWidth(300)
                .SetMaxHeight(75);

            table.Row().SetPadBottom(10);
            table.Add(resetScoreButton)
                .Bottom();

            playButton.OnClicked += OnClickPlay;
            quitButton.OnClicked += OnClickQuit;
            resetScoreButton.OnClicked += OnClickResetScore;
        }

        private void OnClickPlay(Button b) {
            Core.StartSceneTransition(new CrossFadeTransition(() => new MainScene(GameRoot.DesiredWindowWidth * 1.5f)));
        }

        private void OnClickQuit(Button b) {
            Core.Exit();
        }

        private void OnClickResetScore(Button b) {
            Score.ResetTopScore();
            DrawMenu();
        }
    }
}
