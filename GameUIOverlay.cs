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
        private bool displayingGameOverUI = false;

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

            labelStyle = new LabelStyle(font, Color.Cyan);
            labelTable = Stage.AddElement(new Table());
            buttonTable = Stage.AddElement(new Table());
        }

        public override void Update() {
            base.Update();
            if (displayingGameOverUI)
                return;

            labelTable.Clear();
            labelTable.SetFillParent(true);
            labelTable.Top().Left().PadTop(10).PadLeft(10);

            LabelStyle style = new LabelStyle()
            {
                Font = labelStyle.Font,
                FontColor = Color.WhiteSmoke,
                FontScale = .6f,
            };

            Label topScoreLabel = new Label($"Top Score: {score.TopScore:F1}", style);
            Label currentScoreLabel = new Label($"Score: {score.CurrentScore:F1}", style);

            labelTable.Add(topScoreLabel).Left();
            labelTable.Row().SetPadTop(10);
            labelTable.Add(currentScoreLabel).Left();
        }

        public void OnGameOver() {
            displayingGameOverUI = true;

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
