using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class StaticObject : PhysicsObject {
        public StaticObject(Vector2 position, Vector2 size) : base(position, size) { }
        public StaticObject(Rectangle bounds) : base(bounds) { }

        public override bool CanCollideWith(PhysicsObject other) {
            return !other.IsStatic;
        }

        public override void Update(GameTime gt) { }
    }
}
