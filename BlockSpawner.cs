using System.Collections.Generic;
using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class BlockSpawner : Component {
        public Vector2 Size { get; private set; }
        private float minSpawnSize;
        private float maxSpawnSize;
        private readonly HashSet<FallingBlock> spawnedBlocks;

        public BlockSpawner(Vector2 boundsSize, float minSpawnSize, float maxSpawnSize) {
            Size = boundsSize;
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
            spawnedBlocks = new HashSet<FallingBlock>();
        }

        #region Getters/Setters
        public void SetSpawnBoundsSize(Vector2 size) {
            Size = size;
        }

        public void SetMinSpawnSize(float size) {
            minSpawnSize = size;
        }

        public void SetMaxSpawnSize(float size) {
            maxSpawnSize = size;
        }
        #endregion

        /// <summary>
        /// Creates an entity in the scene and attaches a FallingBlock
        /// component to it and sets the fall speed.
        /// </summary>
        /// <returns>A new FallingBlock, or null if a block could not be spawned</returns>
        public FallingBlock SpawnBlock(float fallSpeed) {
            FallingBlock block = SpawnBlock();
            if (block == null) return null;

            block.MaxFallSpeed = fallSpeed;
            return block;
        }

        /// <summary>
        /// Creates an entity in the scene and attaches a FallingBlock
        /// component to it.
        /// </summary>
        /// <returns>A new FallingBlock, or null if a block could not be spawned</returns>
        public FallingBlock SpawnBlock() {
            System.Random rand = new System.Random();
            Vector2 spawnerPosition = Entity.Position;

            float size = (float)rand.NextDouble() * (maxSpawnSize - minSpawnSize) + minSpawnSize;
            float centerX = (float)rand.NextDouble() * Size.X + spawnerPosition.X;
            float centerY = (float)rand.NextDouble() * Size.Y + spawnerPosition.Y;
            Vector2 blockPosition = new Vector2(centerX - size / 2, centerY - size / 2);
            Vector2 blockSize = new Vector2(size, size);

            if (!CanSpawnBlockAt(blockPosition, blockSize)) {
                return null;
            }

            FallingBlock block = Entity.Scene
                .CreateEntity("falling-block-" + spawnedBlocks.Count)
                .AddComponent(new FallingBlock(blockSize));

            block.Transform.SetPosition(blockPosition);
            spawnedBlocks.Add(block);

            return block;
        }

        private bool CanSpawnBlockAt(Vector2 position, Vector2 blockSize) {
            foreach (FallingBlock block in spawnedBlocks) {
                if (block.Bounds.Intersects(new RectangleF(position, blockSize))) return false;
            }
            return true;
        }
    }
}
