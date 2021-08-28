using MarshmallowAvalanche.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace MarshmallowAvalanche {
    public class BlockSpawner : Component {
        public Vector2 Size { get; private set; }
        private float minSpawnSize;
        private float maxSpawnSize;

        private int blockSpawnedIdCounter = 0;

        public BlockSpawner(Vector2 boundsSize, float minSpawnSize, float maxSpawnSize) {
            Size = boundsSize;
            this.minSpawnSize = minSpawnSize;
            this.maxSpawnSize = maxSpawnSize;
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

        public FallingBlock SpawnBlock() {
            System.Random rand = new System.Random();
            Vector2 entityPosition = Entity.Position;

            float size = (float)rand.NextDouble() * (maxSpawnSize - minSpawnSize) + minSpawnSize;
            float centerX = (float)rand.NextDouble() * Size.X + entityPosition.X;
            float centerY = (float)rand.NextDouble() * Size.Y + entityPosition.Y;

            Vector2 blockPosition = new Vector2(centerX - size / 2, centerY - size / 2);

            FallingBlock block = Entity.Scene.CreateEntity("falling-block-" + blockSpawnedIdCounter++)
                .AddComponent(new FallingBlock(new Vector2(size, size)));
            block.Transform.SetPosition(blockPosition);

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
