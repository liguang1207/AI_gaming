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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Astar : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D debugTiles;
        Texture2D normTiles;
        Texture2D wall;
        Texture2D player1;
        Texture2D target;
        Texture2D enemy1;
        Texture2D pathTile;
        Texture2D turret1Image;
        Texture2D bullet1Image;
        Texture2D healthBar;
        player actor;
        player badActor;
        List<player> enemy = new List<player>();
        List<turrets> turretList = new List<turrets>();
        List<Tiles> openList = new List<Tiles>();
        List<Tiles> closedList = new List<Tiles>();
        List<Tiles> shortestPath = new List<Tiles>();

        int targetX;
        int targetY;
        int gridBoundX;
        int gridBoundY;
        Tiles[][] grid;
        bool blocked = false;
        bool wallOn = false;
        bool playerOn = false;
        bool targetOn = false;
        bool enemyOn = false;
        bool enemyExist = false;
        bool playerExist = false;
        bool targetExist = false;
        bool pathfinding = false;
        bool seek = false;
        bool runPath = false;
        bool targetFound = false;
        bool debug = false;
        bool turret1Control = false;
        bool found = false;
        SpriteFont text;
        int startX, startY;
        int tempX;
        int tempY;
        int tempt, tempb, temptr, temptl, templ, tempr, tempbl, tempbr;

        float timer;
        const float TIMER = 2;

        MouseState mouse;
        MouseState preState;
        KeyboardState keyboard;
        KeyboardState prevkeyboard;
        public Astar()
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

            IsMouseVisible = true;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            debugTiles = Content.Load<Texture2D>("debugTiles");
            wall = Content.Load<Texture2D>("Wall");
            player1 = Content.Load<Texture2D>("player");
            target = Content.Load<Texture2D>("target");
            text = Content.Load<SpriteFont>("font");
            enemy1 = Content.Load<Texture2D>("enemy");
            pathTile = Content.Load<Texture2D>("pathTile");
            normTiles = Content.Load<Texture2D>("normTiles");
            turret1Image = Content.Load<Texture2D>("turret1");
            bullet1Image = Content.Load<Texture2D>("bullet1");
            healthBar = Content.Load<Texture2D>("HealthBar");
            gridBoundX = GraphicsDevice.Viewport.Height;
            gridBoundY = GraphicsDevice.Viewport.Width;
            grid = new Tiles[(GraphicsDevice.Viewport.Height)][];

            //creating the game grid
            for (int i = 0; i < (GraphicsDevice.Viewport.Height); i++)
            {
                grid[i] = new Tiles[(GraphicsDevice.Viewport.Width)];

                for (int j = 0; j < (GraphicsDevice.Viewport.Width); j++)
                {
                    grid[i][j] = new Tiles(new Vector2((j * 50) + 25, i * 50 + 25), blocked, new Rectangle(j * 50, i * 50, 50, 50), normTiles, i, j);
                }
            }

            grid[3][15].ifTarget(true, target, debugTiles);
            targetExist = true;
            targetX = (int)grid[3][15].center.Y / 50;
            targetY = (int)grid[3][15].center.X / 50;

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            modeInput();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
            if (timer < 0)
            {
                enemyCharger Charger = new enemyCharger(grid[3][0].center, new Vector2(player1.Width / 2, player1.Height / 2), new Rectangle((int)grid[3][0].center.X - 25, (int)grid[3][0].center.Y - 25, 50, 50), enemy1);
                //Timer expired, execute action
                enemyExist = true;
                badActor = new player(grid[3][0].center, new Vector2(player1.Width / 2, player1.Height / 2), new Rectangle((int)grid[3][0].center.X - 25, (int)grid[3][0].center.Y - 25, 50, 50), enemy1);
                enemy.Add(badActor);
                enemy.Add(Charger);
                timer = TIMER;   //Reset Timer
            }

            foreach (turrets T in turretList)
            {
                if (enemy.Count != 0)
                {
                    T.ScanEnemies(enemy, gameTime);

                }
                for (int i = 0; i < T.bulletList.Count; i++)
                {
                    T.bulletList[i].updateBullet();
                    for (int j = 0; j < enemy.Count; j++)
                    {
                        if (T.bulletList[i].bulletHitBox.Intersects(enemy[j].playerRec))
                        {
                            enemy[j].health -= 20;
                            T.bulletList.RemoveAt(i);
                            if (enemy[j].health < 0)
                            {
                                enemy.RemoveAt(j);
                                
                            }
                            i--;
                            break;
                        }

                    }


                }
            }

            foreach (turrets R in turretList)
            {
                for (int i = 0; i < R.bulletList.Count; i++)
                {
                    if (Vector2.Distance(R.bulletList[i].position, R.spritePosition) > R.MAX_RANGE)
                    {
                        R.bulletList.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            
            //run the path found by A*
            foreach (player A in enemy)
            {
                if (A is enemyCharger && !A.pathFound)
                {
                    found = false;
                    targetFound = false;
                    openList.Clear();
                    closedList.Clear();
                    shortestPath.Clear();
                    for (int i = 0; i < (GraphicsDevice.Viewport.Height); i++)
                    {

                        for (int j = 0; j < (GraphicsDevice.Viewport.Width); j++)
                        {
                            grid[i][j].open = false;
                            grid[i][j].closed = false;
                        }
                    }



                    initialFindPathCharger();
                    A.pathFound = found;
                    A.FollowPath.AddRange(shortestPath);
                }
                
                else if (!A.pathFound)
                {
                    found = false;
                    targetFound = false;
                    openList.Clear();
                    closedList.Clear();
                    shortestPath.Clear();
                    for (int i = 0; i < (GraphicsDevice.Viewport.Height); i++)
                    {

                        for (int j = 0; j < (GraphicsDevice.Viewport.Width); j++)
                        {
                            grid[i][j].open = false;
                            grid[i][j].closed = false;
                        }
                    }



                    initialFindPath();
                    A.pathFound = found;
                    A.FollowPath.AddRange(shortestPath);


                }
                if (A.pathFound && A.FollowPath.Count > 0 && A.curSeek < A.FollowPath.Count && A.seekPath(A.FollowPath[A.curSeek].center))
                {

                    A.curSeek++;


                }



            }
            for (int k = 0; k < enemy.Count; k++)
            {
                if (enemy[k].curSeek == enemy[k].FollowPath.Count && enemy[k].pathFound)
                {
                    enemy.RemoveAt(k);
                    k--;
                }
            }

            //every agent on the board will seek to the mouse click
            if (seek)
            {
                if (playerExist)
                {
                    actor.seekMovement();
                }
                if (enemyExist)
                {
                    foreach (player A in enemy)
                    {
                        if (A != null)
                        {
                            A.seekMovement();

                        }
                    }
                }

            }

            //if A* is turned on, and player and target exist, find the shortest path to target



            mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed && preState.LeftButton == ButtonState.Released)
            {
                //create/delete wall at click
                if (wallOn)
                {
                    grid[mouse.Y / 50][mouse.X / 50].ifBlocked(true, wall, debugTiles);
                }

                //create/move player to click
                else if (playerOn)
                {
                    playerExist = true;
                    actor = new player(grid[mouse.Y / 50][mouse.X / 50].center, new Vector2(player1.Width / 2, player1.Height / 2), new Rectangle(mouse.X, mouse.Y, 50, 50), player1);

                }

                //create enemy at click
                else if (enemyOn)
                {
                    enemyExist = true;
                    badActor = new player(grid[mouse.Y / 50][mouse.X / 50].center, new Vector2(player1.Width / 2, player1.Height / 2), new Rectangle(mouse.X, mouse.Y, 50, 50), enemy1);
                    enemy.Add(badActor);
                }

                //create/delete target at click
                else if (targetOn)
                {
                    targetExist = true;
                    grid[mouse.Y / 50][mouse.X / 50].ifTarget(true, target, debugTiles);
                    targetX = mouse.Y / 50;
                    targetY = mouse.X / 50;
                }
                else if (turret1Control && grid[mouse.Y / 50][mouse.X / 50].blocked)
                {
                    bool checkTurret = false;
                    int i;
                    for (i = 0; i < turretList.Count; i++)
                    {
                        if (turretList[i].turretRec.Contains(mouse.X, mouse.Y))
                        {
                            checkTurret = true;
                            break;
                        }
                    }
                    if (!checkTurret)
                    {
                        turrets newTurret = new turrets(grid[mouse.Y / 50][mouse.X / 50].center, new Vector2(turret1Image.Width / 2, turret1Image.Height / 2), new Rectangle((int)grid[mouse.Y / 50][mouse.X / 50].center.X - 25, (int)grid[mouse.Y / 50][mouse.X / 50].center.Y - 25, 50, 50), turret1Image, bullet1Image);
                        turretList.Add(newTurret);
                    }
                    else
                    {
                        turretList.RemoveAt(turretList.IndexOf(turretList[i]));
                    }
                }


            }

            preState = Mouse.GetState();

            //if no other functions are running then the user can move the player
            if (playerExist && !seek && !pathfinding && !runPath)
            {
                actor.updateMovement();
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

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //if debug is turned on the game board will turn to tiles
            if (debug)
            {
                for (int i = 0; i < (GraphicsDevice.Viewport.Height / 10); i++)
                {
                    for (int j = 0; j < (GraphicsDevice.Viewport.Width / 10); j++)
                    {
                        if (!grid[i][j].blocked && !grid[i][j].target && !grid[i][j].path)
                        {
                            grid[i][j].tiles = debugTiles;
                        }
                        spriteBatch.Draw(grid[i][j].tiles, grid[i][j].rec, Color.White);
                    }
                }
            }
            else if (!debug)
            {
                for (int i = 0; i < (GraphicsDevice.Viewport.Height / 10); i++)
                {
                    for (int j = 0; j < (GraphicsDevice.Viewport.Width / 10); j++)
                    {
                        if (!grid[i][j].blocked && !grid[i][j].target && !grid[i][j].path)
                        {
                            grid[i][j].tiles = normTiles;
                        }
                        spriteBatch.Draw(grid[i][j].tiles, grid[i][j].rec, Color.White);
                    }
                }
            }

            //draw the player location and coord to the board
            if (playerExist)
            {
                //spriteBatch.Draw(player1, new Rectangle(50, 50, 50, 50), Color.White);
                spriteBatch.Draw(actor.playerSprite, actor.spritePosition, null, Color.White, actor.Rotation, actor.spriteCenter, 1f, SpriteEffects.None, 0);
                //spriteBatch.DrawString(text, actor.spritePosition.ToString(), new Vector2(0, 0), Color.Red);

            }

            //draw the enemies to the board
            if (enemyExist)
            {
                foreach (player A in enemy)
                {
                    spriteBatch.Draw(A.playerSprite, A.spritePosition, null, Color.White, A.Rotation, A.spriteCenter, 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(healthBar, new Rectangle((int)A.spritePosition.X-25,(int)A.spritePosition.Y-30,(int)(50*((double)A.health/100)),4),Color.Red);
                    
                }
            }
            if (turretList.Count != 0)
            {
                foreach (turrets T in turretList)
                {
                    spriteBatch.Draw(T.turretSprite, T.spritePosition, null, Color.White, T.Rotation, T.spriteCenter, 1f, SpriteEffects.None, 0);
                    //spriteBatch.DrawString(text, T.bulletList.Count.ToString(), new Vector2(20, 20), Color.Red);
                    foreach (bullet B in T.bulletList)
                    {
                        spriteBatch.Draw(B.bulletSprite, B.position, Color.White);
                    }
                }
            }


            //draw the tiles marked as the shortest path to target
            if (pathfinding)
            {


                for (int e = 0; e < shortestPath.Count; e++)
                {
                    spriteBatch.DrawString(text, shortestPath[e].coordX.ToString() + " " + shortestPath[e].coordY.ToString() + " " + shortestPath[e].center.ToString(), new Vector2(0, (e + 10) * 20), Color.Red);
                    grid[shortestPath[e].coordX][shortestPath[e].coordY].tiles = pathTile;
                    grid[shortestPath[e].coordX][shortestPath[e].coordY].path = true;
                }

                //print information on all the node visited to text
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("dataDump.txt"))
                {
                    for (int q = 0; q < closedList.Count; q++)
                    {
                        file.WriteLine("center coord of tile" + closedList[q].center.ToString() + "; Tile coord (" + closedList[q].coordX.ToString() + "," + closedList[q].coordY.ToString() + ")");
                    }
                    file.WriteLine("shortestPath choosen:");
                    for (int z = 0; z < shortestPath.Count; z++)
                    {
                        file.WriteLine("center coord of the tile" + shortestPath[z].center.ToString() + "; Tile coord (" + shortestPath[z].coordX.ToString() + "," + shortestPath[z].coordY.ToString() + ")");
                    }

                }


            }

            //if debug is on print numbers to each tile
            if (debug)
            {
                int num = 0;
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        spriteBatch.DrawString(text, num.ToString(), new Vector2(x * 50, y * 50), Color.Red);
                        num++;
                    }
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void initialFindPathCharger()
        {
            if (enemyExist && targetExist && !targetFound)
            {

                int curX, curY;

                //use the player's current location as the start node
                curX = (int)grid[3][0].center.Y / 50;
                curY = (int)grid[3][0].center.X / 50;
                startX = curX;
                startY = curY;
                grid[curX][curY].cost = 0;
                grid[curX][curY].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY));
                grid[curX][curY].total = grid[curX][curY].cost + grid[curX][curY].heu;
                grid[curX][curY].open = true;
                openList.Add(grid[curX][curY]);

                //first run through initializing the open lists with all adjacent nodes thats not a wall
                //bot
                if (curX + 1 > -1 && curX + 1 < gridBoundX && curY > -1 && curY < gridBoundY  && !grid[curX + 1][curY].closed)
                {
                    grid[curX + 1][curY].parentTile = grid[curX][curY];
                    grid[curX + 1][curY].cost = 10;
                    grid[curX + 1][curY].heu = Math.Abs(targetX - (curX + 1)) + Math.Abs(targetY - (curY));
                    grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                    grid[curX + 1][curY].open = true;
                    if (grid[curX + 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX + 1][curY]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX + 1][curY]);
                    }
                    tempb = grid[curX + 1][curY].total;
                }


                //top
                if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY  && !grid[curX - 1][curY].closed)
                {
                    grid[curX - 1][curY].parentTile = grid[curX][curY];
                    grid[curX - 1][curY].cost = 10;
                    grid[curX - 1][curY].heu = Math.Abs(targetX - (curX - 1)) + Math.Abs(targetY - (curY));
                    grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                    grid[curX - 1][curY].open = true;
                    if (grid[curX - 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX - 1][curY]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX - 1][curY]);
                    }
                    tempt = grid[curX - 1][curY].total;
                }

                //left
                if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY  && !grid[curX][curY - 1].closed)
                {
                    grid[curX][curY - 1].parentTile = grid[curX][curY];
                    grid[curX][curY - 1].cost = 10;
                    grid[curX][curY - 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY - 1));
                    grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                    grid[curX][curY - 1].open = true;
                    if (grid[curX][curY - 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY - 1]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX][curY - 1]);
                    }
                    templ = grid[curX][curY - 1].total;
                }
                //right
                if (curX > -1 && curX < gridBoundX && curY + 1 > -1 && curY + 1 < gridBoundY  && !grid[curX][curY + 1].closed)
                {
                    grid[curX][curY + 1].parentTile = grid[curX][curY];
                    grid[curX][curY + 1].cost = 10;
                    grid[curX][curY + 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY + 1));
                    grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                    grid[curX][curY + 1].open = true;
                    if (grid[curX][curY + 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY + 1]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX][curY + 1]);
                    }
                    tempr = grid[curX][curY + 1].total;
                }
                if (!targetFound)
                {
                    grid[curX][curY].closed = true;

                    //move the current node to closed list after evaluating all adjacent nodes
                    closedList.Add(grid[curX][curY]);
                    openList.RemoveAt(0);

                    //loop through the rest of the nodes in open list
                    findPathCharger(curX, curY);
                }

                //if target found traverse the shortest path through the close nodes
                //each node is linked to a parent, so the shortest path is the link of parents from the target node
                if (targetFound)
                {
                    tempX = targetX;
                    tempY = targetY;
                    int placeholderX = tempX, placeholderY = tempY;
                    while (true)
                    {
                        if (tempX == startX && tempY == startY)
                        {
                            shortestPath.Reverse();

                            found = true;
                            break;
                        }
                        shortestPath.Add(grid[tempX][tempY]);

                        placeholderX = tempX;
                        placeholderY = tempY;
                        tempX = grid[placeholderX][placeholderY].parentTile.coordX;
                        tempY = grid[placeholderX][placeholderY].parentTile.coordY;

                    }

                }


            }
        }
        
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      
        public void initialFindPath()
        {
            if (enemyExist && targetExist && !targetFound)
            {

                int curX, curY;

                //use the player's current location as the start node
                curX = (int)grid[3][0].center.Y / 50;
                curY = (int)grid[3][0].center.X / 50;
                startX = curX;
                startY = curY;
                grid[curX][curY].cost = 0;
                grid[curX][curY].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY));
                grid[curX][curY].total = grid[curX][curY].cost + grid[curX][curY].heu;
                grid[curX][curY].open = true;
                openList.Add(grid[curX][curY]);

                //first run through initializing the open lists with all adjacent nodes thats not a wall
                //bot
                if (curX + 1 > -1 && curX + 1 < gridBoundX && curY > -1 && curY < gridBoundY && !grid[curX + 1][curY].blocked && !grid[curX + 1][curY].closed)
                {
                    grid[curX + 1][curY].parentTile = grid[curX][curY];
                    grid[curX + 1][curY].cost = 10;
                    grid[curX + 1][curY].heu = Math.Abs(targetX - (curX + 1)) + Math.Abs(targetY - (curY));
                    grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                    grid[curX + 1][curY].open = true;
                    if (grid[curX + 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX + 1][curY]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX + 1][curY]);
                    }
                    tempb = grid[curX + 1][curY].total;
                }

               
                //top
                if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY && !grid[curX - 1][curY].blocked && !grid[curX - 1][curY].closed)
                {
                    grid[curX - 1][curY].parentTile = grid[curX][curY];
                    grid[curX - 1][curY].cost = 10;
                    grid[curX - 1][curY].heu = Math.Abs(targetX - (curX - 1)) + Math.Abs(targetY - (curY));
                    grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                    grid[curX - 1][curY].open = true;
                    if (grid[curX - 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX - 1][curY]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX - 1][curY]);
                    }
                    tempt = grid[curX - 1][curY].total;
                }
                
                //left
                if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY && !grid[curX][curY - 1].blocked && !grid[curX][curY - 1].closed)
                {
                    grid[curX][curY - 1].parentTile = grid[curX][curY];
                    grid[curX][curY - 1].cost = 10;
                    grid[curX][curY - 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY - 1));
                    grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                    grid[curX][curY - 1].open = true;
                    if (grid[curX][curY - 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY - 1]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX][curY - 1]);
                    }
                    templ = grid[curX][curY - 1].total;
                }
                //right
                if (curX > -1 && curX < gridBoundX && curY + 1 > -1 && curY + 1 < gridBoundY && !grid[curX][curY + 1].blocked && !grid[curX][curY + 1].closed)
                {
                    grid[curX][curY + 1].parentTile = grid[curX][curY];
                    grid[curX][curY + 1].cost = 10;
                    grid[curX][curY + 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY + 1));
                    grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                    grid[curX][curY + 1].open = true;
                    if (grid[curX][curY + 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY + 1]);
                        targetFound = true;
                    }
                    if (!targetFound)
                    {
                        openList.Add(grid[curX][curY + 1]);
                    }
                    tempr = grid[curX][curY + 1].total;
                }
                if (!targetFound)
                {
                    grid[curX][curY].closed = true;

                    //move the current node to closed list after evaluating all adjacent nodes
                    closedList.Add(grid[curX][curY]);
                    openList.RemoveAt(0);

                    //loop through the rest of the nodes in open list
                    findPath(curX, curY);
                }

                //if target found traverse the shortest path through the close nodes
                //each node is linked to a parent, so the shortest path is the link of parents from the target node
                if (targetFound)
                {
                    tempX = targetX;
                    tempY = targetY;
                    int placeholderX = tempX, placeholderY = tempY;
                    while (true)
                    {
                        if (tempX == startX && tempY == startY)
                        {
                            shortestPath.Reverse();

                            found = true;
                            break;
                        }
                        shortestPath.Add(grid[tempX][tempY]);

                        placeholderX = tempX;
                        placeholderY = tempY;
                        tempX = grid[placeholderX][placeholderY].parentTile.coordX;
                        tempY = grid[placeholderX][placeholderY].parentTile.coordY;

                    }

                }


            }
        }


        //changes the mode of the game
        //when one mode is active, all other will be switched off
        public void modeInput()
        {
            keyboard = Keyboard.GetState();

            //allow to create walls
            if (keyboard.IsKeyDown(Keys.W) && !prevkeyboard.IsKeyDown(Keys.W))
            {
                if (wallOn)
                {
                    wallOn = false;
                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = false;
                    pathfinding = false;
                    wallOn = true;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = false;
                }
            }

            //allow to locate player
            if (keyboard.IsKeyDown(Keys.P) && !prevkeyboard.IsKeyDown(Keys.P))
            {
                if (playerOn)
                {
                    playerOn = false;
                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = false;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = true;
                    targetOn = false;
                    enemyOn = false;
                }
            }

            //run the shortest path in A*
            if (keyboard.IsKeyDown(Keys.R) && !prevkeyboard.IsKeyDown(Keys.R))
            {
                if (runPath)
                {
                    runPath = false;
                }
                else
                {
                    turret1Control = false;
                    runPath = true;
                    seek = false;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = false;
                }

            }

            // allow enemy creation
            if (keyboard.IsKeyDown(Keys.E) && !prevkeyboard.IsKeyDown(Keys.E))
            {
                if (enemyOn)
                {
                    enemyOn = false;
                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = false;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = true;
                }
            }

            //allow to create target
            if (keyboard.IsKeyDown(Keys.T) && !prevkeyboard.IsKeyDown(Keys.T))
            {
                if (targetOn)
                {
                    targetOn = false;
                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = false;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = false;
                    targetOn = true;
                    enemyOn = false;
                }
            }

            //run the A* algorithm
            if (keyboard.IsKeyDown(Keys.F) && !prevkeyboard.IsKeyDown(Keys.F))
            {
                if (pathfinding)
                {
                    pathfinding = false;

                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = false;
                    pathfinding = true;
                    wallOn = false;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = false;
                }
            }

            //let all agents on board seek to mouse click
            if (keyboard.IsKeyDown(Keys.S) && !prevkeyboard.IsKeyDown(Keys.S))
            {
                if (seek)
                {
                    seek = false;


                }
                else
                {
                    turret1Control = false;
                    runPath = false;
                    seek = true;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = false;
                }
            }
            if (keyboard.IsKeyDown(Keys.D1) && !prevkeyboard.IsKeyDown(Keys.D1))
            {
                if (turret1Control)
                {
                    turret1Control = false;


                }
                else
                {
                    turret1Control = true;
                    runPath = false;
                    seek = false;
                    pathfinding = false;
                    wallOn = false;
                    playerOn = false;
                    targetOn = false;
                    enemyOn = false;
                }
            }

            //enter debug mode
            if (keyboard.IsKeyDown(Keys.D) && !prevkeyboard.IsKeyDown(Keys.D))
            {
                if (debug)
                {
                    debug = false;
                }
                else
                {
                    debug = true;
                }

            }
            prevkeyboard = Keyboard.GetState();
        }

        public void findPathCharger(int curX, int curY)
        {
            int selectedIndex;
            int lowestTotal = 0;
            while (targetFound == false)
            {
                //find the lowest total cost of all the node in open list and pick it as the selected node
                selectedIndex = 0;
                if (openList.Count > 0)
                {
                    lowestTotal = openList[0].total;
                }

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].total < lowestTotal)
                    {
                        lowestTotal = openList[i].total;
                        selectedIndex = i;
                    }
                }

                if (openList.Count > 0)
                {
                    curX = openList[selectedIndex].coordX;
                    curY = openList[selectedIndex].coordY;
                }


                if (grid[curX][curY].target)
                {
                    closedList.Add(grid[curX][curY]);
                    targetFound = true;

                    break;
                }

                //using the selected node go through all its adjacent node and add to open list unless already added or its a wall.
                //bot
                if (curX + 1 < gridBoundX && curX + 1 > -1 && curY > -1 && curY < gridBoundY  && !grid[curX + 1][curY].closed && !grid[curX + 1][curY].open)
                {
                    grid[curX + 1][curY].parentTile = grid[curX][curY];
                    grid[curX + 1][curY].cost = grid[curX][curY].cost + 10;
                    grid[curX + 1][curY].heu = Math.Abs(targetX - (curX + 1)) + Math.Abs(targetY - (curY));
                    grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                    grid[curX + 1][curY].open = true;

                    //if the adjacent node is the target then add to closed list and break from loop
                    if (grid[curX + 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        grid[curX + 1][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX + 1][curY]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX + 1][curY]);
                }
                else if (curX + 1 < gridBoundX && curX + 1 > -1 && curY > -1 && curY < gridBoundY  && !grid[curX + 1][curY].closed && grid[curX + 1][curY].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX + 1][curY].cost)
                    {
                        grid[curX + 1][curY].parentTile = grid[curX][curY];
                        grid[curX + 1][curY].cost = grid[curX][curY].cost + 10;
                        grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                        openList[openList.IndexOf(grid[curX + 1][curY])] = grid[curX + 1][curY];
                    }
                }

                //top
                if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY  && !grid[curX - 1][curY].closed && !grid[curX - 1][curY].open)
                {
                    grid[curX - 1][curY].parentTile = grid[curX][curY];
                    grid[curX - 1][curY].cost = grid[curX][curY].cost + 10;
                    grid[curX - 1][curY].heu = Math.Abs(targetX - (curX - 1)) + Math.Abs(targetY - (curY));
                    grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                    grid[curX - 1][curY].open = true;
                    if (grid[curX - 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX - 1][curY]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX - 1][curY]);
                }
                else if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY  && !grid[curX - 1][curY].closed && grid[curX - 1][curY].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX - 1][curY].cost)
                    {
                        grid[curX - 1][curY].parentTile = grid[curX][curY];
                        grid[curX - 1][curY].cost = grid[curX][curY].cost + 10;
                        grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                        openList[openList.IndexOf(grid[curX - 1][curY])] = grid[curX - 1][curY];
                    }
                }

                //left
                if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY  && !grid[curX][curY - 1].closed && !grid[curX][curY - 1].open)
                {
                    grid[curX][curY - 1].parentTile = grid[curX][curY];
                    grid[curX][curY - 1].cost = grid[curX][curY].cost + 10;
                    grid[curX][curY - 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY - 1));
                    grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                    grid[curX][curY - 1].open = true;
                    if (grid[curX][curY - 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY - 1]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX][curY - 1]);
                }
                else if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY  && !grid[curX][curY - 1].closed && grid[curX][curY - 1].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX][curY - 1].cost)
                    {
                        grid[curX][curY - 1].parentTile = grid[curX][curY];
                        grid[curX][curY - 1].cost = grid[curX][curY].cost + 10;
                        grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                        openList[openList.IndexOf(grid[curX][curY - 1])] = grid[curX][curY - 1];
                    }
                }
                //right
                if (curX > -1 && curX < gridBoundX && curY + 1 < gridBoundY && curY + 1 > -1 && !grid[curX][curY + 1].closed && !grid[curX][curY + 1].open)
                {
                    grid[curX][curY + 1].parentTile = grid[curX][curY];
                    grid[curX][curY + 1].cost = grid[curX][curY].cost + 10;
                    grid[curX][curY + 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY + 1));
                    grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                    grid[curX][curY + 1].open = true;
                    if (grid[curX][curY + 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY + 1]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX][curY + 1]);
                }
                else if (curX > -1 && curX < gridBoundX && curY + 1 < gridBoundY && curY + 1 > -1  && !grid[curX][curY + 1].closed && grid[curX][curY + 1].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX][curY + 1].cost)
                    {
                        grid[curX][curY + 1].parentTile = grid[curX][curY];
                        grid[curX][curY + 1].cost = grid[curX][curY].cost + 10;
                        grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                        openList[openList.IndexOf(grid[curX][curY + 1])] = grid[curX][curY + 1];
                    }
                }
                grid[curX][curY].closed = true;

                //move the selected node into closed list after inspecting all its adjacent nodes
                closedList.Add(grid[curX][curY]);


                if (openList.Count > 0)
                {
                    //remove the current selected node from open list
                    openList.RemoveAt(selectedIndex);
                }

            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //rest of the A* alorithm loop
        public void findPath(int curX, int curY)
        {
            int selectedIndex;
            int lowestTotal = 0;
            while (targetFound == false)
            {
                //find the lowest total cost of all the node in open list and pick it as the selected node
                selectedIndex = 0;
                if (openList.Count > 0)
                {
                    lowestTotal = openList[0].total;
                }

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].total < lowestTotal)
                    {
                        lowestTotal = openList[i].total;
                        selectedIndex = i;
                    }
                }

                if (openList.Count > 0)
                {
                    curX = openList[selectedIndex].coordX;
                    curY = openList[selectedIndex].coordY;
                }


                if (grid[curX][curY].target)
                {
                    closedList.Add(grid[curX][curY]);
                    targetFound = true;

                    break;
                }

                //using the selected node go through all its adjacent node and add to open list unless already added or its a wall.
                //bot
                if (curX + 1 < gridBoundX && curX + 1 > -1 && curY > -1 && curY < gridBoundY && !grid[curX + 1][curY].blocked && !grid[curX + 1][curY].closed && !grid[curX + 1][curY].open)
                {
                    grid[curX + 1][curY].parentTile = grid[curX][curY];
                    grid[curX + 1][curY].cost = grid[curX][curY].cost + 10;
                    grid[curX + 1][curY].heu = Math.Abs(targetX - (curX + 1)) + Math.Abs(targetY - (curY));
                    grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                    grid[curX + 1][curY].open = true;

                    //if the adjacent node is the target then add to closed list and break from loop
                    if (grid[curX + 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        grid[curX + 1][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX + 1][curY]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX + 1][curY]);
                }
                else if (curX + 1 < gridBoundX && curX + 1 > -1 && curY > -1 && curY < gridBoundY && !grid[curX + 1][curY].blocked && !grid[curX + 1][curY].closed && grid[curX + 1][curY].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX + 1][curY].cost)
                    {
                        grid[curX + 1][curY].parentTile = grid[curX][curY];
                        grid[curX + 1][curY].cost = grid[curX][curY].cost + 10;
                        grid[curX + 1][curY].total = grid[curX + 1][curY].cost + grid[curX + 1][curY].heu;
                        openList[openList.IndexOf(grid[curX + 1][curY])] = grid[curX + 1][curY];
                    }
                }
                
                //top
                if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY && !grid[curX - 1][curY].blocked && !grid[curX - 1][curY].closed && !grid[curX - 1][curY].open)
                {
                    grid[curX - 1][curY].parentTile = grid[curX][curY];
                    grid[curX - 1][curY].cost = grid[curX][curY].cost + 10;
                    grid[curX - 1][curY].heu = Math.Abs(targetX - (curX - 1)) + Math.Abs(targetY - (curY));
                    grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                    grid[curX - 1][curY].open = true;
                    if (grid[curX - 1][curY].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX - 1][curY]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX - 1][curY]);
                }
                else if (curX - 1 > -1 && curX - 1 < gridBoundX && curY > -1 && curY < gridBoundY && !grid[curX - 1][curY].blocked && !grid[curX - 1][curY].closed && grid[curX - 1][curY].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX - 1][curY].cost)
                    {
                        grid[curX - 1][curY].parentTile = grid[curX][curY];
                        grid[curX - 1][curY].cost = grid[curX][curY].cost + 10;
                        grid[curX - 1][curY].total = grid[curX - 1][curY].cost + grid[curX - 1][curY].heu;
                        openList[openList.IndexOf(grid[curX - 1][curY])] = grid[curX - 1][curY];
                    }
                }
                
                //left
                if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY && !grid[curX][curY - 1].blocked && !grid[curX][curY - 1].closed && !grid[curX][curY - 1].open)
                {
                    grid[curX][curY - 1].parentTile = grid[curX][curY];
                    grid[curX][curY - 1].cost = grid[curX][curY].cost + 10;
                    grid[curX][curY - 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY - 1));
                    grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                    grid[curX][curY - 1].open = true;
                    if (grid[curX][curY - 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY - 1]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX][curY - 1]);
                }
                else if (curX > -1 && curX < gridBoundX && curY - 1 > -1 && curY - 1 < gridBoundY && !grid[curX][curY - 1].blocked && !grid[curX][curY - 1].closed && grid[curX][curY - 1].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX][curY - 1].cost)
                    {
                        grid[curX][curY - 1].parentTile = grid[curX][curY];
                        grid[curX][curY - 1].cost = grid[curX][curY].cost + 10;
                        grid[curX][curY - 1].total = grid[curX][curY - 1].cost + grid[curX][curY - 1].heu;
                        openList[openList.IndexOf(grid[curX][curY - 1])] = grid[curX][curY - 1];
                    }
                }
                //right
                if (curX > -1 && curX < gridBoundX && curY + 1 < gridBoundY && curY + 1 > -1 && !grid[curX][curY + 1].blocked && !grid[curX][curY + 1].closed && !grid[curX][curY + 1].open)
                {
                    grid[curX][curY + 1].parentTile = grid[curX][curY];
                    grid[curX][curY + 1].cost = grid[curX][curY].cost + 10;
                    grid[curX][curY + 1].heu = Math.Abs(targetX - (curX)) + Math.Abs(targetY - (curY + 1));
                    grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                    grid[curX][curY + 1].open = true;
                    if (grid[curX][curY + 1].target)
                    {
                        grid[curX][curY].closed = true;
                        closedList.Add(grid[curX][curY]);
                        closedList.Add(grid[curX][curY + 1]);
                        targetFound = true;

                        break;
                    }
                    openList.Add(grid[curX][curY + 1]);
                }
                else if (curX > -1 && curX < gridBoundX && curY + 1 < gridBoundY && curY + 1 > -1 && !grid[curX][curY + 1].blocked && !grid[curX][curY + 1].closed && grid[curX][curY + 1].open)
                {
                    if (grid[curX][curY].cost + 10 < grid[curX][curY + 1].cost)
                    {
                        grid[curX][curY + 1].parentTile = grid[curX][curY];
                        grid[curX][curY + 1].cost = grid[curX][curY].cost + 10;
                        grid[curX][curY + 1].total = grid[curX][curY + 1].cost + grid[curX][curY + 1].heu;
                        openList[openList.IndexOf(grid[curX][curY + 1])] = grid[curX][curY + 1];
                    }
                }
                grid[curX][curY].closed = true;

                //move the selected node into closed list after inspecting all its adjacent nodes
                closedList.Add(grid[curX][curY]);


                if (openList.Count > 0)
                {
                    //remove the current selected node from open list
                    openList.RemoveAt(selectedIndex);
                }

            }

        }
    }
}
