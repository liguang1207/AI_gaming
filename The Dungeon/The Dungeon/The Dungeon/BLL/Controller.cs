using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace The_Dungeon.BLL
{
    class Controller
    {
        Pawn pActor;

        public Controller(Pawn aActor)
        {
            pActor = aActor;
        }

        public virtual void SetActor(Pawn aActor)
        {
            pActor = aActor;
        }

        public virtual void HandleInput()
        {
            KeyboardState keyboard = Keyboard.GetState();
            Vector2 Velocity = pActor.Velocity;
            
            if (keyboard.IsKeyDown(Keys.Up))
            {
                Velocity.X = (float)Math.Cos(pActor.Rotation) * pActor.RotationalVelocity;
                Velocity.Y = (float)Math.Sin(pActor.Rotation) * pActor.RotationalVelocity;
            }
            else if (Velocity != Vector2.Zero)
            {
                Velocity.X = 0;
                Velocity.Y = 0;
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                Velocity.X = -(float)Math.Cos(pActor.Rotation) * pActor.RotationalVelocity;
                Velocity.Y = -(float)Math.Sin(pActor.Rotation) * pActor.RotationalVelocity;
            }

            if (keyboard.IsKeyDown(Keys.Left))
            {
                pActor.Rotation -= .1f;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                pActor.Rotation += .1f;
            }

            pActor.Velocity = Velocity;
        }
    }
}
