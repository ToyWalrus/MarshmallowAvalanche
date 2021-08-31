using System;
using System.Collections.Generic;
using Nez;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class RisingZone : Component, IUpdatable {
        private float riseRate = 5f;
        private Character character;
        private PrototypeSpriteRenderer renderer;
        private Color zoneColor;

        public bool IsRising { get; private set; }
        public BoxCollider Collider { get; private set; }

        public override void OnAddedToEntity() {
            IsRising = false;

            Collider = Entity.GetComponent<BoxCollider>();
            if (Collider == null) {
                Collider = Entity.AddComponent<BoxCollider>();
            }

            Collider.SetHeight(0);
            Collider.IsTrigger = true;

            renderer = Entity.AddComponent<PrototypeSpriteRenderer>();

            if (zoneColor != null) {
                renderer.Color = zoneColor;
            }
        }

        public void SetCharacter(Character character) {
            this.character = character;
        }

        public void SetZoneColor(Color color) {
            zoneColor = new Color(color.R, color.G, color.B, (byte)150);
            if (renderer != null) {
                renderer.Color = zoneColor;
            }
        }

        public void BeginRising() {
            if (Collider == null) {
                Debug.Error("Cannot begin rising, this component has not been added to an entity");
                return;
            }
            IsRising = true;
        }

        public void StopRising() {
            IsRising = false;
        }

        public void SetRiseRate(float rate) {
            riseRate = rate;
        }

        public void Update() {
            if (!IsRising) return;

            float amountChange = riseRate * Time.DeltaTime;

            Entity.Position -= new Vector2(0, amountChange);
            Collider.SetHeight(Collider.Height + (amountChange * 2));

            UpdateRenderer();
            CheckForCharacterOverlap();
        }

        private void CheckForCharacterOverlap() {
            if (character == null) return;
            BoxCollider characterCollider = character.Collider;
            if (characterCollider.Height > 0 && Collider.CollidesWith(characterCollider, out _)) {
                PrototypeSpriteRenderer characterRenderer = character.GetComponent<PrototypeSpriteRenderer>();

                float overlapAmount = MathF.Abs(MathF.Max(characterCollider.Bounds.Bottom - Collider.Bounds.Top, 0));
                float newHeight = characterCollider.Height - overlapAmount;

                characterCollider.SetHeight(newHeight);
                characterRenderer.SetHeight(newHeight);
                characterRenderer.SetOriginNormalized(new Vector2(.5f, .5f));
                character.Entity.Position = new Vector2(character.Entity.Position.X, Collider.Bounds.Top - newHeight / 2);

                character.IsBeingDissolved = true;
            } else {
                character.IsBeingDissolved = false;
            }
        }

        private void UpdateRenderer() {
            renderer.SetOriginNormalized(new Vector2(.5f, .5f));
            renderer.SetHeight(Collider.Height);
            renderer.SetWidth(Collider.Width);
        }
    }
}
