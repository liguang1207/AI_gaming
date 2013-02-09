using System;
using System.Linq;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

using The_Dungeon.BLL;

using Drawing;

namespace The_Dungeon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TDGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<Actor> WorldActors = new List<Actor>();

        //Some Content
        ActorMover AM = null;
        SpriteFont DebugFont;
        Boolean bDebug = true;
        DateTime Delay = DateTime.Now;

        Pawn pPlayer = null;

        public TDGame()
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

            DrawingHelper.Initialize(GraphicsDevice);

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

            //Debug Font
            DebugFont = Content.Load<SpriteFont>("Debug Font");

            // TODO: use this.Content to load your game content here
            //Player
            Texture2D PlayerTexture = Content.Load<Texture2D>("Player");
            pPlayer = new Pawn(PlayerTexture, new Rectangle(0, 0, PlayerTexture.Width, PlayerTexture.Height), Color.SlateGray);
            pPlayer.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            //pPlayer.ToggleDebug(bDebug);
            pPlayer.Sensor = new WallSensor(ref WorldActors, pPlayer, DebugFont);
            WorldActors.Add(pPlayer);
            
            

            if (bDebug)
            {
                AM = new ActorMover(ref WorldActors, DebugFont);
                IsMouseVisible = true;
            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            WorldActors.Clear();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Delay < DateTime.Now)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    AddWall();
                    Delay = DateTime.Now.AddSeconds(0.5);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    AddEnemy();
                    Delay = DateTime.Now.AddSeconds(0.5);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.T))
                {
                    ToggleSensor();
                    Delay = DateTime.Now.AddSeconds(0.5);
                }
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (AM != null)
            {
                AM.Update();
            }

            // TODO: Add your update logic here
            foreach (Actor A in WorldActors)
            {
                if (A != null)
                {
                    A.Update(gameTime);
                }
            }

            // Check Collisions
            foreach (Actor A in WorldActors)
            {
                foreach (Actor B in WorldActors)
                {
                    if (A != null && B != null && A.Collision == Actor.CollisionType.Solid && B.Collision == Actor.CollisionType.Solid && A != B)
                    {
                        A.CheckCollision(B);
                        
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);
            spriteBatch.Begin();

            if (AM != null)
            {
                AM.Draw(ref spriteBatch);
            }
            
            // TODO: Add your drawing code here
            foreach (Actor A in WorldActors)
            {
                if (A != null)
                {
                    A.Draw(ref spriteBatch);
                    
                    //Draw Collision Rectangle Code
                    //if (bDebug)
                    //{
                    //    Texture2D rect = new Texture2D(graphics.GraphicsDevice, A.CollisionRectangle.Width, A.CollisionRectangle.Height);

                    //    Color[] data = new Color[A.CollisionRectangle.Width * A.CollisionRectangle.Height];
                    //    for (int i = 0; i < data.Length; ++i) data[i] = Color.Yellow;
                    //    rect.SetData(data);

                    //    Vector2 coor = new Vector2(A.CollisionRectangle.X, A.CollisionRectangle.Y);
                    //    spriteBatch.Draw(rect, coor, Color.White);
                    //}
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }




        protected void AddWall()
        {
            Texture2D WallTexture = Content.Load<Texture2D>("Wall");
            BlockingActor Wall = new BlockingActor(WallTexture, new Rectangle(0, 0, WallTexture.Width, WallTexture.Height), Color.SlateGray);
            Wall.Position = new Vector2(40, 40);
            //Wall.ToggleDebug(bDebug);
            WorldActors.Add(Wall);
        }

        protected void AddEnemy()
        {
            Texture2D EnemyTexture = Content.Load<Texture2D>("Enemy");
            Pawn Enemy = new Pawn(EnemyTexture, new Rectangle(0, 0, EnemyTexture.Width, EnemyTexture.Height), Color.SlateGray);
            Enemy.Position = new Vector2(100, 100);
            //Enemy.ToggleDebug(bDebug);
            Enemy.SetController(null);
            WorldActors.Add(Enemy);
        }

        private void ToggleSensor()
        {
            if (pPlayer.Sensor == null)
            {
                pPlayer.Sensor = new WallSensor(ref WorldActors, pPlayer, DebugFont);
            }
            else if (pPlayer.Sensor is WallSensor)
            {
                pPlayer.Sensor = new AgentSensor(ref WorldActors, pPlayer, DebugFont);
            }
            else if (pPlayer.Sensor is AgentSensor)
            {
                pPlayer.Sensor = new PieSensor(ref WorldActors, pPlayer, DebugFont);
            }
            else
            {
                pPlayer.Sensor = new WallSensor(ref WorldActors, pPlayer, DebugFont);
            }
        }
    }
}
