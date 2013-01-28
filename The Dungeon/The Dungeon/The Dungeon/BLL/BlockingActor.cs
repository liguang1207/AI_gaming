using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dungeon.BLL
{
    class BlockingActor : Actor
    {
        public BlockingActor(Texture2D aSprite, Rectangle? aSourceRectangle, Color aSourceColor)
            : base(aSprite, aSourceRectangle, aSourceColor)
        {
            SetCollision(Actor.CollisionType.Solid);
        }
    }
}
