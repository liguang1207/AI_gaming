using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace The_Dungeon.BLL
{
    class ActorMover
    {
        Actor Selected = null;
        List<Actor> WorldActors = null;
        SpriteFont DebugFont;
        DateTime Delay = DateTime.Now;

        public ActorMover(ref List<Actor> aWorldActors, SpriteFont aDebugFont)
        {
            WorldActors = aWorldActors;
            DebugFont = aDebugFont;
        }

        public void Draw(ref SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(DebugFont, "Current Actor: " + (Selected == null ? "None" : Selected.ToString()), new Vector2(4, 4), Color.White);
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();

            if (Delay < DateTime.Now && mouse.LeftButton == ButtonState.Pressed)
            {
                if (Selected == null)
                {
                    Rectangle R = new Rectangle(mouse.X-3, mouse.Y-3, 2, 2);

                    for (int i = 0; i < WorldActors.Count(); i++)
                    {
                        Actor A = WorldActors[i] as Actor;

                        if (R.Intersects(A.CollisionRectangle))
                        {
                            Selected = A;
                            Delay = DateTime.Now.AddSeconds(0.5);
                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < WorldActors.Count(); i++)
                    {
                        Actor A = WorldActors[i] as Actor;

                        if (A == Selected)
                        {
                            Selected.Position = new Vector2(mouse.X, mouse.Y);
                            WorldActors[i] = Selected;
                            Selected = null;

                            Delay = DateTime.Now.AddSeconds(0.5);

                            return;
                        }
                    }

                }
            }
        }
    }
}
