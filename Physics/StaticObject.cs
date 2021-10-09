using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class StaticObject : PhysicsObject {
        public StaticObject(Vector2 size) : base(size) { }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            Collider.PhysicsLayer = (int)PhysicsLayers.Static;
            Collider.CollidesWithLayers = (int)PhysicsLayers.None;
        }

        public override void Update() { }
    }
}
