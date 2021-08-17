using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarshmallowAvalanche.Utils {
    public class Logger {


        public static void LogToConsole(string msg) {
            Debug.WriteLine(msg);
        }

        public static void LogToConsole(object obj) {
            Debug.WriteLine(obj);
        }


        #region Visual Debugging/Logging
        private static SpriteFont font;
        private static Texture2D texture;

        private static Texture2D GetTexture(SpriteBatch sb) {
            if (texture == null) {
                texture = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData(new[] { Color.White });
            }

            return texture;
        }

        public static void DisposeTexture() {
            if (texture != null) {
                texture.Dispose();
            }
        }

        public static void DrawLine(SpriteBatch sb, Vector2 point1, Vector2 point2, Color color, float thickness = 1f) {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(sb, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch sb, Vector2 point, float length, float angle, Color color, float thickness = 1f) {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            sb.Draw(GetTexture(sb), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void InitFont(SpriteFont font) {
            Logger.font = font;
        }

        public static void DrawText(SpriteBatch sb, string text, Vector2 position) {
            DrawText(sb, text, position, Color.White);
        }

        public static void DrawText(SpriteBatch sb, string text, Vector2 position, Color color) {
            DrawText(sb, text, position, color, Vector2.One);
        }

        public static void DrawText(SpriteBatch sb, string text, Vector2 position, Color color, Vector2 scale, bool centerText = true) {
            Vector2 stringSize = centerText ? font.MeasureString(text) * scale : Vector2.Zero;
            sb.DrawString(font, text, position - (stringSize / 2), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public static void DrawOutlineRect(SpriteBatch sb, Rectangle rect, Color color, float thickness = 1) {
            var topLeft = new Vector2(rect.X, rect.Y);
            var topRight = new Vector2(rect.X + rect.Width, rect.Y);
            var botLeft = new Vector2(rect.X, rect.Y + rect.Height);
            var botRight = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);

            DrawLine(sb, topLeft, topRight, color, thickness);
            DrawLine(sb, topRight, botRight, color, thickness);
            DrawLine(sb, botRight, botLeft, color, thickness);
            DrawLine(sb, botLeft, topLeft, color, thickness);
        }

        public static void DrawFilledRect(SpriteBatch sb, Rectangle rect, Color color) {
            sb.Draw(GetTexture(sb), rect, color);
        }
        #endregion
    }
}
