using System;
using Microsoft.Xna.Framework;
using MarshmallowAvalanche.Utils;
using Nez;

//https://gamedevelopment.tutsplus.com/tutorials/basic-2d-platformer-physics-part-2--cms-25922
namespace MarshmallowAvalanche.Physics {
    // Top left corner is 0,0 in local coordinates
    public abstract class MovingObject : PhysicsObject {
        public const float GravityConst = 9.8f;

        public MovingObject(Vector2 position, Vector2 size) : base(position, size) {
            gravityModifier = 1;
            MaxFallSpeed = 1000;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        public MovingObject(Rectangle bounds) : base(bounds) {
            gravityModifier = 1;
            MaxFallSpeed = 1000;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        /// <summary>
        /// The Y axis is inverted, so positive velocity in the Y
        /// direction means down, while negative means up.
        /// </summary>
        public Vector2 Velocity {
            get => _velocity;
            set => _velocity = value;
        }
        protected Vector2 _velocity;

        public float MaxFallSpeed { get; set; }

        protected bool wasOnRightWall = false;
        public bool OnRightWall { get; protected set; }

        protected bool wasOnLeftWall;
        public bool OnLeftWall { get; protected set; }

        protected bool wasTouchingTopEdge;
        public bool TouchingTopEdge { get; protected set; }

        protected readonly int inputGracePeriod = 3;
        protected int ticksSinceLeavingGround = 0;
        protected bool wasOnGround;
        public bool Grounded { get; protected set; }

        protected float gravityModifier;

        public override void Update(GameTime gt) {
            _velocity.Y += gravityModifier * GravityConst * GetDirectionalSpeedModifier();
            _velocity.Y = MathF.Min(_velocity.Y, MaxFallSpeed);
            Vector2 deltaMovement = _velocity * Time.DeltaTime;

            if (Collider.CollidesWithAny(ref deltaMovement, out CollisionResult result)) {
                Debug.Log("Collision result: {0}", result);
            }

            Position += deltaMovement;
            Velocity = deltaMovement / Time.DeltaTime;
        }

        protected virtual float GetDirectionalSpeedModifier() {
            return 1;
        }

        public virtual void SetGravityModifier(float value) {
            gravityModifier = value;
        }

        public virtual float GetGravityModifier() {
            return gravityModifier;
        }

        public virtual void ResetCollisionStatus() {
            wasOnGround = Grounded;
            wasOnRightWall = OnRightWall;
            wasOnLeftWall = OnLeftWall;
            wasTouchingTopEdge = TouchingTopEdge;

            Grounded = false;
            OnLeftWall = false;
            OnRightWall = false;
            TouchingTopEdge = false;
        }

        //public override void CheckForCollisionWith(PhysicsObject other) {
        //    if (other == null || !CanCollideWith(other)) return;

        //    RectF otherBounds = other.Bounds;
        //    Vector2 _position = Position;

        //    if (Bounds.Intersects(otherBounds, out Vector2 overlap)) {

        //        // TODO: add mass? the lesser mass object moves?
        //        // We only care about the lesser overlap amount
        //        bool xIsSmaller = Math.Abs(overlap.X) < Math.Abs(overlap.Y);
        //        if (xIsSmaller) {
        //            if (overlap.X < 0) {
        //                OnLeftWall = true;
        //                _position.X = otherBounds.Right;
        //                if (_velocity.X < 0) {
        //                    _velocity.X = 0;
        //                }
        //            }
        //            if (overlap.X > 0) {
        //                OnRightWall = true;
        //                _position.X = otherBounds.Left - Size.X;
        //                if (_velocity.X > 0) {
        //                    _velocity.X = 0;
        //                }
        //            }
        //        } else {
        //            if (overlap.Y < 0) {
        //                TouchingTopEdge = true;
        //                _position.Y = otherBounds.Bottom;
        //                if (_velocity.Y < 0) {
        //                    _velocity.Y = 0;
        //                }
        //            }
        //            if (overlap.Y > 0) {
        //                Grounded = true;
        //                _position.Y = otherBounds.Top - Size.Y;
        //                if (_velocity.Y > 0) {
        //                    _velocity.Y = 0;
        //                }
        //            }
        //        }
        //    }
        //}
    
    }
}
