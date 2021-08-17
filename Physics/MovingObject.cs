using System.Collections.Generic;
using Microsoft.Xna.Framework;

//https://gamedevelopment.tutsplus.com/tutorials/basic-2d-platformer-physics-part-2--cms-25922
namespace MarshmallowAvalanche.Physics {
    // Top left corner is 0,0 in local coordinates
    public abstract class MovingObject : PhysicsObject {
        public const float GravityConst = 9.8f;

        public MovingObject(Vector2 position, Vector2 size) : base(position, size) {
            gravityModifier = 1;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        public MovingObject(Rectangle bounds) : base(bounds) {
            gravityModifier = 1;
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
            set {
                PreviousVelocity = _velocity;
                _velocity = value;
            }
        }
        protected Vector2 _velocity;

        public override Vector2 Position {
            get => _position;
            set {
                PreviousPosition = _position;
                _position = value;
            }
        }

        public Vector2 PreviousVelocity { get; protected set; }
        public Vector2 PreviousPosition { get; protected set; }

        protected bool wasOnRightWall = false;
        public bool OnRightWall { get; private set; }

        protected bool wasOnLeftWall;
        public bool OnLeftWall { get; private set; }

        protected bool wasTouchingTopEdge;
        public bool TouchingTopEdge { get; private set; }

        protected readonly int inputGracePeriod = 3;
        protected int ticksSinceLeavingGround = 0;
        protected bool wasOnGround;
        public bool Grounded { get; private set; }

        protected float gravityModifier;

        public override void Update(GameTime gt) {
            if (!Grounded && wasOnGround) {
                ticksSinceLeavingGround++;
                if (ticksSinceLeavingGround > inputGracePeriod) {
                    wasOnGround = false;
                    ticksSinceLeavingGround = 0;
                }
            } else {
                wasOnGround = Grounded;
                ticksSinceLeavingGround = 0;
            }

            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            float directionalSpeedModifier = 1;
            if (_velocity.Y > 0) {
                directionalSpeedModifier = 1.25f; // fall faster
            } else if (_velocity.Y < 0 && !OnLeftWall && !OnRightWall) {
                directionalSpeedModifier = .85f; // rise faster
            }

            _velocity.Y += gravityModifier * GravityConst * directionalSpeedModifier;
            Position += Velocity * deltaTime;

            CheckForCollisions();
        }

        public virtual void SetGravityModifier(float value) {
            gravityModifier = value;
        }

        public virtual float GetGravityModifier() {
            return gravityModifier;
        }

        private void CheckForCollisions() {
            OnLeftWall = false;
            OnRightWall = false;
            TouchingTopEdge = false;
            Grounded = false;

            for (int i = 0; i < allCollidingObjects.Count; ++i) {
                CollisionData cd = allCollidingObjects[i];
                if (cd.overlap.X < 0) {
                    OnLeftWall = true;
                    if (_velocity.X < 0) {
                        _position.X = cd.other.Bounds.Right;
                    }
                }
                if (cd.overlap.X > 0) {
                    OnRightWall = true;
                    if (_velocity.X > 0) {
                        _position.X = cd.other.Bounds.Right - Size.X;
                    }
                }
                if (cd.overlap.Y < 0) {
                    TouchingTopEdge = true;
                    if (_velocity.Y < 0) {
                        _position.Y = cd.other.Bounds.Bottom;
                    }
                }
                if (cd.overlap.Y > 0) {
                    Grounded = true;
                    if (_velocity.Y > 0) {
                        _position.Y = cd.other.Bounds.Top - Size.Y;
                    }
                }
            }

            ClearCollisions();
        }
    }
}
