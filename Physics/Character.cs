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

        public bool IsDead => Collider.Height <= .25f;

        private float overriddenGravityModifier;
        private readonly Dictionary<CharacterInput, ICollection<Keys>> inputKeyMap;

        // The time before the input kicks in again to
        // steer back to wall
        private readonly float wallJumpGracePeriod = .125f;
        private float timeSinceRightWallJump = 0;
        private float timeSinceLeftWallJump = 0;

        public float JumpSpeed { get; set; }
        public float GroundMoveSpeed { get; set; }
        public float AirMoveSpeed { get; set; }
        public float SlideSpeed { get; set; }

        public bool IsBeingDissolved { get; set; }
        public bool IsBeingCrushed { get; set; }

        private MarshmallowRenderer renderer;

        public Character(Vector2 size) : base(size) {
            JumpSpeed = 600;
            GroundMoveSpeed = 550;
            AirMoveSpeed = GroundMoveSpeed * .8f;
            SlideSpeed = GroundMoveSpeed / 4;
            IsBeingDissolved = false;
            IsBeingCrushed = false;

            overriddenGravityModifier = float.NaN;

            inputKeyMap = new Dictionary<CharacterInput, ICollection<Keys>> {
                { CharacterInput.Left, new HashSet<Keys> { Keys.Left, /*Keys.A*/ }},
                { CharacterInput.Right, new HashSet<Keys> { Keys.Right, /*Keys.D*/ }},
                { CharacterInput.Jump, new HashSet<Keys> { Keys.Up, /*Keys.Space*/ }}
            };
        }

        public CharacterState State
        {
            get {
                bool againstWall = OnLeftWall || wasOnLeftWall || OnRightWall || wasOnRightWall;
                if (!Grounded && !wasOnGround) {
                    if (Velocity.Y > 0 && againstWall) {
                        return CharacterState.Sliding;
                    } else if (Velocity.Y != 0) {
                        return CharacterState.Jumping;
                    }
                }
                if (Velocity.X != 0 && !againstWall) {
                    return CharacterState.Moving;
                }
                return CharacterState.Idle;
            }
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            Collider.PhysicsLayer = (int) PhysicsLayers.Marshmallow;
            Collider.CollidesWithLayers = (int) (PhysicsLayers.Block | PhysicsLayers.Static);

            renderer = Entity.AddComponent<MarshmallowRenderer>();
        }

        public override float GetGravityModifier() {
            if (!float.IsNaN(overriddenGravityModifier)) {
                return overriddenGravityModifier;
            }
            return base.GetGravityModifier();
        }

        public void GetCrushed(float crushAmount, float blockBottomPosition) {
            float newHeight = Bounds.Height - crushAmount;

            Collider.SetHeight(newHeight);
            renderer.SetHeight(newHeight);
            renderer.SetOriginNormalized(new Vector2(.5f, .5f));
            Entity.Position = new Vector2(Entity.Position.X, blockBottomPosition + newHeight / 2);
            IsBeingCrushed = true;
        }

        public void GetDissolved(float disolveAmount, float liquidPosition) {
            float newHeight = Bounds.Height - disolveAmount;

            Collider.SetHeight(newHeight);
            renderer.SetHeight(newHeight);
            renderer.SetOriginNormalized(new Vector2(.5f, .5f));
            Entity.Position = new Vector2(Entity.Position.X, liquidPosition - newHeight / 2);
            IsBeingDissolved = true;
        }

        public override void Update() {
            if (IsDead) {
                SetEnabled(false);
                return;
            }

            float offsetGravity = -gravityModifier * GravityConst * GetDirectionalSpeedModifier();
            if (IsBeingDissolved && Velocity.Y > offsetGravity) {
                _velocity.Y = offsetGravity;
            }

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

            renderer.UpdateCharacterState(State, Math.Sign(_velocity.X), IsBeingCrushed);

            IsBeingDissolved = false;
            IsBeingCrushed = false;
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

            if (KeyPressed(CharacterInput.Jump)) {
                if (Grounded || wasOnGround) {
                    _velocity.Y = -JumpSpeed;
                } else if (IsBeingDissolved) {
                    // Allow for tiny hops in liquid
                    _velocity.Y = -JumpSpeed / 1.5f;
                    IsBeingDissolved = false;
                }
            } else if (KeyReleased(CharacterInput.Jump) && _velocity.Y < 0) {
                _velocity.Y /= 2;
            }

            float moveSpeed = Grounded ? GroundMoveSpeed : AirMoveSpeed;
            if (IsBeingDissolved) {
                moveSpeed -= moveSpeed / 1.5f;
            }

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
            // Don't allow sliding or wall jumping from static objects
            bool isSliding = State == CharacterState.Sliding &&
                !Flags.IsFlagSet(
                    _collisionData.Collider?.PhysicsLayer ?? _previousFrameCollisionData.Collider.PhysicsLayer,
                    (int) PhysicsLayers.Static
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
                if (Input.IsKeyReleased(key))
                    return true;
            }
            return false;
        }

        private bool KeyPressed(CharacterInput input) {
            foreach (Keys key in inputKeyMap[input]) {
                if (Input.IsKeyPressed(key))
                    return true;
            }
            return false;
        }

        private bool KeyDown(CharacterInput input) {
            foreach (Keys key in inputKeyMap[input]) {
                if (Input.IsKeyDown(key))
                    return true;
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
