using System;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace MarshmallowAvalanche.Physics {
    public class FallingBlock : MovingObject {
        public const int RenderLayer = 10;
        private SpriteRenderer sr;
        private Color savedColor;

        public override float MaxFallSpeed
        {
            get => base.MaxFallSpeed;
            set {
                _velocity.Y = value;
                base.MaxFallSpeed = value;
            }
        }

        public FallingBlock(Vector2 size) : base(size) {
            MaxFallSpeed = 500;
            Grounded = false;
            Velocity = new Vector2(0, MaxFallSpeed);
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            Collider.PhysicsLayer = (int) PhysicsLayers.Block;
            Collider.CollidesWithLayers = (int) (PhysicsLayers.Block | PhysicsLayers.Marshmallow | PhysicsLayers.Static);

            sr = Entity.GetComponent<SpriteRenderer>();
            if (sr == null) {
                sr = Entity.AddComponent<SpriteRenderer>();
            }
            sr.Sprite = new Sprite(Entity.Scene.Content.LoadTexture("blocks/FallingBlock"));
            sr.RenderLayer = RenderLayer;

            Rectangle spriteBounds = sr.Sprite.Texture2D.Bounds;
            float scale = Bounds.Width / spriteBounds.Width;

            Collider.SetSize(spriteBounds.Width, spriteBounds.Height);
            sr.Transform.SetScale(scale);

            if (savedColor != null) {
                sr.Color = savedColor;
            }
        }

        public void SetBlockColor(Color color) {
            savedColor = color;
            if (sr != null) {
                sr.SetColor(color);
            }
        }

        public override void Update() {
            // Blocks will never move once grounded
            if (Grounded) {
                return;
            }
            base.Update();
        }

        public override void ResetCollisionStatus() { }

        public override void SetTouchingBorder(PhysicsObject other) {
            if (other == null) {
                return;
            }
            Vector2 overlap = _collisionData.MinimumTranslationVector;

            if (overlap.Y > 0) {
                if (other is StaticObject) {
                    Grounded = true;
                    _velocity.Y = 0;
                } else if (other is FallingBlock otherBlock) {
                    // We only want to set this block to grounded if
                    // it is on a static object or another block that
                    // is also grounded

                    Grounded = otherBlock.Grounded;
                    if (Grounded && _velocity.Y > 0) {
                        _velocity.Y = 0;
                    }
                } else if (other is Character marshmallow && marshmallow.Grounded) {
                    // Squash that pesky marshmallow!
                    float overlapAmount = MathF.Abs(overlap.Y);
                    marshmallow.GetCrushed(overlapAmount, Collider.Bounds.Bottom);

                    if (marshmallow.IsDead) {
                        _velocity.Y = 0;
                        Grounded = true;
                    }
                }
            }

        }
    }
}
