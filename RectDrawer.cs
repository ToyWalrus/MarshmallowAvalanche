using System;
using System.Collections.Generic;
using System.Text;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche {
    public class RectDrawer {
        public PhysicsObject PhysicsObject { get; private set; }
        private Texture2D tex;
        private bool hasBeenInitialized;
        private Color color;

        private Vector2 size;

        public RectDrawer(PhysicsObject obj) {
            PhysicsObject = obj;
            hasBeenInitialized = false;
            color = Color.White;
        }

        public RectDrawer(PhysicsObject obj, Color color) : this(obj) {
            this.color = color;
        }

        public RectDrawer(Vector2 size, Color color) {
            this.size = size;
            this.color = color;
        }

        public void Initialize(GraphicsDevice gd) {
            if (color != null) {
                Initialize(gd, color);
            } else {
                Initialize(gd, Color.White);
            }
        }

        public void Initialize(GraphicsDevice gd, Color color) {
            hasBeenInitialized = true;
            tex = new Texture2D(gd, 1, 1);
            SetColor(color);
        }

        public void Dispose() {
            tex.Dispose();
        }

        public void SetColor(Color color) {
            this.color = color;
            if (hasBeenInitialized) {
                tex.SetData(new Color[] { color });
            }
        }

        public void Draw(SpriteBatch sb) {
            if (PhysicsObject == null) {
                throw new Exception("No physics object assigned to this rect drawer!");
            }
            sb.Draw(tex, PhysicsObject.Bounds, Color.White);
        }

        public void Draw(SpriteBatch sb, Vector2 location) {
            if (size == null) {
                if (PhysicsObject == null) {
                    throw new Exception("No physics object assigned to this rect drawer!");
                }
                sb.Draw(tex, new Rectangle(location.ToPoint(), PhysicsObject.Size.ToPoint()), Color.White);
            } else {
                sb.Draw(tex, new Rectangle(location.ToPoint(), size.ToPoint()), Color.White);
            }
        }
    }
}
