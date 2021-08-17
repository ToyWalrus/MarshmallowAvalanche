using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche.Physics {
    public class CollisionData {
        public PhysicsObject other;
        public Vector2 overlap;
        public Vector2 pos;
        public Vector2 posOther;

        public CollisionData(
            PhysicsObject other,
            Vector2 overlap = default,
            Vector2 pos = default,
            Vector2 posOther = default
        ) {
            this.other = other;
            this.overlap = overlap;
            this.pos = pos;
            this.posOther = posOther;
        }
    }
}
