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
    class bullet
    {
        public Texture2D bulletSprite;
        public Vector2 bulletCenter;
        public Vector2 velocity;
        public Vector2 position;
        public float tRotation;
        public float bulletSpeed;
        public Rectangle bulletHitBox;

        public bullet(Texture2D bulletImage, float turretRotation, Vector2 turretPosition)
        {
            bulletSprite = bulletImage;
            tRotation = turretRotation;
            position = turretPosition;
            bulletCenter = new Vector2(bulletImage.Width / 2, bulletImage.Height / 2);
            velocity = Vector2.Zero;
            bulletSpeed = 10f;
            bulletHitBox = new Rectangle((int)turretPosition.X - 2, (int)turretPosition.Y - 2, 4, 4);

        }

        public void updateBullet()
        {
            //velocity = Vector2.Zero;

            velocity.X = (float)Math.Cos(tRotation) * bulletSpeed;
            velocity.Y = (float)Math.Sin(tRotation) * bulletSpeed;
            
            position += velocity;

            bulletHitBox.X = (int)position.X-2;
            bulletHitBox.Y = (int)position.Y-2;


        }


    }
}
