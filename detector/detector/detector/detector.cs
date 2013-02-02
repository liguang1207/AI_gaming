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
        SpriteFont text; 
        SpriteBatch spriteBatch;
        Texture2D sprite;
        Texture2D sprite2;
        Rectangle spriteRectangle;
        Texture2D dot;
        Texture2D rec;
        Rectangle objRec;

        BasicEffect basicEffect;
        VertexPositionColor[] vertices;

        float rotation = 0;
        Vector2 spritePosition = new Vector2(50f,50f);
        Vector2 spritePosition2 = new Vector2(100f, 100f);
        Vector2 spriteCenter;
        Vector2 spriteVelocity;
       
        const float rotationVelocity = 4f;
        float friction = 0.1f;

        Vector2 bound;
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
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane

            vertices = new VertexPositionColor[6];
            
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
            spriteRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, sprite.Width, sprite.Height);
            sprite2 = Content.Load<Texture2D>("Circle");
            rec = Content.Load<Texture2D>("rectangle");
            objRec = new Rectangle(300, 300, rec.Width, rec.Height);

            dot = Content.Load <Texture2D> ("dot");

            text = Content.Load<SpriteFont>("font");
            
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


            bound.X = (spritePosition.X + (sprite.Width / 2)) ;
            bound.Y = spritePosition.Y;


            spriteCenter = new Vector2(sprite.Width/2 , sprite.Height/2);
            /*if (bound.X >= objRec.Left)
            {
                spritePosition.X -= spriteVelocity.X;
            }
            */
            if (keyboard.IsKeyDown(Keys.Up) /*&& bound.X < objRec.Left*/)
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
                spriteVelocity.X = -(float)Math.Cos(rotation) * rotationVelocity;
                spriteVelocity.Y = -(float)Math.Sin(rotation) * rotationVelocity;
            }
            
            if (keyboard.IsKeyDown(Keys.Left))
            {
                rotation = (float)(2 * Math.PI + (rotation - .1)) % (float)(2 * Math.PI);
                //rotation = (float)(rotation - .1)%(float)(2*Math.PI);
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                rotation = (float)(rotation + .1) % (float)(2 * Math.PI);
            }
            if (Vector2.Distance(spritePosition,spritePosition2) < sprite.Width)
            {
                spriteVelocity.X = -(float)Math.Cos(rotation) * rotationVelocity;
                spriteVelocity.Y = -(float)Math.Sin(rotation) * rotationVelocity;
            }
            vertices[0].Position = new Vector3(spritePosition.X, spritePosition.Y, 0);
            vertices[0].Color = Color.Violet;
            vertices[1].Position = new Vector3((float)Math.Cos(rotation) * 100+spritePosition.X, (float)Math.Sin(rotation) *100 + spritePosition.Y, 0);
            vertices[1].Color = Color.Violet;
            vertices[2].Position = new Vector3(spritePosition.X, spritePosition.Y, 0);
            vertices[2].Color = Color.Violet;
            vertices[3].Position = new Vector3((float)Math.Cos(rotation + Math.PI / 4) * 100 + spritePosition.X, (float)Math.Sin(rotation + Math.PI / 4) * 100 + spritePosition.Y, 0);
            vertices[3].Color = Color.Violet;
            vertices[4].Position = new Vector3(spritePosition.X, spritePosition.Y, 0);
            vertices[4].Color = Color.Violet;
            vertices[5].Position = new Vector3((float)Math.Cos(rotation % (2 * Math.PI) + 3 * Math.PI / 4) * 100 + spritePosition.X, (float)Math.Sin(rotation % (2 * Math.PI) + 3 * Math.PI / 4) * 100 + spritePosition.Y, 0);
            vertices[5].Color = Color.Violet;
           
            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, spritePosition,null, Color.White,rotation,spriteCenter,1f,SpriteEffects.None,0);
            spriteBatch.Draw(sprite2, spritePosition2, null, Color.White, rotation, spriteCenter, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(rec, objRec, Color.White);
            
            //spriteBatch.DrawString(text,objRec.Bottom.ToString(),new Vector2(100f,100f),Color.Red);
            
            spriteBatch.DrawString(text, bound.X.ToString(), new Vector2(100f, 150f), Color.Red);
            spriteBatch.DrawString(text, (rotation%(2*Math.PI)).ToString(), new Vector2(100f, 200f), Color.Red);
            spriteBatch.DrawString(text, "angle degree "+(rotation*180/Math.PI%360).ToString(), new Vector2(100f, 250f), Color.Red);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
