using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MarshmallowAvalanche {
    public class Character : MovingObject {
        public Character(Vector2 position, Vector2 size) : base(position, size) {
            JumpSpeed = 410;
            MoveSpeed = 160;
            SlideSpeed = MoveSpeed / 2;
            MinJumpSpeed = JumpSpeed / 2;

            inputs = new bool[Enum.GetValues(typeof(Input)).Length];
            previousInputs = new bool[inputs.Length];
        }


        public enum Input {
            Left = 0,
            Right = 1,
            Jump = 2,
        }

        public CharacterState State {
            get {
                if (Velocity.Y != 0 && !onGround) {
                    if (onLeftWall || onRightWall) {
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

        protected bool[] inputs;
        protected bool[] previousInputs; // from prev frame

        public float JumpSpeed { get; set; }
        public float MinJumpSpeed { get; set; }
        public float MoveSpeed { get; set; }
        public float SlideSpeed { get; set; }

        public void UpdateKeyboardState(KeyboardState keyboard) {
            inputs[(int)Input.Left] = keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A);
            inputs[(int)Input.Right] = keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D);
            inputs[(int)Input.Jump] = keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up);
        }

        public override void Update(GameTime gt) {
            UpdateFromInputs();
            base.Update(gt);
            UpdatePreviousInputs();
        }

        private void UpdatePreviousInputs() {
            for (int i = 0; i < inputs.Length; ++i) {
                previousInputs[i] = inputs[i];
                inputs[i] = false;
            }
        }

        private void UpdateFromInputs() {
            if (KeyState(Input.Jump) && onGround) {
                _velocity.Y = JumpSpeed;
            } else if (InputReleased(Input.Jump) && _velocity.Y > 0) {
                _velocity.Y = MathF.Min(_velocity.Y, MinJumpSpeed);
            }

            if (KeyState(Input.Left) == KeyState(Input.Right)) {
                _velocity.X = 0;
            } else if (KeyState(Input.Left)) {
                _velocity.X = onLeftWall ? 0 : -MoveSpeed;
            } else {
                _velocity.X = onRightWall ? 0 : MoveSpeed;
            }
        }

        private bool InputReleased(Input input) {
            int idx = (int)input;
            return !inputs[idx] && previousInputs[idx];
        }

        private bool InputPressed(Input input) {
            int idx = (int)input;
            return inputs[idx] && !previousInputs[idx];
        }

        private bool KeyState(Input input) {
            int idx = (int)input;
            return inputs[idx];
        }

    }

    public enum CharacterState {
        Idle,
        Moving,
        Jumping,
        Sliding,
    }
}
