using System;
using System.Collections.Generic;
using Nez;
using Nez.Sprites;
using Microsoft.Xna.Framework;
using MarshmallowAvalanche.Physics;

namespace MarshmallowAvalanche {
    public class RisingZone : Component, IUpdatable {
        public const int RenderLayer = -100;

        private readonly Color zoneColor = new Color(174, 23, 23);
        private float riseRate = 5f;
        private Character character;
        private PrototypeSpriteRenderer renderer;
        private SpriteAnimator waveAnimator;
        private HashSet<FallingBlock> spawnedBlocks;

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
            Collider.PhysicsLayer = (int) PhysicsLayers.Liquid;

            renderer = Entity.AddComponent<PrototypeSpriteRenderer>();
            renderer.Color = zoneColor;
            renderer.RenderLayer = RenderLayer + 1;
            renderer.SetEnabled(false);

            var waveAtlas = Entity.Scene.Content.LoadSpriteAtlas("Content/wave/wave.atlas", true);
            waveAnimator = Entity.Scene.CreateEntity("wave").AddComponent<SpriteAnimator>();

            waveAnimator.Transform.SetParent(Transform);
            waveAnimator.Transform.LocalScale = new Vector2(1.25f, 1);
            waveAnimator.AddAnimationsFromAtlas(waveAtlas);
            waveAnimator.RenderLayer = RenderLayer;
            waveAnimator.SetEnabled(false);

            spawnedBlocks = new HashSet<FallingBlock>();
        }

        public void SetCharacter(Character character) {
            this.character = character;
        }

        public void BeginRising() {
            if (Collider == null) {
                Debug.Error("Cannot begin rising, this component has not been added to an entity");
                return;
            }
            IsRising = true;
            renderer.SetEnabled(true);
            waveAnimator.SetEnabled(true);
            waveAnimator.Play("wave");
        }

        public void StopRising() {
            IsRising = false;
        }

        public void SetRiseRate(float rate) {
            riseRate = rate;
        }

        public void IncreaseRiseRate(float amount, float maxRate = 100f) {
            riseRate = Math.Min(riseRate + amount, maxRate);
        }

        public void RegisterSpawnedBlock(FallingBlock block) {
            spawnedBlocks.Add(block);
        }

        public void Update() {
            if (!IsRising)
                return;

            float amountChange = riseRate * Time.DeltaTime;

            Entity.Position -= new Vector2(0, amountChange);
            Collider.SetHeight(Collider.Height + (amountChange * 2));

            UpdateRenderer();
            CheckForCharacterOverlap();
            CheckForBlockOverlap();
        }

        private void CheckForCharacterOverlap() {
            if (character == null)
                return;

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

        private void CheckForBlockOverlap() {
            float cutoffPosition = Collider.Bounds.Top;
            HashSet<FallingBlock> toRemove = new HashSet<FallingBlock>();
            foreach (FallingBlock block in spawnedBlocks) {
                if (!block.Entity.IsDestroyed && block.HasComponent<Collider>() && block.Bounds.Top > cutoffPosition) {
                    toRemove.Add(block);
                    block.Entity.Destroy();
                }
            }
            spawnedBlocks.RemoveWhere((block) => toRemove.Contains(block));
        }

        private void UpdateRenderer() {
            renderer.SetOriginNormalized(new Vector2(.5f, .5f));
            renderer.SetHeight(Collider.Height);
            renderer.SetWidth(Collider.Width);

            waveAnimator.SetLocalOffset(new Vector2(0, -Collider.Bounds.Height / 2 - 10));
        }
    }
}
