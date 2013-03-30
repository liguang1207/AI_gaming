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
    class turrets
    {
        public Vector2 spritePosition;
        public Vector2 spriteCenter;
        public Vector2 spriteVelocity;
        public Texture2D playerSprite;
        public float spriteSpeed = 5;
        public float seekSpeed = 2;
        const float rotationVelocity = 4f;
        public float Rotation = 0;
        public Rectangle turretRec;
        public float distance;
        public Vector2 TempVector;
        public Vector2 Vector90Degree;

        public Vector2 VectorA;
        public Vector2 VectorB;
        public Vector2 VectorA90;
        public double radian, radian90;
        public Vector2 seekCoord = new Vector2(-1, -1);

        public MouseState mouse;
        public MouseState preState;
        public Vector2 click = new Vector2(-1, -1);
        public turrets(Vector2 Pos, Vector2 Center, Rectangle rec, Texture2D sprite)
        {
            spritePosition = Pos;
            spriteCenter = Center;
            turretRec = rec;
            playerSprite = sprite;
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

    }
}
