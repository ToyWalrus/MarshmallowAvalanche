using System;
using Microsoft.Xna.Framework;
using Nez;

//https://gamedevelopment.tutsplus.com/tutorials/basic-2d-platformer-physics-part-2--cms-25922
namespace MarshmallowAvalanche.Physics {
    // Top left corner is 0,0 in local coordinates
    public abstract class MovingObject : PhysicsObject {
        public const float GravityConst = 9.8f;

        public MovingObject(Vector2 size) : base(size) {
            gravityModifier = 1;
            MaxFallSpeed = 1000;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        public MovingObject(Rectangle bounds) : base(bounds.Size.ToVector2()) {
            gravityModifier = 1;
            MaxFallSpeed = 1000;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            _mover = Entity.AddComponent<Mover>();
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

        protected Mover _mover;

        public virtual float MaxFallSpeed { get; set; }

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

        public override void Update() {
            ResetCollisionStatus();

            _velocity.Y += gravityModifier * GravityConst * GetDirectionalSpeedModifier();
            _velocity.Y = MathF.Min(_velocity.Y, MaxFallSpeed);
            Vector2 deltaMovement = _velocity * Time.DeltaTime;

            if (_mover.Move(deltaMovement, out _collisionData)) {
                SetTouchingBorder(_collisionData.Collider.GetComponent<PhysicsObject>());
            }
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
            _previousFrameCollisionData = _collisionData;

            Grounded = false;
            OnLeftWall = false;
            OnRightWall = false;
            TouchingTopEdge = false;
        }

        public virtual void SetTouchingBorder(PhysicsObject other) {
            if (other == null) return;
            Vector2 overlap = _collisionData.MinimumTranslationVector;

            if (overlap.X < 0) {
                OnLeftWall = true;
                if (_velocity.X < 0) {
                    _velocity.X = 0;
                }
            }

            if (overlap.X > 0) {
                OnRightWall = true;
                if (_velocity.X > 0) {
                    _velocity.X = 0;
                }
            }

            if (overlap.Y < 0) {
                TouchingTopEdge = true;
                if (_velocity.Y < 0) {
                    _velocity.Y = 0;
                }
            }

            if (overlap.Y > 0) {
                Grounded = true;
                if (_velocity.Y > 0) {
                    _velocity.Y = 0;
                }
            }
        }

    }
}
