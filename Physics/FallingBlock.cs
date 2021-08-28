using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class FallingBlock : MovingObject {
        public FallingBlock(Vector2 position, Vector2 size) : base(position, size) {
            MaxFallSpeed = 500;
            Grounded = false;
            Velocity = new Vector2(0, MaxFallSpeed);
        }

        public override void Update() {
            // Blocks will never move once grounded
            if (Grounded) return;
            base.Update();
        }

        public override bool CanCollideWith(PhysicsObject other) {
            // Blocks should not collide with character,
            // or in other words, a character should not
            // impede the block's progress.
            return !(other is Character);
        }

        // Once collision status has been set for block
        // it won't be altered
        public override void ResetCollisionStatus() { }

        //public override void CheckForCollisionWith(PhysicsObject other) {
            //if (other == null || Grounded || !CanCollideWith(other)) return;
            //var otherBounds = other.Bounds;

            //if (Bounds.Intersects(otherBounds, out Vector2 overlap)) {
            //    if (other is FallingBlock otherBlock) {
            //        bool xIsSmaller = Math.Abs(overlap.X) < Math.Abs(overlap.Y);
            //        if (xIsSmaller) {
            //            // Blocks will never be moving horizontally so 
            //            // this should only ever happen if a block spawns
            //            // in another block
            //            if (overlap.X > 0) {
            //                _position.X = MathF.Floor(otherBounds.Left - Size.X);
            //            } else if (overlap.X < 0) {
            //                _position.X = MathF.Floor(otherBounds.Right);
            //            }
            //        }
            //        if (overlap.Y > 0) {
            //            _position.Y = MathF.Floor(otherBounds.Top - Size.Y);

            //            // Ground the block if the other block is grounded
            //            if (otherBlock.Grounded) {
            //                Grounded = true;
            //            }
            //        }
            //    } else if (other is StaticObject) {
            //        if (overlap.Y > 0) {
            //            Grounded = true;
            //            _position.Y = MathF.Floor(otherBounds.Top - Size.Y);
            //            if (_velocity.Y > 0) {
            //                _velocity.Y = 0;
            //            }
            //        }
            //    }
            //}
        //}
    }
}
