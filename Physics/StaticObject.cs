using MarshmallowAvalanche.Utils;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class StaticObject : PhysicsObject {
        public const string DefaultTag = "StaticObject";

        public StaticObject(Vector2 position, Vector2 size, string tag = DefaultTag) : base(position, size) {
            Tag = tag;
        }

        public StaticObject(Rectangle bounds, string tag = DefaultTag) : base(bounds) {
            Tag = tag;
        }

        public override string Tag {
            get;
            protected set;
        }

        public bool hasSetCollisionBoundsInWorld = false;


        public override bool CanCollideWith(PhysicsObject other) {
            return !other.IsStatic;
        }

        public override void Update(GameTime gt) { }
        public override void AddCollision(CollisionData collision) { }
    }
}
