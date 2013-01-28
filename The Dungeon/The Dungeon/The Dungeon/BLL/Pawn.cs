using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dungeon.BLL
{
    class Pawn : Actor
    {
        Controller pController;
        Sensor pSensor = null;

        public Pawn(Texture2D aSprite, Rectangle? aSourceRectangle, Color aSourceColor)
            : base(aSprite, aSourceRectangle, aSourceColor)
        {
            pController = new Controller(this);

            SetCollision(Actor.CollisionType.Solid);
        }

        public Sensor Sensor
        {
            get { return pSensor; }
            set { pSensor = value; }
        }

        public virtual void SetController(Controller C)
        {
            pController = C;
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
        }

        public override void Draw(ref SpriteBatch SB)
        {
            base.Draw(ref SB);

            if (pSensor != null)
                pSensor.Draw(ref SB);
        }


        public virtual void HandleInput()
        {
            if (pController != null)
            {
                pController.HandleInput();
            }

            Position += Velocity;
        }
    }
}
