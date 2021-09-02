using Nez;
using Nez.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche {
    public class MainMenu : Scene {
        private UICanvas canvas;
        private Table table;
        private Nez.BitmapFonts.BitmapFont font;

        public override void Initialize() {
            base.Initialize();

            font = Content.LoadBitmapFont("Content/Marshmallow.fnt", true);
            Texture2D buttonTex = Content.LoadTexture("ui/ButtonTexture");
            Texture2D buttonTexHover = Content.LoadTexture("ui/ButtonTexture-Hover");
            Texture2D buttonTexPressed = Content.LoadTexture("ui/ButtonTexture-Pressed");

            Skin buttonSkin = new Skin();
            buttonSkin.Add("default", new TextButtonStyle {
                Up = new SpriteDrawable(buttonTex),
                Over = new SpriteDrawable(buttonTexHover),
                Down = new SpriteDrawable(buttonTexPressed),
                Font = font,
                FontScale = 1,
                FontColor = Color.DeepPink,
                DownFontColor = Color.White,
            });

            canvas = CreateEntity("ui-canvas").AddComponent<UICanvas>();
            table = canvas.Stage.AddElement(new Table());

            table.SetFillParent(true).Top().PadTop(40);
            table.Add(new Label("Marshmallow\n Avalanche!", new LabelStyle {
                Font = font,
                FontColor = Color.White,
                FontScale = 1.25f,
            }));

            TextButton playButton = new TextButton("play", buttonSkin);
            TextButton quitButton = new TextButton("quit", buttonSkin);

            table.Row().SetPadTop(GameRoot.DesiredWindowHeight / 2);
            table.Add(playButton)
                 .Bottom()
                 .SetMinWidth(300)
                 .SetFillY();

            table.Row().SetPadTop(10);
            table.Add(quitButton)
                .Top()
                .SetMinWidth(300)
                .SetMaxHeight(75);

            playButton.OnClicked += OnClickPlay;
            quitButton.OnClicked += OnClickQuit;
        }

        private void OnClickPlay(Button b) {
            Core.StartSceneTransition(new CrossFadeTransition(() => new MainScene(GameRoot.DesiredWindowWidth * 1.5f)));
        }

        private void OnClickQuit(Button b) {
            Core.Exit();
        }
    }
}
