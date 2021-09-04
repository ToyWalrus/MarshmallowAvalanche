using Nez;
using Nez.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche {
    public class GameUIOverlay : UICanvas {
        new public const int RenderLayer = -200;

        private Table labelTable;
        private Table buttonTable;
        private LabelStyle labelStyle;
        private Skin buttonSkin;
        private Score score;

        public GameUIOverlay(Score score) {
            this.score = score;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            base.RenderLayer = RenderLayer;

            var content = Entity.Scene.Content;

            var font = content.LoadBitmapFont("Content/Marshmallow.fnt", true);
            Texture2D buttonTex = content.LoadTexture("ui/ButtonTexture");
            Texture2D buttonTexHover = content.LoadTexture("ui/ButtonTexture-Hover");
            Texture2D buttonTexPressed = content.LoadTexture("ui/ButtonTexture-Pressed");

            buttonSkin = new Skin();
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

            labelStyle = new LabelStyle(font, Color.Red);
            labelTable = Stage.AddElement(new Table());
            buttonTable = Stage.AddElement(new Table());
        }

        public void OnGameOver() {
            score.SaveData();

            labelTable.Clear();
            labelTable.SetFillParent(true).Center();

            Label gameOverLabel = new Label("Game\nOver!", labelStyle);
            gameOverLabel.SetFontScale(2);
            labelTable.Add(gameOverLabel);


            buttonTable.Clear();
            buttonTable.SetFillParent(true).Bottom();
            buttonTable.Row().SetPadBottom(50);

            TextButton tryAgainButton = new TextButton("Try again", buttonSkin);
            buttonTable.Add(tryAgainButton)
                .Center()
                .SetMaxHeight(70);

            tryAgainButton.OnClicked += OnTryAgainPressed;
        }

        private void OnTryAgainPressed(Button b) {
            float currentSceneWidth = (Entity.Scene as MainScene).SceneWidth;
            Core.StartSceneTransition(new FadeTransition(() => new MainScene(currentSceneWidth)));
        }
    }
}
