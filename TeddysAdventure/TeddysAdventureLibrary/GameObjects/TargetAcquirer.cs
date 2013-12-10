using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class TargetAcquirer : GameObject
    {
        public TargetAcquirer(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\TargetAcquirer");
          
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
        }
    }
}
