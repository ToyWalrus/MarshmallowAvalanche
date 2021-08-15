using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    // Top left corner is 0,0 in local coordinates
    public class MovingObject {
        public MovingObject(Vector2 position, Vector2 size) {
            GravityModifier = 1;
            Position = position;
            Size = size;

            onRightWall = false;
            onLeftWall = false;
            onGround = Position.Y == GameRoot.DesiredWindowHeight - Size.Y;
        }

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
        public Vector2 Velocity {
            get => _velocity;
            set {
                PreviousVelocity = _velocity;
                _velocity = value;
            }
        }
        public Vector2 PreviousVelocity { get; protected set; }

        public float GravityModifier { get; set; }
        public Vector2 Size { get; protected set; }
        public Vector2 BoundingBoxOffset { get => _position; }

        protected bool wasOnRightWall;
        protected bool onRightWall;

        protected bool wasOnLeftWall;
        protected bool onLeftWall;

        protected bool wasOnGround;
        protected bool onGround;

        public virtual void Update(GameTime gt) {
            wasOnGround = onGround;
            wasOnRightWall = onRightWall;
            wasOnLeftWall = onLeftWall;

            _velocity.Y += GravityModifier * 9.8f * (float)gt.ElapsedGameTime.TotalSeconds;

            Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        private void CheckForGrounded() {
            if (_position.Y + Size.Y >= GameRoot.DesiredWindowHeight) {
                _position.Y = GameRoot.DesiredWindowHeight - Size.Y;
                onGround = true;
            } else {
                onGround = false;
            }
        }

        private void CheckForOnLeftWall() {
            if (_position.X <= 0) {
                _position.X = 0;
                onLeftWall = true;
            } else {
                onLeftWall = false;
            }
        }

        private void CheckForOnRightWall() {
            if (_position.X + Size.X >= GameRoot.DesiredWindowWidth) {
                _position.X = GameRoot.DesiredWindowWidth - Size.X;
                onRightWall = true;
            } else {
                onRightWall = false;
            }
        }
    }
}
