using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MarshmallowAvalanche.Physics;

namespace MarshmallowAvalanche {
    public class Character : MovingObject {
        public enum Input {
            Left = 0,
            Right = 1,
            Jump = 2,
        }

        private readonly bool[] frameInput;
        private readonly bool[] prevFrameInput;
        private float overriddenGravityModifier;

        // The time before the input kicks in again to
        // steer back to wall
        private readonly float wallJumpGracePeriod = .1f;
        private float timeSinceRightWallJump = 0;
        private float timeSinceLeftWallJump = 0;

        public float JumpSpeed { get; set; }
        public float GroundMoveSpeed { get; set; }
        public float AirMoveSpeed { get; set; }
        public float SlideSpeed { get; set; }

        public override string Tag {
            get;
            protected set;
        }

        public Character(Vector2 position, Vector2 size, string tag = "Player") : base(position, size) {
            JumpSpeed = 800;
            GroundMoveSpeed = 550;
            AirMoveSpeed = GroundMoveSpeed * .8f;
            SlideSpeed = GroundMoveSpeed / 4;
            Tag = tag;

            frameInput = new bool[Enum.GetValues(typeof(Input)).Length];
            prevFrameInput = new bool[frameInput.Length];

            overriddenGravityModifier = float.NaN;
        }

        public CharacterState State {
            get {
                if (Velocity.Y != 0 && !Grounded) {
                    if (OnLeftWall || OnRightWall) {
                        return CharacterState.Sliding;
                    }
                    return CharacterState.Jumping;
                }
                if (Velocity.X != 0) {
                    return CharacterState.Moving;
                }
                return CharacterState.Idle;
            }
        }

        public override float GetGravityModifier() {
            if (!float.IsNaN(overriddenGravityModifier)) {
                return overriddenGravityModifier;
            }
            return base.GetGravityModifier();
        }

        public void UpdateKeyboardState(KeyboardState keyboard) {
            frameInput[(int)Input.Left] = keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A);
            frameInput[(int)Input.Right] = keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D);
            frameInput[(int)Input.Jump] = keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up);
        }

        public override void Update(GameTime gt) {
            UpdateTimers(gt);
            UpdateFromInputs(gt);
            base.Update(gt);
            UpdatePreviousInputs();
        }

        private void UpdateFromInputs(GameTime gt) {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            if (KeyPressed(Input.Jump) && Grounded) {
                _velocity.Y = -JumpSpeed;
            } else if (KeyReleased(Input.Jump) && _velocity.Y < 0) {
                _velocity.Y /= 2;
            }

            float moveSpeed = Grounded ? GroundMoveSpeed : AirMoveSpeed;
            float delta = moveSpeed * deltaTime * 100;

            if (KeyState(Input.Left) == KeyState(Input.Right)) {
                _velocity.X = ConvergeToZero(_velocity.X, deltaTime, moveSpeed);
            } else if (KeyState(Input.Left) && timeSinceLeftWallJump <= 0) {
                if (OnLeftWall) {
                    _velocity.X = 0;
                } else {
                    _velocity.X = MathF.Max(-moveSpeed, _velocity.X - delta);
                }
            } else if (KeyState(Input.Right) && timeSinceRightWallJump <= 0) {
                if (OnRightWall) {
                    _velocity.X = 0;
                } else {
                    _velocity.X = MathF.Min(moveSpeed, _velocity.X + delta);
                }
            }

            CheckForWallInteraction();
        }

        private void CheckForWallInteraction() {
            bool isOnLeftWall = KeyState(Input.Left) && OnLeftWall;
            bool isOnRightWall = KeyState(Input.Right) && OnRightWall;
            bool isWallJumping = KeyPressed(Input.Jump) && (isOnLeftWall || isOnRightWall) && !Grounded;
            bool isSlidingDown = (isOnLeftWall || isOnRightWall) && _velocity.Y > 0;

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

        private float ConvergeToZero(float value, float deltaTime, float moveSpeed, float rate = 7.5f) {
            float delta = moveSpeed * deltaTime * rate;
            if (value < 0) {
                value = MathF.Min(0, value + delta);
            } else if (value > 0) {
                value = MathF.Max(0, value - delta);
            }
            return value;
        }

        private void UpdateTimers(GameTime gt) {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
            timeSinceLeftWallJump -= deltaTime;
            timeSinceRightWallJump -= deltaTime;
        }

        private void UpdatePreviousInputs() {
            for (int i = 0; i < frameInput.Length; ++i) {
                prevFrameInput[i] = frameInput[i];
                frameInput[i] = false;
            }
        }
        private bool KeyReleased(Input input) {
            int idx = (int)input;
            return !frameInput[idx] && prevFrameInput[idx];
        }

        private bool KeyPressed(Input input) {
            int idx = (int)input;
            return frameInput[idx] && !prevFrameInput[idx];
        }

        private bool KeyState(Input input) {
            int idx = (int)input;
            return frameInput[idx];
        }
    }

    public enum CharacterState {
        Idle,
        Moving,
        Jumping,
        Sliding,
    }
}
