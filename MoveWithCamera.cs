using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class MoveWithCamera : Component, IUpdatable {
        private Camera camera;
        private Vector2 prevCamPosition;
        private bool followOnXAxis;
        private bool followOnYAxis;

        public MoveWithCamera() {
            followOnXAxis = true;
            followOnYAxis = true;
        }
        public MoveWithCamera(Camera camera) : this() {
            this.camera = camera;
        }

        public override void OnAddedToEntity() {
            if (camera == null) {
                camera = Entity.Scene.Camera;
            }

            prevCamPosition = camera.Transform.Position;
        }

        public void SetFollowOnXAxis(bool follow) {
            followOnXAxis = follow;
        }

        public void SetFollowOnYAxis(bool follow) {
            followOnYAxis = follow;
        }

        public void Update() {
            Vector2 delta = camera.Position - prevCamPosition;

            if (!followOnYAxis) {
                delta.Y = 0;
            }

            if (!followOnXAxis) {
                delta.X = 0;
            }

            Entity.Position += delta;
            prevCamPosition = camera.Position;
        }
    }
}
