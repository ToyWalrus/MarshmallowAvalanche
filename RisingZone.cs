using System;
using System.Collections.Generic;
using Nez;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class RisingZone : Component, IUpdatable {
        private float riseRate = 5f;
        private Collider characterCollider;
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
            Collider.PhysicsLayer = (int)PhysicsLayers.Liquid;
            Collider.CollidesWithLayers = (int)PhysicsLayers.None;

            renderer = Entity.AddComponent<PrototypeSpriteRenderer>();
            renderer.RenderLayer = int.MinValue; // render over everything else

            if (zoneColor != null) {
                renderer.Color = zoneColor;
            }
        }

        public void SetCharacter(Character character) {
            characterCollider = character.GetComponent<Collider>();
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
            Collider.SetHeight(Collider.Height + amountChange);

            UpdateRenderer();
            CheckForCharacterOverlap();
        }

        private void CheckForCharacterOverlap() {
            if (characterCollider == null) return;
            if (Collider.CollidesWith(characterCollider, out CollisionResult result)) {
                //Debug.Log("Rising zone overtook character!");
            }
        }

        private void UpdateRenderer() {
            renderer.SetHeight(Collider.Height);
            renderer.SetWidth(Collider.Width);
        }
    }
}
