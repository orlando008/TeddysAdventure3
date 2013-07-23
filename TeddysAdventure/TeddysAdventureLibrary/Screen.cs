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

namespace TeddysAdventureLibrary
{
    public class Screen : IGameComponent
    {

        private string filename;
        private Texture2D sprite;

        [ContentSerializerIgnore]
        public static SpriteBatch spriteBatch;
        
        /// <summary>
        /// The filename of the stylesheet used for this screen
        /// </summary>
        public string StyleSheet
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Sprite
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Screen()
        {

        }

        public static void LoadScreen(string screenName, Game g)
        {
            Screen nuScreen = g.Content.Load<Screen>(screenName);
            g.Components.Add(nuScreen);
        }

        #region IGameComponent Members

        void IGameComponent.Initialize()
        {

        }

        #endregion

        /// <summary>
        /// Read a Map object from the content pipeline.
        /// </summary>
        public class ScreenReader : ContentTypeReader<Screen>
        {
            protected override Screen Read(ContentReader input, Screen existingInstance)
            {
                Screen scrn = existingInstance;
                if (scrn == null)
                {
                    scrn = new Screen();
                }

                scrn.StyleSheet = input.ReadString();
                scrn.Sprite = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine(@"Screens", scrn.StyleSheet));
                
                return scrn;
            }
        }
    }
}
