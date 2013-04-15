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
    class player
    {
        public Vector2 spritePosition;
        public Vector2 spriteCenter;
        public Vector2 spriteVelocity;
        public Texture2D playerSprite;
        public float spriteSpeed = 5;
        public float seekSpeed = 2;
        public float rotationVelocity = 4f;
        public float Rotation = 0;
        public Rectangle playerRec;
        public float distance;
        public Vector2 TempVector;
        public Vector2 Vector90Degree;
        public int curSeek = 0;
        public Vector2 VectorA;
        public Vector2 VectorB;
        public Vector2 VectorA90;
        public double radian, radian90;
        public Vector2 seekCoord = new Vector2(-1, -1);
        public List<Tiles> FollowPath = new List<Tiles>();
        public bool pathFound = false;
        public int health = 100;


        public MouseState mouse;
        public MouseState preState;
        public Vector2 click = new Vector2(-1, -1);
        public player(Vector2 Pos, Vector2 Center, Rectangle rec, Texture2D sprite)
        {
            spritePosition = Pos;
            spriteCenter = Center;
            playerRec = rec;
            playerSprite = sprite;
        }

        //function to let the player seek to the list of points found in the shortest path in A*
        public virtual bool seekPath(Vector2 coord)
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
            playerRec.X = (int)spritePosition.X-25;
            playerRec.Y = (int)spritePosition.Y-25;
            if ((spritePosition.X > seekCoord.X - 5 && spritePosition.X < seekCoord.X + 5) && (spritePosition.Y > seekCoord.Y - 5 && spritePosition.Y < seekCoord.Y + 5))
            {
                //return true if the player is at its destination
                return true;
            }
            return false;


        }

        //let the agents seek to the mouse clicked position
        public void seekMovement()
        {
            mouse = Mouse.GetState();
            Vector2 direction;
            spriteVelocity = Vector2.Zero;

            direction = click - spritePosition;
            distance = calculateDistance(click, spritePosition);

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            if (mouse.LeftButton == ButtonState.Pressed && preState.LeftButton == ButtonState.Released)
            {

                click = new Vector2(mouse.X, mouse.Y);


                //find a random point that the heading is point to
                TempVector = new Vector2((float)Math.Cos(Rotation) * 50 + spritePosition.X, (float)Math.Sin(Rotation) * 50 + spritePosition.Y);

                //Made a point that is 90 degrees of the heading - this is used for testing the angles for the two sides 
                Vector90Degree = new Vector2((float)Math.Cos(Rotation + (Math.PI / 2)) * 100 + spritePosition.X, (float)Math.Sin(Rotation + (Math.PI / 2)) * 100 + spritePosition.Y);

                //find the vector between the player and the point its heading to
                VectorA = new Vector2(TempVector.X - spritePosition.X, TempVector.Y - spritePosition.Y);
                VectorA90 = new Vector2(Vector90Degree.X - spritePosition.X, Vector90Degree.Y - spritePosition.Y);

                //find the vector between the player and the enemy
                VectorB = new Vector2(click.X - spritePosition.X, click.Y - spritePosition.Y);



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


            }

            //when the agents are at the click location reset the click so they dont move there again later when seek is turned on.
            if (click.X != -1 && click.Y != -1)
            {
                if (distance < spriteSpeed)
                {
                    spriteVelocity += direction * spriteSpeed;
                }
                else
                {
                    spriteVelocity += direction * spriteSpeed;
                }

            }
            spritePosition += spriteVelocity;
            if ((spritePosition.X > click.X - 5 && spritePosition.X < click.X + 5) && (spritePosition.Y > click.Y - 5 && spritePosition.Y < click.Y + 5))
            {
                click = new Vector2(-1, -1);

            }
            preState = Mouse.GetState();
        }
        public double DotProduct(Vector2 A, Vector2 B)
        {
            double radians, DotProdNum, DotProdDenom;
            //the dot product numerator 
            DotProdNum = A.X * B.X + A.Y * B.Y;
            //the dot product denominator
            DotProdDenom = Math.Sqrt(A.X * A.X + A.Y * A.Y) * (Math.Sqrt(B.X * B.X + B.Y * B.Y));


            radians = Math.Acos(Math.Round(DotProdNum / DotProdDenom, 6));
            return radians;
        }

        //calculating the distance between two vectors 
        public float calculateDistance(Vector2 A, Vector2 B)
        {
            float Y_diff, X_diff, distance;
            A = new Vector2(Math.Abs(A.X), Math.Abs(A.Y));
            B = new Vector2(Math.Abs(B.X), Math.Abs(B.Y));

            Y_diff = A.Y - B.Y;
            X_diff = A.X - B.X;

            distance = (float)Math.Sqrt(Math.Pow(X_diff, 2) + Math.Pow(Y_diff, 2));

            return Math.Abs(distance);

        }

        //let the user control the player to move
        public void updateMovement()
        {
            KeyboardState keyboard = Keyboard.GetState();

            spritePosition += spriteVelocity;

            //move forward
            if (keyboard.IsKeyDown(Keys.Up))
            {
                spriteVelocity.X = (float)Math.Cos(Rotation) * rotationVelocity;
                spriteVelocity.Y = (float)Math.Sin(Rotation) * rotationVelocity;
            }
            //stop moving
            else if (spriteVelocity != Vector2.Zero)
            {
                spriteVelocity.X = 0;
                spriteVelocity.Y = 0;

                //use below equation if dont want the ball to stop immediately 
                //spriteVelocity -=  friction * spriteVelocity;
            }

            //move backwards
            if (keyboard.IsKeyDown(Keys.Down))
            {
                spriteVelocity.X = -(float)Math.Cos(Rotation) * rotationVelocity;
                spriteVelocity.Y = -(float)Math.Sin(Rotation) * rotationVelocity;
            }

            //rotate heading left or right
            if (keyboard.IsKeyDown(Keys.Left))
            {
                Rotation = (float)(2 * Math.PI + (Rotation - .1)) % (float)(2 * Math.PI);
                //rotation = (float)(rotation - .1)%(float)(2*Math.PI);
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                Rotation = (float)(Rotation + .1) % (float)(2 * Math.PI);
            }
        }
    }
}
