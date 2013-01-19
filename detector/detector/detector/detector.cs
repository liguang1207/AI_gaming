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

namespace detector
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class detector : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D sprite;
        Rectangle spriteRectangle; 

        float rotation;
        Vector2 spritePosition;
        Vector2 spriteCenter;
        Vector2 spriteVelocity;

        const float rotationVelocity = 5f;
        float friction = 0.1f;
        public detector()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprite = Content.Load<Texture2D>("Circle");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            spriteRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, sprite.Width, sprite.Height);
            // TODO: Add your update logic here
            spritePosition += spriteVelocity;
            spriteCenter = new Vector2(sprite.Width / 2, sprite.Height / 2);

            if (keyboard.IsKeyDown(Keys.Up))
            {
                spriteVelocity.X = (float)Math.Cos(rotation) * rotationVelocity;
                spriteVelocity.Y = (float)Math.Sin(rotation) * rotationVelocity;
            }
            else if(spriteVelocity != Vector2.Zero)
            {
                spriteVelocity.X = 0;
                spriteVelocity.Y = 0;

                //use below equation if dont want the ball to stop immediately 
                //spriteVelocity -=  friction * spriteVelocity;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                //position.Y += 5.05f;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                rotation -= .1f;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                rotation += .1f;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, spritePosition,null, Color.White, rotation,spriteCenter,1f,SpriteEffects.None,0);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
