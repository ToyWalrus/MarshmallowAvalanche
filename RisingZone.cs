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
            //BoxCollider characterCollider = character.Collider;
            //if (characterCollider.CollidesWith(Collider, out CollisionResult result)) {
            //    PrototypeSpriteRenderer characterRenderer = character.GetComponent<PrototypeSpriteRenderer>();

            //    float overlapAmount = 0;
            //    float newHeight = characterCollider.Height - MathF.Max(overlapAmount, 0);

            //    characterCollider.SetHeight(newHeight);
            //    characterRenderer.SetHeight(newHeight);
            //}
        }

        private void UpdateRenderer() {
            // not sure why the +25 is needed but otherwise the renderer is, well, offset
            renderer.SetLocalOffset(new Vector2(-Collider.Width / 2, -Collider.Height / 2 + 25));
            renderer.SetHeight(Collider.Height);
            renderer.SetWidth(Collider.Width);
        }
    }
}
