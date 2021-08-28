using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class StaticObject : PhysicsObject {
        public StaticObject(Vector2 size) : base(size) { }

        public override bool CanCollideWith(PhysicsObject other) {
            return !other.IsStatic;
        }

        public override void Update() { }
    }
}
