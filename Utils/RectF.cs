using System;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Utils {
    public struct RectF : IEquatable<RectF> {
        public float X;
        public float Y;

        public float Width;
        public float Height;

        public RectF(Rectangle rectangle) {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public RectF(Vector2 position, Vector2 size) {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public RectF(float x, float y, float width, float height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static RectF Empty => new RectF(0, 0, 0, 0);

        public Vector2 Position {
            get => new Vector2(X, Y);
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        public bool IsEmpty => Width == 0 && Height == 0 && X == 0 && Y == 0;
        public float Bottom => Y + Height;
        public float Top => Y;
        public float Right => X + Width;
        public float Left => X;

        public Vector2 TopLeft => new Vector2(Left, Top);
        public Vector2 TopRight => new Vector2(Right, Top);
        public Vector2 BotRight => new Vector2(Right, Bottom);
        public Vector2 BotLeft => new Vector2(Left, Bottom);

        public Vector2 Size {
            get => new Vector2(Width, Height);
            set {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Vector2 Center => new Vector2((Left + Right) / 2f, (Top + Bottom) / 2f);

        //
        // Summary:
        //     Creates a new Rect that contains overlapping region
        //     of two other Rects.
        //
        // Parameters:
        //   value1:
        //     The first Rect.
        //
        //   value2:
        //     The second Rect.
        //
        // Returns:
        //     Overlapping region of the two Rects.
        //public static Rect Intersect(Rect value1, Rect value2) { }

        //public static void Intersect(ref Rect value1, ref Rect value2, out Rect result);

        public bool Intersects(RectF other, out Vector2 overlap) {
            overlap = Vector2.Zero;

            if (!Intersects(other)) return false;

            int signX = Math.Sign(other.Center.X - Center.X);
            int signY = Math.Sign(other.Center.Y - Center.Y);

            float rectHalfX = Width / 2f;
            float rectHalfY = Height / 2f;

            float otherHalfX = other.Width / 2f;
            float otherHalfY = other.Height / 2f;

            overlap = new Vector2(
                signX * (rectHalfX + otherHalfX - Math.Abs(Center.X - other.Center.X)),
                signY * (rectHalfY + otherHalfY - Math.Abs(Center.Y - other.Center.Y))
            );

            return true;
        }

        public bool Intersects(RectF other) {
            return !(
                    Left > other.Right ||
                    Right < other.Left ||
                    Top > other.Bottom ||
                    Bottom < other.Top
                );
        }

        public static bool Intersects(RectF a, RectF b) {
            return a.Intersects(b);
        }

        //
        // Summary:
        //     Creates a new Rect that completely contains two
        //     other Rects.
        //
        // Parameters:
        //   value1:
        //     The first Rect.
        //
        //   value2:
        //     The second Rect.
        //
        // Returns:
        //     The union of the two Rects.
        //public static Rect Union(Rect value1, Rect value2);
        //public static void Union(ref Rect value1, ref Rect value2, out Rect result);

        //
        // Summary:
        //     Gets whether or not the provided Rect lies within
        //     the bounds of this Rect.
        //
        // Parameters:
        //   value:
        //     The Rect to check for inclusion in this Rect.
        //
        //   result:
        //     true if the provided Rect's bounds lie entirely
        //     inside this Rect; false otherwise. As an output
        //     parameter.
        //public void Contains(ref Rect value, out bool result);

        //
        // Summary:
        //     Gets whether or not the provided coordinates lie within the bounds of this Rect.
        //
        // Parameters:
        //   x:
        //     The x coordinate of the point to check for containment.
        //
        //   y:
        //     The y coordinate of the point to check for containment.
        //
        // Returns:
        //     true if the provided coordinates lie inside this Rect;
        //     false otherwise.
        //public bool Contains(int x, int y);

        //
        // Summary:
        //     Gets whether or not the provided Vector2 lies within
        //     the bounds of this Rect.
        //
        // Parameters:
        //   value:
        //     The coordinates to check for inclusion in this Rect.
        //
        //   result:
        //     true if the provided Vector2 lies inside this Rect;
        //     false otherwise. As an output parameter.
        //public void Contains(ref Vector2 value, out bool result);

        //
        // Summary:
        //     Gets whether or not the provided coordinates lie within the bounds of this Rect.
        //
        // Parameters:
        //   x:
        //     The x coordinate of the point to check for containment.
        //
        //   y:
        //     The y coordinate of the point to check for containment.
        //
        // Returns:
        //     true if the provided coordinates lie inside this Rect;
        //     false otherwise.
        //public bool Contains(float x, float y);

        //
        // Summary:
        //     Gets whether or not the provided Point lies within the
        //     bounds of this Rect.
        //
        // Parameters:
        //   value:
        //     The coordinates to check for inclusion in this Rect.
        //
        // Returns:
        //     true if the provided Point lies inside this Rect;
        //     false otherwise.
        //public bool Contains(Point value);
        //public void Contains(ref Point value, out bool result);

        //
        // Summary:
        //     Gets whether or not the provided Vector2 lies within
        //     the bounds of this Rect.
        //
        // Parameters:
        //   value:
        //     The coordinates to check for inclusion in this Rect.
        //
        // Returns:
        //     true if the provided Vector2 lies inside this Rect;
        //     false otherwise.
        //public bool Contains(Vector2 value);

        //
        // Summary:
        //     Gets whether or not the provided Rect lies within
        //     the bounds of this Rect.
        //
        // Parameters:
        //   value:
        //     The Rect to check for inclusion in this Rect.
        //
        // Returns:
        //     true if the provided Rect's bounds lie entirely
        //     inside this Rect; false otherwise.
        //public bool Contains(Rect value);






        //
        // Summary:
        //     Adjusts the edges of this Rect by specified horizontal
        //     and vertical amounts.
        //
        // Parameters:
        //   horizontalAmount:
        //     Value to adjust the left and right edges.
        //
        //   verticalAmount:
        //     Value to adjust the top and bottom edges.
        //public void Inflate(float horizontalAmount, float verticalAmount);

        //
        // Summary:
        //     Adjusts the edges of this Rect by specified horizontal
        //     and vertical amounts.
        //
        // Parameters:
        //   horizontalAmount:
        //     Value to adjust the left and right edges.
        //
        //   verticalAmount:
        //     Value to adjust the top and bottom edges.
        //public void Inflate(int horizontalAmount, int verticalAmount);

        //
        // Summary:
        //     Gets whether or not the other Rect intersects with
        //     this Rect.
        //
        // Parameters:
        //   value:
        //     The other Rect for testing.
        //
        // Returns:
        //     true if other Rect intersects with this Rect;
        //     false otherwise.
        //public bool Intersects(Rect value);
        //public void Intersects(ref Rect value, out bool result);

        public RectF Offset(Point amount) {
            return Offset(new Vector2(amount.X, amount.Y));
        }

        public RectF Offset(float offsetX, float offsetY) {
            return Offset(new Vector2(offsetX, offsetY));
        }

        public RectF Offset(int offsetX, int offsetY) {
            return Offset(new Vector2(offsetX, offsetY));
        }

        public RectF Offset(Vector2 amount) {
            X += amount.X;
            Y += amount.Y;
            return this;
        }

        public override string ToString() {
            return $"{{ X: {X:F2}, Y: {Y:F2}, Width: {Width}, Height: {Height} }}";
        }

        public static bool operator ==(RectF a, RectF b) => a.Equals(b);
        public static bool operator !=(RectF a, RectF b) => !a.Equals(b);

        public static implicit operator RectF(Rectangle rectangle) {
            return new RectF(rectangle);
        }

        public static implicit operator Rectangle(RectF rect) {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

        public override bool Equals(object obj) {
            return obj is RectF rect && Equals(rect);
        }

        public bool Equals(RectF other) {
            return other.X == X &&
                other.Y == Y &&
                other.Width == Width &&
                other.Height == Height;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y, Width, Height);
        }
    }

}
