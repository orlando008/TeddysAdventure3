using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace TeddysAdventureLibrary
{
    public class SurfaceHelper
    {

        public Rectangle Rect { get; set; }
        public string Sprite { get; set; }

        public SurfaceHelper() { }


    }

    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class SurfaceReader : ContentTypeReader<SurfaceHelper>
    {
        protected override SurfaceHelper Read(ContentReader input, SurfaceHelper existingInstance)
        {
            SurfaceHelper sHelper = existingInstance;

            if (sHelper == null)
            {
                sHelper = new SurfaceHelper();
            }

            sHelper.Rect = input.ReadObject<Rectangle>();
            sHelper.Sprite = input.ReadString();
            return sHelper;
        }
    }

}
