using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public abstract class PhysicsObject {
        protected List<CollisionData> allCollidingObjects = new List<CollisionData>();

        public bool IsStatic => this is StaticObject;
        public bool IsDynamic => this is MovingObject;

        protected Vector2 _position;
        public virtual Vector2 Position {
            get => _position;
            set => _position = value;
        }

        protected Vector2 _size;
        public virtual Vector2 Size {
            get => Size;
            set => _size = value;
        }

        public Rectangle Bounds => new Rectangle(Position.ToPoint(), Size.ToPoint());

        public PhysicsObject(Vector2 position, Vector2 size) {
            _position = position;
            _size = size;
        }

        public PhysicsObject(Rectangle bounds) {
            _position = bounds.Location.ToVector2();
            _size = bounds.Size.ToVector2();
        }
    }
}
