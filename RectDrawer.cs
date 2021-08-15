using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche {
    public class RectDrawer {
        public Vector2 Size { get; private set; }
        private Texture2D tex;

        public RectDrawer(Vector2 dimensions) {
            this.Size = dimensions;
        }

        public void Initialize(GraphicsDevice gd) {
            Initialize(gd, Color.White);
        }

        public void Initialize(GraphicsDevice gd, Color color) {
            tex = new Texture2D(gd, 1, 1);
            SetColor(color);
        }

        public void Dispose() {
            tex.Dispose();
        }

        public void SetColor(Color color) {
            tex.SetData(new Color[] { color });
        }

        public void Draw(SpriteBatch sb, Vector2 location) {
            Rectangle rect = new Rectangle(location.ToPoint(), Size.ToPoint());
            sb.Draw(tex, rect, Color.White);
        }
    }
}
