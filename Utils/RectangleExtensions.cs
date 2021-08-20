using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Utils {
    public static class RectangleExtensions {
        public static bool Intersects(this Rectangle rect, Rectangle other, out Vector2 overlap) {
            overlap = Vector2.Zero;

            int rectL = rect.Left;
            int rectT = rect.Top;
            int rectR = rect.Right;
            int rectB = rect.Bottom;

            int otherL = other.Left;
            int otherT = other.Top;
            int otherR = other.Right;
            int otherB = other.Bottom;

            // Returns true if the borders are touching
            if (rectL == otherR || rectR == otherL || rectB == otherT || rectT == otherB) {
                overlap = new Vector2(
                    rectL == otherR ? -1 : rectR == otherL ? 1 : 0,
                    rectT == otherB ? -1 : rectB == otherT ? 1 : 0
                );
                return true;
            }

            if (!rect.Intersects(other)) return false;

            int signX = Math.Sign(other.Center.X - rect.Center.X);
            int signY = Math.Sign(other.Center.Y - rect.Center.Y);

            int rectHalfX = rect.Width / 2;
            int rectHalfY = rect.Height / 2;

            int otherHalfX = other.Width / 2;
            int otherHalfY = other.Height / 2;

            overlap = new Vector2(
                signX * (rectHalfX + otherHalfX - Math.Abs(rect.Center.X - other.Center.X)),
                signY * (rectHalfY + otherHalfY - Math.Abs(rect.Center.Y - other.Center.Y))
            );

            return true;
        }
    }
}
