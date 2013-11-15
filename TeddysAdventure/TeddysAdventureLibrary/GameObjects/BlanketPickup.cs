using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    class BlanketPickup: GameObject
    {
        public BlanketPickup(Game game, Vector2 position)
            : base(game, position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\BlanketPickup");

            BoxToDraw = new Rectangle(0, 0, 58, 33);
        }


    }
}
