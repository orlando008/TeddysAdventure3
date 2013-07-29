using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class ScreenHelper : IGameComponent
    {

        public void Initialize()
        {
            
        }

        private string[] _assets;
        private int[] _positions;
        private List<Rectangle> _surfaces;
        private List<Vector2> _fluffLocations;

        public string[] Assets
        {
            get { return _assets; }
            set { _assets = value; }
        }

        public int[] Positions
        {
            get { return _positions; }
            set { _positions = value; }
        }

        public List<Rectangle> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }

        public List<Vector2> FluffLocations
        {
            get { return _fluffLocations; }
            set { _fluffLocations = value; }
        }

        public ScreenHelper()
        {

        }

        public static void LoadScreenHelper(string screenName, Game g)
        {
            ScreenHelper sh = g.Content.Load<ScreenHelper>(screenName);
        }

    }

    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class ScreenReader : ContentTypeReader<ScreenHelper>
    {
        protected override ScreenHelper Read(ContentReader input, ScreenHelper existingInstance)
        {
            ScreenHelper scrn = existingInstance;
            if (scrn == null)
            {
                scrn = new ScreenHelper();
            }

            scrn.Assets = input.ReadObject<string[]>();
            scrn.Positions = input.ReadObject<int[]>();
            scrn.Surfaces = input.ReadObject<List<Rectangle>>();
            scrn.FluffLocations = input.ReadObject<List<Vector2>>();
            
            return scrn;
        }
    }
}
