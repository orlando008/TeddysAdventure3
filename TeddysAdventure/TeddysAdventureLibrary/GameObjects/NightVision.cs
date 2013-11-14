using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class NightVision : GameObject
    {
        public NightVision(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\NightVision");
          
            BoxToDraw = new Rectangle(50, 0, 50, 44);
        }
    }
}
