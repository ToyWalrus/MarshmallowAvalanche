﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
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
            get => _size;
            set => _size = value;
        }

        public abstract string Tag {
            get;
            protected set;
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

        public abstract void Update(GameTime gt);

        public virtual bool CanCollideWith(PhysicsObject other) {
            return true;
        }

        public void AddCollision(CollisionData collision) {
            allCollidingObjects.Add(collision);
        }

        protected void ClearCollisions() {
            allCollidingObjects.Clear();
        }
    }
}