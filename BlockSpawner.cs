using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class BlockSpawner : Component {
        private RectangleF _spawnBounds;
        public RectangleF SpawnBounds {
            get => _spawnBounds;
            private set {
                _spawnBounds = value;
            }
        }
        private Vector2 minSpawnSize;
        private Vector2 maxSpawnSize;

        private int blockSpawnedIdCounter = 0;

        public BlockSpawner(RectangleF spawnBounds, Vector2 minSpawnSize, Vector2 maxSpawnSize) {
            SpawnBounds = spawnBounds;
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
        }

        public BlockSpawner(Vector2 position, Vector2 size, Vector2 minSpawnSize, Vector2 maxSpawnSize) {
            SpawnBounds = new RectangleF(position, size);
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
        }

        public override void OnAddedToEntity() {
            Entity.Position = SpawnBounds.Center;
        }

        #region Getters/Setters
        public void MoveSpawnLocation(Vector2 offset) {
            _spawnBounds.Location += offset;
        }

        /// <summary>
        /// Set the center of the spawn location
        /// </summary>
        public void SetSpawnLocation(Vector2 position) {
            _spawnBounds.Location = position + _spawnBounds.Size / 2;
        }

        public void SetSpawnBoundsSize(Vector2 size) {
            _spawnBounds.Size = size;
        }

        public void SetSpawnBounds(RectangleF newBounds) {
            SpawnBounds = newBounds;
        }

        public void SetMinSpawnSize(Vector2 size) {
            minSpawnSize = size;
        }

        public void SetMaxSpawnSize(Vector2 size) {
            maxSpawnSize = size;
        }
        #endregion

        public FallingBlock SpawnBlock(bool keepSquare = false) {
            System.Random rand = new System.Random();
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

            FallingBlock block = Entity.Scene.CreateEntity("falling-block-" + blockSpawnedIdCounter++)
                .AddComponent(new FallingBlock(new Vector2(blockWidth, blockHeight)));
            block.Transform.SetPosition(position);

            return block;
        }

        /// <summary>
        /// Creates an entity in the scene and attaches a FallingBlock
        /// component to it.
        /// </summary>
        /// <param name="fallSpeed"></param>
        /// <param name="keepSquare"></param>
        /// <returns></returns>
        public FallingBlock SpawnBlock(float fallSpeed, bool keepSquare = false) {
            FallingBlock block = SpawnBlock(keepSquare);
            block.MaxFallSpeed = fallSpeed;
            return block;
        }
    }
}
