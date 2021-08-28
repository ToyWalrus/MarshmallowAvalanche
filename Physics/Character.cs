using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MarshmallowAvalanche.Physics;
using Nez;

namespace MarshmallowAvalanche {
    public class Character : MovingObject {
        public enum CharacterInput {
            Left = 0,
            Right = 1,
            Jump = 2,
        }

        private float overriddenGravityModifier;
        private readonly Dictionary<CharacterInput, ICollection<Keys>> inputKeyMap;

        // The time before the input kicks in again to
        // steer back to wall
        private readonly float wallJumpGracePeriod = .1f;
        private float timeSinceRightWallJump = 0;
        private float timeSinceLeftWallJump = 0;

        public float JumpSpeed { get; set; }
        public float GroundMoveSpeed { get; set; }
        public float AirMoveSpeed { get; set; }
        public float SlideSpeed { get; set; }

        public bool doUpdate = true;

        public Character(Vector2 size) : base(size) {
            JumpSpeed = 500;
            GroundMoveSpeed = 550;
            AirMoveSpeed = GroundMoveSpeed * .8f;
            SlideSpeed = GroundMoveSpeed / 4;

            overriddenGravityModifier = float.NaN;

            inputKeyMap = new Dictionary<CharacterInput, ICollection<Keys>> {
                { CharacterInput.Left, new HashSet<Keys> { Keys.Left, Keys.A }},
                { CharacterInput.Right, new HashSet<Keys> { Keys.Right, Keys.D }},
                { CharacterInput.Jump, new HashSet<Keys> { Keys.Up, Keys.Space }}
            };
        }

        public CharacterState State {
            get {
                if (!Grounded) {
                    if (Velocity.Y > 0 && (OnLeftWall || wasOnLeftWall || OnRightWall || wasOnRightWall)) {
                        return CharacterState.Sliding;
                    } else if (Velocity.Y != 0) {
                        return CharacterState.Jumping;
                    }
                }
                if (Velocity.X != 0) {
                    return CharacterState.Moving;
                }
                return CharacterState.Idle;
            }
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            Collider.PhysicsLayer = (int)PhysicsLayers.Marshmallow;
        }

        public override float GetGravityModifier() {
            if (!float.IsNaN(overriddenGravityModifier)) {
                return overriddenGravityModifier;
            }
            return base.GetGravityModifier();
        }


        public override void Update() {
            if (!doUpdate) return;

            UpdateTimers();
            UpdateFromInput();

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

            base.Update();

            MatchFallingBlockVelocity();
        }

        protected override float GetDirectionalSpeedModifier() {
            float directionalSpeedModifier = 1;
            if (_velocity.Y > 0) {
                directionalSpeedModifier = 1.25f; // fall faster
            } else if (_velocity.Y < 0 && !OnLeftWall && !OnRightWall) {
                directionalSpeedModifier = .85f; // rise faster
            }
            return directionalSpeedModifier;
        }

        private void UpdateFromInput() {
            float deltaTime = Time.DeltaTime;

            if (KeyPressed(CharacterInput.Jump) && (Grounded || wasOnGround)) {
                _velocity.Y = -JumpSpeed;
            } else if (KeyReleased(CharacterInput.Jump) && _velocity.Y < 0) {
                _velocity.Y /= 2;
            }

            float moveSpeed = Grounded ? GroundMoveSpeed : AirMoveSpeed;
            float delta = moveSpeed * deltaTime + 100;

            if (KeyDown(CharacterInput.Left) == KeyDown(CharacterInput.Right)) {
                _velocity.X = ConvergeToZero(_velocity.X, deltaTime, moveSpeed);
            } else if (KeyDown(CharacterInput.Left) && timeSinceLeftWallJump <= 0) {
                if (OnLeftWall) {
                    _velocity.X = 0;
                } else {
                    _velocity.X = MathF.Max(-moveSpeed, _velocity.X - delta);
                }
            } else if (KeyDown(CharacterInput.Right) && timeSinceRightWallJump <= 0) {
                if (OnRightWall) {
                    _velocity.X = 0;
                } else {
                    _velocity.X = MathF.Min(moveSpeed, _velocity.X + delta);
                }
            }

            CheckForWallInteraction();
        }

        private void CheckForWallInteraction() {
            bool isSliding = State == CharacterState.Sliding && 
                !Flags.IsFlagSet(
                    _collisionData.Collider?.PhysicsLayer ?? _previousFrameCollisionData.Collider.PhysicsLayer, 
                    (int)PhysicsLayers.Static
                );

            bool isOnLeftWall = KeyDown(CharacterInput.Left) && isSliding;
            bool isOnRightWall = KeyDown(CharacterInput.Right) && isSliding;
            bool isWallJumping = KeyPressed(CharacterInput.Jump) && isSliding;
            bool isSlidingDown = isOnLeftWall || isOnRightWall;

            if (isWallJumping) {
                float jumpForceMultiplier = 1.25f;
                if (isOnLeftWall) {
                    _velocity.X = AirMoveSpeed * jumpForceMultiplier;
                    timeSinceLeftWallJump = wallJumpGracePeriod;
                } else {
                    _velocity.X = -AirMoveSpeed * jumpForceMultiplier;
                    timeSinceRightWallJump = wallJumpGracePeriod;
                }
                _velocity.Y = -JumpSpeed * jumpForceMultiplier;
            } else {
                if (isSlidingDown) {
                    _velocity.Y = SlideSpeed;
                    overriddenGravityModifier = 0;
                } else {
                    overriddenGravityModifier = float.NaN;
                }
            }
        }

        private void MatchFallingBlockVelocity() {
            if (Grounded) {
                FallingBlock block = _collisionData.Collider?.GetComponent<FallingBlock>();
                if (block != null) {
                    _velocity.Y = block.Velocity.Y;
                }
            }
        }

        private float ConvergeToZero(float value, float deltaTime, float moveSpeed, float rate = 7.5f) {
            float delta = moveSpeed * deltaTime * rate;
            if (value < 0) {
                value = MathF.Min(0, value + delta);
            } else if (value > 0) {
                value = MathF.Max(0, value - delta);
            }
            return value;
        }

        private void UpdateTimers() {
            timeSinceLeftWallJump -= Time.DeltaTime;
            timeSinceRightWallJump -= Time.DeltaTime;
        }

        private bool KeyReleased(CharacterInput input) {
            foreach (Keys key in inputKeyMap[input]) {
                if (Input.IsKeyReleased(key)) return true;
            }
            return false;
        }

        private bool KeyPressed(CharacterInput input) {
            foreach (Keys key in inputKeyMap[input]) {
                if (Input.IsKeyPressed(key)) return true;
            }
            return false;
        }

        private bool KeyDown(CharacterInput input) {
            foreach (Keys key in inputKeyMap[input]) {
                if (Input.IsKeyDown(key)) return true;
            }
            return false;
        }
    }

    public enum CharacterState {
        Idle,
        Moving,
        Jumping,
        Sliding,
    }
}
