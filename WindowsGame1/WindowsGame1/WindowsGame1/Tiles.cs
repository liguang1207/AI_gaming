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
    class Tiles
    {
        public Vector2 center;
        public int heu;
        public int cost;
        public int total;
        public Texture2D tiles;
        public bool blocked;
        public bool target;
        public bool path = false;
        public bool closed = false;
        public bool open = false;
        public Rectangle rec;
        public Tiles parentTile;
        public int coordX=0;
        public int coordY=0;

        public Tiles(Vector2 Center, bool Blocked, Rectangle Rec, Texture2D Tiles, int X, int Y)
        {
            center = Center;
            blocked = Blocked;
            rec = Rec;
            tiles = Tiles;
            target = false;
            coordX = X;
            coordY = Y;
        }

        //change the tiles of the grid to be blocked(a wall) if not already
        //if the tile is already a wall then change back to normal tile
        public void ifBlocked(bool b, Texture2D wallTile, Texture2D normTile)
        {
            if (b&& blocked)
            {
                blocked = false;
                tiles = normTile;
            }
            else if(b)
            {
                blocked = true;
                tiles = wallTile;
            }
        }

        //change the tile as a target if not already
        //if tile is already a target then change back to normal tile
        public void ifTarget(bool b, Texture2D targetTile, Texture2D normTile)
        {
            if (b && target)
            {
                target = false;
                tiles = normTile;
            }
            else if (b)
            {
                target = true;
                tiles = targetTile;
            }
        }
    }
}
