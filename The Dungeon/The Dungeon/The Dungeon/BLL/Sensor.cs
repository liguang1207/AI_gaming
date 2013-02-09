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
        protected String DebugInformation = String.Empty;
        protected SpriteFont DebugFont;

        public Sensor(ref List<Actor> aWorldActors, Actor aHost, SpriteFont aDebugFont)
        {
            pHost = aHost;
            pWorldActors = aWorldActors;
            DebugFont = aDebugFont;
        }

        public virtual void Sense()
        {
            DebugInformation = String.Empty;
        }

        public virtual void Draw(ref SpriteBatch SB)
        {
            Sense();

            if(DebugFont != null)
            SB.DrawString(DebugFont, DebugInformation, new Vector2(10, 30), Color.White);
        }

        public virtual String GetDebugInfo()
        {
            return DebugInformation;
        }
    }
}
