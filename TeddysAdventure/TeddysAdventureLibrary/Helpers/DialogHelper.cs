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

        public static void ShowDialogBubble(Game g, string dialogText, SpriteFont font, SpriteBatch sp, Rectangle entitySpeaking, bool preferLeft, bool preferTop)
        {
            Vector2 sizeOfString = font.MeasureString(dialogText);
            Vector2 positionOfString = new Vector2(0,0);
            int padding = 5;
            Rectangle cameraBounds = ((Screen)g.Components[0]).CameraBounds;

            //See if text will fit to the left of entity
            if (preferLeft)
            {
                if (entitySpeaking.Left - sizeOfString.X > 0)
                {
                    positionOfString = new Vector2(entitySpeaking.Left - sizeOfString.X, 0);
                }
                else
                {
                    positionOfString = new Vector2(entitySpeaking.Right, 0);
                }
            }
            else
            {
                if (entitySpeaking.Right + sizeOfString.X < cameraBounds.Width)
                {
                    positionOfString = new Vector2(entitySpeaking.Right + sizeOfString.X, 0);
                }
                else
                {
                    positionOfString = new Vector2(entitySpeaking.Left - sizeOfString.X, 0);
                }
            }

            if (preferTop)
            {
                if (entitySpeaking.Top - sizeOfString.Y > 0)
                {
                    positionOfString = new Vector2(positionOfString.X, entitySpeaking.Top - sizeOfString.Y);
                }
                else
                {
                    positionOfString = new Vector2(positionOfString.X, entitySpeaking.Bottom);
                }
            }
            else
            {
                if (entitySpeaking.Bottom + sizeOfString.Y < cameraBounds.Height)
                {
                    positionOfString = new Vector2(positionOfString.X, entitySpeaking.Bottom);
                }
                else
                {
                    positionOfString = new Vector2(positionOfString.X, entitySpeaking.Top - sizeOfString.Y);
                }
            }

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
