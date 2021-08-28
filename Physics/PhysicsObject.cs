using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche.Physics {
    public abstract class PhysicsObject : Entity {
        public bool IsStatic => this is StaticObject;
        public bool IsDynamic => this is MovingObject;

        public Vector2 Size => Collider.Bounds.Size;
        public RectangleF Bounds => Collider.Bounds;
        public BoxCollider Collider {
            get;
            protected set;
        }

        public PhysicsObject(Vector2 position, Vector2 size) {
            Collider = AddComponent(new BoxCollider(position.X, position.Y, size.X, size.Y));
        }

        public PhysicsObject(Rectangle bounds) {
            Collider = AddComponent(new BoxCollider(bounds));
        }

        public abstract void Update(GameTime gt);

        public virtual bool CanCollideWith(PhysicsObject other) {
            return true;
        }
    }
}
