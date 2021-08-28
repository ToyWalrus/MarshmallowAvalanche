using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace MarshmallowAvalanche.Physics {
    public class FallingBlock : MovingObject {
        private SpriteRenderer sr;

        public override float MaxFallSpeed {
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
            Collider.PhysicsLayer = (int)PhysicsLayers.Block;
            Collider.CollidesWithLayers = (int)(PhysicsLayers.Block | PhysicsLayers.Static);

            sr = Entity.GetComponent<SpriteRenderer>();
            if (sr == null) {
                sr = Entity.AddComponent<SpriteRenderer>();
            }
            sr.Sprite = new Sprite(Entity.Scene.Content.LoadTexture("FallingBlock"));
            //sr.Sprite.
        }

        public void SetBlockColor(Color color) {
            if (sr == null) {
                Debug.Error("Cannot set color of block if it has not yet been added to entity");
                return;
            }
            sr.SetColor(color);
        }

        public override void Update() {
            // Blocks will never move once grounded
            if (Grounded) return;
            base.Update();
        }

        public override void ResetCollisionStatus() { }
    }
}
