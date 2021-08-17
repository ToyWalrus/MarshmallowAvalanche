using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MarshmallowAvalanche {
    public class CollisionData {
        public PhysicsObject other;
        public Vector2 overlap;
        public Vector2 velocity, velocityOther;
        public Vector2 oldPos, oldPosOther, pos, posOther;

        public CollisionData(
            PhysicsObject other, 
            Vector2 overlap = default, 
            Vector2 velocity = default, 
            Vector2 velocityOther = default, 
            Vector2 oldPos = default, 
            Vector2 oldPosOther = default, 
            Vector2 pos = default, 
            Vector2 posOther = default
        ) {
            this.other = other;
            this.overlap = overlap;
            this.velocity = velocity;
            this.velocityOther = velocityOther;
            this.oldPos = oldPos;
            this.oldPosOther = oldPosOther;
            this.pos = pos;
            this.posOther = posOther;
        }
    }
}
