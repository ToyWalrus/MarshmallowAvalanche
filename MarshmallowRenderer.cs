using System;
using System.Collections.Generic;
using Nez.Sprites;
using Nez;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class MarshmallowRenderer : SpriteRenderer {
        new public const int RenderLayer = 1;

        private const string defaultSprite = "default";
        private const string wallJumpSprite = "wallJump";
        private const string crushedSprite = "crushed";
        private Dictionary<string, Texture2D> sprites;

        private float _height;
        private float _width;
        private int facingDirection;
        private CharacterState characterState;

        private float skewTopX;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            sprites = new Dictionary<string, Texture2D>
            {
                { defaultSprite, Entity.Scene.Content.LoadTexture("marshmallow/standing") },
                { wallJumpSprite, Entity.Scene.Content.LoadTexture("marshmallow/wall") },
                { crushedSprite, Entity.Scene.Content.LoadTexture("marshmallow/crushed") }
            };

            Sprite = new Sprite(sprites[defaultSprite]);
            base.RenderLayer = RenderLayer;

            _height = Sprite.Texture2D.Height;
            _width = Sprite.Texture2D.Width;

            facingDirection = 1;

            ResetSkew();
        }

        public override float Height => _height;
        public override float Width => _width;

        public void SetHeight(float height) {
            _height = height;
        }

        public void SetWidth(float width) {
            _width = width;
        }

        public void UpdateCharacterState(CharacterState state, int facingDirection, bool isBeingCrushed) {
            if (sprites == null || (characterState == state && this.facingDirection == facingDirection))
                return;

            characterState = state;
            this.facingDirection = facingDirection;

            if (isBeingCrushed) {
                Sprite.Texture2D = sprites[crushedSprite];
                ResetSkew();
            } else {
                switch (state) {
                    case CharacterState.Idle:
                        Sprite.Texture2D = sprites[defaultSprite];
                        ResetSkew();
                        break;
                    case CharacterState.Moving:
                    case CharacterState.Jumping:
                        Sprite.Texture2D = sprites[defaultSprite];
                        SetSkewFromFacingDirection();
                        break;
                    case CharacterState.Sliding:
                        Sprite.Texture2D = sprites[wallJumpSprite];
                        ResetSkew();
                        break;
                }
            }
        }

        private void SetSkewFromFacingDirection() {
            if (facingDirection != 0) {
                skewTopX = 4f;
            } else {
                skewTopX = 0;
            }

            if (facingDirection == -1) {
                FlipX = true;
            } else if (facingDirection == 1) {
                FlipX = false;
            }
        }

        private void ResetSkew() {
            skewTopX = 0;
        }

        public override void Render(Batcher batcher, Camera camera) {
            Vector2 pos = Entity.Transform.Position - (Origin * Entity.Transform.Scale) + LocalOffset;
            Vector2 size = new Vector2(_width * Entity.Transform.Scale.X, _height * Entity.Transform.Scale.Y);
            Rectangle destRect = new Rectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y);

            batcher.Draw(_sprite, destRect, _sprite.SourceRect, Color, Entity.Transform.Rotation,
                SpriteEffects, LayerDepth, skewTopX, 0, 0, 0);
        }
    }
}
