using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche.Physics {
    public abstract class PhysicsObject : Component, IUpdatable {
        private Vector2 _initialSize;

        public Vector2 Size => Collider.Bounds.Size;
        public RectangleF Bounds => Collider.Bounds;
        public BoxCollider Collider { get; private set; }

        protected CollisionResult _collisionResult;

        public override void OnAddedToEntity() {
            Collider = Entity.GetComponent<BoxCollider>();

            if (Collider == null) {
                Collider = Entity.AddComponent<BoxCollider>();
            }

            Collider.SetSize(_initialSize.X, _initialSize.Y);
            Collider.SetLocalOffset(Vector2.Zero);
        }

        public PhysicsObject() : this(Vector2.Zero) { }
        public PhysicsObject(Vector2 size) {
            _initialSize = size;
        }

        public abstract void Update();
    }

    [System.Flags]
    public enum PhysicsLayers {
        None = 0,
        Static = 1 << 0,
        Block = 1 << 1,
        Marshmallow = 1 << 2,
    }
}
