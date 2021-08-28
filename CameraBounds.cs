using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class CameraBounds : Component, IUpdatable {

        private float _minY;
        public float MinY { get => _minY + ExtraCamPadding.Y; set => _minY = value; }

        private float _minX;
        public float MinX { get => _minX + ExtraCamPadding.X; set => _minX = value; }

        private float _maxX;
        public float MaxX { get => _maxX - ExtraCamPadding.X; set => _maxX = value; }

        public Vector2 ExtraCamPadding { get; set; }

        public CameraBounds(float minY, float minX, float maxX) {
            ExtraCamPadding = Vector2.Zero;
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
