using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class enemyCharger: player
    {
        public float rotationVelocity = 2f;
        public enemyCharger(Vector2 Pos, Vector2 Center, Rectangle rec, Texture2D sprite)
            : base(Pos, Center, rec, sprite)
        { }
        public override bool seekPath(Vector2 coord)
        {


            spriteVelocity = Vector2.Zero;


            distance = calculateDistance(seekCoord, spritePosition);


            seekCoord = new Vector2(coord.X, coord.Y);


            //find a random point that the heading is point to
            TempVector = new Vector2((float)Math.Cos(Rotation) * 50 + spritePosition.X, (float)Math.Sin(Rotation) * 50 + spritePosition.Y);

            //Made a point that is 90 degrees of the heading - this is used for testing the angles for the two sides 
            Vector90Degree = new Vector2((float)Math.Cos(Rotation + (Math.PI / 2)) * 100 + spritePosition.X, (float)Math.Sin(Rotation + (Math.PI / 2)) * 100 + spritePosition.Y);

            //find the vector between the player and the point its heading to
            VectorA = new Vector2(TempVector.X - spritePosition.X, TempVector.Y - spritePosition.Y);
            VectorA90 = new Vector2(Vector90Degree.X - spritePosition.X, Vector90Degree.Y - spritePosition.Y);

            //find the vector between the player and the enemy
            VectorB = new Vector2(seekCoord.X - spritePosition.X, seekCoord.Y - spritePosition.Y);



            //degree of the enemy to the head of the player.
            //if enemy is 90 degrees to the head of the player then the angle will be pi/2
            radian = DotProduct(VectorA, VectorB);


            //this is the degree of the enemy to pi/2 degrees of the head. 
            //so if the enemy is pi/2 degrees to the original head of the player, the new angle will be 0.
            radian90 = DotProduct(VectorA90, VectorB);

            if (radian90 < Math.PI / 2)
            {
                radian = 2 * Math.PI - radian;
            }
            Rotation += -(float)radian;

            spriteVelocity.X = (float)Math.Cos(Rotation) * rotationVelocity;
            spriteVelocity.Y = (float)Math.Sin(Rotation) * rotationVelocity;



            spritePosition += spriteVelocity;
            playerRec.X = (int)spritePosition.X - 25;
            playerRec.Y = (int)spritePosition.Y - 25;
            if ((spritePosition.X > seekCoord.X - 5 && spritePosition.X < seekCoord.X + 5) && (spritePosition.Y > seekCoord.Y - 5 && spritePosition.Y < seekCoord.Y + 5))
            {
                //return true if the player is at its destination
                return true;
            }
            return false;


        }
    }
}
