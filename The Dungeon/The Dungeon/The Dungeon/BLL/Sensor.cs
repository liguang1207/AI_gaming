using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace The_Dungeon.BLL
{
    class Sensor
    {
        protected Actor pHost;
        protected List<Actor> pWorldActors = new List<Actor>();

        public Sensor(ref List<Actor> aWorldActors, Actor aHost)
        {
            pHost = aHost;
            pWorldActors = aWorldActors;
        }

        public virtual void Sense()
        {

        }

        public virtual void Draw(ref SpriteBatch SB)
        {
            Sense();
        }
    }
}
