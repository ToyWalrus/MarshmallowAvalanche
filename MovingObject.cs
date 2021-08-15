using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

//https://gamedevelopment.tutsplus.com/tutorials/basic-2d-platformer-physics-part-2--cms-25922
namespace MarshmallowAvalanche {
    // Top left corner is 0,0 in local coordinates
    public class MovingObject {
        public const float GravityConst = 9.8f;

        public MovingObject(Vector2 position, Vector2 size) {
            gravityModifier = 1;
            Position = position;
            Size = size;

            OnRightWall = false;
            OnLeftWall = false;
            Grounded = Position.Y == GameRoot.DesiredWindowHeight - Size.Y;
        }
        
        public Vector2 Size { get; protected set; }

        protected Vector2 _position;
        public Vector2 Position {
            get => _position;
            set {
                PreviousPosition = _position;
                _position = value;

                CheckForGrounded();
                CheckForOnLeftWall();
                CheckForOnRightWall();
            }
        }
        public Vector2 PreviousPosition { get; protected set; }

        protected Vector2 _velocity;
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
        public Vector2 PreviousVelocity { get; protected set; }

        protected bool wasOnRightWall;
        public bool OnRightWall { get; private set; }

        protected bool wasOnLeftWall;
        public bool OnLeftWall { get; private set; }

        protected bool wasOnGround;
        public bool Grounded { get; private set; }

        protected float gravityModifier;

        public virtual void Update(GameTime gt) {
            wasOnGround = Grounded;
            wasOnRightWall = OnRightWall;
            wasOnLeftWall = OnLeftWall;

            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            float additionalWeight = 1;
            if (_velocity.Y > 0) {
                additionalWeight = 1.5f; // fall faster
            } else if (_velocity.Y < 0) {
                additionalWeight = .85f; // rise faster
            }

            _velocity.Y += gravityModifier * GravityConst * additionalWeight;
            Position += Velocity * deltaTime;
        }

        public virtual void SetGravityModifier(float value) {
            gravityModifier = value;
        }

        public virtual float GetGravityModifier() {
            return gravityModifier;
        }

        private void CheckForGrounded() {
            if (_position.Y + Size.Y >= GameRoot.DesiredWindowHeight) {
                _position.Y = GameRoot.DesiredWindowHeight - Size.Y;
                Grounded = true;
            } else {
                Grounded = false;
            }
        }

        private void CheckForOnLeftWall() {
            if (_position.X <= 0) {
                _position.X = 0;
                OnLeftWall = true;
            } else {
                OnLeftWall = false;
            }
        }

        private void CheckForOnRightWall() {
            if (_position.X + Size.X >= GameRoot.DesiredWindowWidth) {
                _position.X = GameRoot.DesiredWindowWidth - Size.X;
                OnRightWall = true;
            } else {
                OnRightWall = false;
            }
        }
    }
}
