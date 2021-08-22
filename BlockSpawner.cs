using System;
using MarshmallowAvalanche.Physics;
using MarshmallowAvalanche.Utils;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class BlockSpawner {
        private RectF _spawnBounds;
        public RectF SpawnBounds {
            get => _spawnBounds;
            private set {
                _spawnBounds = value;
            }
        }
        private Vector2 minSpawnSize;
        private Vector2 maxSpawnSize;

        public BlockSpawner(RectF spawnBounds, Vector2 minSpawnSize, Vector2 maxSpawnSize) {
            SpawnBounds = spawnBounds;
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
        }

        public BlockSpawner(Vector2 position, Vector2 size, Vector2 minSpawnSize, Vector2 maxSpawnSize) {
            SpawnBounds = new RectF(position, size);
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
        }

        #region Getters/Setters
        public void MoveSpawnLocation(Vector2 offset) {
            _spawnBounds.Position += offset;
        }

        public void SetSpawnLocation(Vector2 position) {
            _spawnBounds.Position = position;
        }

        public void SetSpawnBoundsSize(Vector2 size) {
            _spawnBounds.Size = size;
        }

        public void SetSpawnBounds(RectF newBounds) {
            SpawnBounds = newBounds;
        }

        public void SetMinSpawnSize(Vector2 size) {
            minSpawnSize = size;
        }

        public void SetMaxSpawnSize(Vector2 size) {
            maxSpawnSize = size;
        }
        #endregion

        public FallingBlock SpawnBlock(bool keepSquare = false, string tag = FallingBlock.DefaultTag) {
            Random rand = new Random();
            float blockWidth = (float)rand.NextDouble() * (maxSpawnSize.X - minSpawnSize.X) + minSpawnSize.X;
            float blockHeight = (float)rand.NextDouble() * (maxSpawnSize.Y - minSpawnSize.Y) + minSpawnSize.Y;

            if (keepSquare) {
                float averageSize = (blockWidth + blockHeight) / 2;
                blockWidth = averageSize;
                blockHeight = averageSize;
            }

            float centerX = (float)rand.NextDouble() * SpawnBounds.Size.X + SpawnBounds.Left;
            float centerY = (float)rand.NextDouble() * SpawnBounds.Size.Y + SpawnBounds.Top;
            Vector2 position = new Vector2(centerX - blockWidth / 2, centerY - blockHeight / 2);

            return new FallingBlock(position, new Vector2(blockWidth, blockHeight), tag);
        }
        
        public FallingBlock SpawnBlock(float fallSpeed, bool keepSquare = false, string tag = FallingBlock.DefaultTag) {
            FallingBlock block = SpawnBlock(keepSquare, tag);
            block.MaxFallSpeed = fallSpeed;
            return block;
        }
    }
}
