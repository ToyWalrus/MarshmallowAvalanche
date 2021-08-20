using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MarshmallowAvalanche.Utils;

//https://gamedevelopment.tutsplus.com/tutorials/basic-2d-platformer-physics-part-2--cms-25922
namespace MarshmallowAvalanche.Physics {
    // Top left corner is 0,0 in local coordinates
    public abstract class MovingObject : PhysicsObject {
        public const float GravityConst = 9.8f;

        public MovingObject(Vector2 position, Vector2 size) : base(position, size) {
            gravityModifier = 1;
            MaxFallSpeed = 500;
            TouchingTopEdge = false;
            OnRightWall = false;
            OnLeftWall = false;
            Grounded = false;
        }

        public MovingObject(Rectangle bounds) : base(bounds) {
            gravityModifier = 1;
            MaxFallSpeed = 500;
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
        public float MaxFallSpeed { get; set; }

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

            CalculateNewPosition(gt, Position, Velocity, out Vector2 newPos, out Vector2 newVel);

            Position = newPos;
            Velocity = newVel;

            Logger.LogToConsole("--- Update object ---");
        }

        public virtual void SetGravityModifier(float value) {
            gravityModifier = value;
        }

        public virtual float GetGravityModifier() {
            return gravityModifier;
        }

        private void CalculateNewPosition(GameTime gt, Vector2 currentPosition, Vector2 currentVelocity, out Vector2 newPosition, out Vector2 newVelocity) {
            Vector2 calculatedVelocity = currentVelocity;

            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            float directionalSpeedModifier = 1;
            if (calculatedVelocity.Y > 0) {
                directionalSpeedModifier = 1.25f; // fall faster
            } else if (calculatedVelocity.Y < 0 && !OnLeftWall && !OnRightWall) {
                directionalSpeedModifier = .85f; // rise faster
            }

            calculatedVelocity.Y += gravityModifier * GravityConst * directionalSpeedModifier;
            calculatedVelocity.Y = MathF.Min(calculatedVelocity.Y, MaxFallSpeed);

            CheckForCollisions(currentPosition + calculatedVelocity * deltaTime, calculatedVelocity, out newPosition, out newVelocity);
        }

        private void CheckForCollisions(Vector2 currentPosition, Vector2 currentVelocity, out Vector2 newPosition, out Vector2 newVelocity) {
            newPosition = currentPosition;
            newVelocity = currentVelocity;

            OnLeftWall = false;
            OnRightWall = false;
            TouchingTopEdge = false;
            Grounded = false;

            for (int i = 0; i < allCollidingObjects.Count; ++i) {
                CollisionData cd = allCollidingObjects[i];
                RectF otherBounds = cd.other.Bounds;

                // We only care about the lesser overlap amount
                bool xIsSmaller = Math.Abs(cd.overlap.X) < Math.Abs(cd.overlap.Y);
                if (xIsSmaller) {
                    if (cd.overlap.X < 0) {
                        OnLeftWall = true;
                        newPosition.X = otherBounds.Right;
                        if (currentVelocity.X < 0) {
                            newVelocity.X = 0;
                        }
                    }
                    if (cd.overlap.X > 0) {
                        OnRightWall = true;
                        newPosition.X = otherBounds.Left - Size.X;
                        if (currentVelocity.X > 0) {
                            newVelocity.X = 0;
                        }
                    }
                } else {
                    if (cd.overlap.Y < 0) {
                        TouchingTopEdge = true;
                        newPosition.Y = otherBounds.Bottom;
                        if (currentVelocity.Y < 0) {
                            newVelocity.Y = 0;
                        }
                    }
                    if (cd.overlap.Y > 0) {
                        Grounded = true;
                        newPosition.Y = otherBounds.Top - Size.Y;
                        if (currentVelocity.Y > 0) {
                            newVelocity.Y = 0;
                        }
                    }
                }
            }
        }
    }
}
