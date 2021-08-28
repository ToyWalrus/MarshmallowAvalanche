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
        private float minSpawnSize;
        private float maxSpawnSize;

        private int blockSpawnedIdCounter = 0;

        public BlockSpawner(RectangleF spawnBounds, float minSpawnSize, float maxSpawnSize) {
            SpawnBounds = spawnBounds;
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
        }

        public BlockSpawner(Vector2 position, Vector2 size, float minSpawnSize, float maxSpawnSize) {
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

        public void SetMinSpawnSize(float size) {
            minSpawnSize = size;
        }

        public void SetMaxSpawnSize(float size) {
            maxSpawnSize = size;
        }
        #endregion

        public FallingBlock SpawnBlock() {
            System.Random rand = new System.Random();
            float size = (float)rand.NextDouble() * (maxSpawnSize - minSpawnSize) + minSpawnSize;
            float centerX = (float)rand.NextDouble() * SpawnBounds.Size.X + SpawnBounds.Left;
            float centerY = (float)rand.NextDouble() * SpawnBounds.Size.Y + SpawnBounds.Top;

            Vector2 position = new Vector2(centerX - size / 2, centerY - size / 2);

            FallingBlock block = Entity.Scene.CreateEntity("falling-block-" + blockSpawnedIdCounter++)
                .AddComponent(new FallingBlock(new Vector2(size, size)));
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
        public FallingBlock SpawnBlock(float fallSpeed) {
            FallingBlock block = SpawnBlock();
            block.MaxFallSpeed = fallSpeed;
            return block;
        }
    }
}
