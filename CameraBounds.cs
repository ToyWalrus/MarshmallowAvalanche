using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class CameraBounds : Component, IUpdatable {
        public float MinY { get; set; }
        public float MinX { get; set; }
        public float MaxX { get; set; }

        public CameraBounds(float minY, float minX, float maxX) {
            MinY = minY;
            MinX = minX;
            MaxX = maxX;
        }

        public override void OnAddedToEntity() {
            // make sure we run last so the camera is already moved before we evaluate its position
            Entity.UpdateOrder = int.MaxValue;
        }

        public void Update() {
            RectangleF currentBounds = Entity.Scene.Camera.Bounds;

            if (currentBounds.Bottom >= MinY) {
                Entity.Scene.Camera.Position += new Vector2(0, MinY - currentBounds.Bottom);
            }

            if (currentBounds.Left <= MinX) {
                Entity.Scene.Camera.Position += new Vector2(MinX - currentBounds.Left, 0);
            }

            if (currentBounds.Right >= MaxX) {
                Entity.Scene.Camera.Position += new Vector2(MaxX - currentBounds.Right, 0);
            }
        }
    }
}
