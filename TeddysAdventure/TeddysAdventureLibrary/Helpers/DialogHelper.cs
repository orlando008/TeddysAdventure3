using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary.Helpers
{
    public static class DialogHelper
    {
        public static Texture2D SimpleTexture;

        public static void ShowDialogBubble(Game g, string dialogText, SpriteFont font, Vector2 positionOfString, SpriteBatch sp)
        {
            Vector2 sizeOfString = font.MeasureString(dialogText);
            int padding = 5;

            if (SimpleTexture == null)
            {
                SimpleTexture = new Texture2D(g.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                SimpleTexture.SetData(new[] { Color.White });
            }

            sp.DrawString(font, dialogText, positionOfString, Color.Black);

            sp.Draw(SimpleTexture, new Rectangle((int)positionOfString.X - padding, (int)positionOfString.Y - padding, (int)sizeOfString.X + padding * 2, 1), Color.Black);
            sp.Draw(SimpleTexture, new Rectangle((int)positionOfString.X - padding, (int)positionOfString.Y + (int)sizeOfString.Y + 5, (int)sizeOfString.X + padding * 2, 1), Color.Black);
            sp.Draw(SimpleTexture, new Rectangle((int)positionOfString.X - padding, (int)positionOfString.Y - padding, 1, (int)sizeOfString.Y + padding * 2), Color.Black);
            sp.Draw(SimpleTexture, new Rectangle((int)positionOfString.X + (int)sizeOfString.X + padding, (int)positionOfString.Y - padding, 1, (int)sizeOfString.Y + padding * 2), Color.Black);
        }
    }
}
