using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class Background
    {

        public string BackgroundName { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public bool Scrolls { get; set; }
        public Vector2 Offset { get; set; }

        public Background(BackgroundHelper bh)
        {
            this.BackgroundName = bh.Image;
            this.RepeatX = bh.RepeatX;
            this.RepeatY = bh.RepeatY;
            this.Scrolls = bh.Scrolls;
            this.Offset = bh.Offset;
        }

    }


    public class BackgroundHelper
    {

        public string Image { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public bool Scrolls { get; set; }
        public Vector2 Offset { get; set; }

        public BackgroundHelper() { }


    }

    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class BackgroundReader : ContentTypeReader<BackgroundHelper>
    {
        protected override BackgroundHelper Read(ContentReader input, BackgroundHelper existingInstance)
        {
            BackgroundHelper sHelper = existingInstance;

            if (sHelper == null)
            {
                sHelper = new BackgroundHelper();
            }

            sHelper.Image = input.ReadString();
            sHelper.RepeatX = input.ReadBoolean();
            sHelper.RepeatY = input.ReadBoolean();
            sHelper.Scrolls = input.ReadBoolean();
            sHelper.Offset = input.ReadObject<Vector2>();

            return sHelper;
        }
    }


}
