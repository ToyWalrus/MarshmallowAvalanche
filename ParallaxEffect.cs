using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;


namespace MarshmallowAvalanche {
    public class ParallaxEffect : SpriteRenderer {
        public Vector2 Speed { get; set; }

        private Vector2 offset;
        private Vector2 prevCamPosition;

        public ParallaxEffect(Texture2D texture, Vector2 speed) : base(texture) {
            Speed = speed;
            offset = Vector2.Zero;
            prevCamPosition = new Vector2(float.MinValue, float.MinValue);
        }

        public override void Render(Batcher batcher, Camera camera) {
            if (prevCamPosition.X != float.MinValue) {
                Vector2 direction = prevCamPosition - camera.Position;
                offset += direction * Speed * Time.DeltaTime;
            }

            batcher.Draw(Sprite, Entity.Transform.Position + LocalOffset - offset, Color,
                Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);

            prevCamPosition = camera.Position;
        }
    }
}
