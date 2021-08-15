using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MarshmallowAvalanche {
    public class Character : MovingObject {
        public enum Input {
            Left = 0,
            Right = 1,
            Jump = 2,
        }

        private bool[] frameInput;
        private bool[] prevFrameInput;
        private float overriddenGravityModifier;

        public float JumpSpeed { get; set; }
        public float MoveSpeed { get; set; }
        public float SlideSpeed { get; set; }
        

        public Character(Vector2 position, Vector2 size) : base(position, size) {
            JumpSpeed = 810;
            MoveSpeed = 300;
            SlideSpeed = MoveSpeed / 4;

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
            UpdateFromInputs();
            base.Update(gt);
            UpdatePreviousInputs();
        }

        private void UpdatePreviousInputs() {
            for (int i = 0; i < frameInput.Length; ++i) {
                prevFrameInput[i] = frameInput[i];
                frameInput[i] = false;
            }
        }

        private void UpdateFromInputs() {
            if (KeyPressed(Input.Jump) && Grounded) {
                _velocity.Y = -JumpSpeed;
            } else if (KeyReleased(Input.Jump) && _velocity.Y < 0) {
                _velocity.Y /= 2;
            }

            if (KeyState(Input.Left) == KeyState(Input.Right)) {
                _velocity.X = 0;
            } else if (KeyState(Input.Left)) {
                _velocity.X = OnLeftWall ? 0 : -MoveSpeed;
            } else {
                _velocity.X = OnRightWall ? 0 : MoveSpeed;
            }

            if (((KeyState(Input.Left) && OnLeftWall) || (KeyState(Input.Right) && OnRightWall)) && _velocity.Y > 0) {
                _velocity.Y = SlideSpeed;
                overriddenGravityModifier = 0;
            } else {
                overriddenGravityModifier = float.NaN;
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
            return frameInput[idx] || prevFrameInput[idx];
        }

    }

    public enum CharacterState {
        Idle,
        Moving,
        Jumping,
        Sliding,
    }
}
