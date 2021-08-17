using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class StaticObject : PhysicsObject {
        public StaticObject(Vector2 position, Vector2 size) : base(position, size) { }
        public StaticObject(Rectangle bounds) : base(bounds) { }
    }
}
