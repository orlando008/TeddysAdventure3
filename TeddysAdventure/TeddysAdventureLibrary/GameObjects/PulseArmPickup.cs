using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class PulseArmPickup : GameObject
    {
        public PulseArmPickup(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\PulseArm");
          
            BoxToDraw = new Rectangle(28, 40, 22, 17);
        }
    }
}
