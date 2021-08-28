using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche.Physics {
    public abstract class PhysicsObject : Component, IUpdatable {
        public bool IsStatic => this is StaticObject;
        public bool IsDynamic => this is MovingObject;

        private Vector2 _initialSize;

        public Vector2 Size => Collider.Bounds.Size;
        public RectangleF Bounds => Collider.Bounds;
        public BoxCollider Collider {
            get;
            protected set;
        }

        protected CollisionResult _collisionResult;

        public override void OnAddedToEntity() {
            Collider = Entity.AddComponent(new BoxCollider(0, 0, _initialSize.X, _initialSize.Y));
            Collider.SetLocalOffset(Vector2.Zero);
        }

        public PhysicsObject() : this(Vector2.Zero) { }
        public PhysicsObject(Vector2 size) {
            _initialSize = size;
        }

        public abstract void Update();

        public virtual bool CanCollideWith(PhysicsObject other) {
            return true;
        }
    }
}
