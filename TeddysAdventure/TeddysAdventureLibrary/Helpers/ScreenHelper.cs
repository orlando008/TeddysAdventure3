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

        private List<SurfaceHelper> _surfaces;
        private List<GameObjectHelper> _gameObjectLocations;
        private List<EnemyHelper> _listOfEnemies;
        private string _levelType;
        private Vector2 _levelSize;



        public List<SurfaceHelper> Surfaces
        {
            get { return _surfaces; }
            set { _surfaces = value; }
        }

        public List<GameObjectHelper> ListOfObjects
        {
            get { return _gameObjectLocations; }
            set { _gameObjectLocations = value; }
        }

        public List<EnemyHelper> ListOfEnemies
        {
            get { return _listOfEnemies; }
            set{_listOfEnemies = value;}
        }

        public string LevelType
        {
            get { return _levelType; }
            set { _levelType = value; }
        }

        public Vector2 LevelSize
        {
            get { return _levelSize; }
            set { _levelSize = value; }
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

            scrn.Surfaces = input.ReadObject<List<SurfaceHelper>>();
            scrn.ListOfObjects = input.ReadObject<List<GameObjectHelper>>();
            scrn.ListOfEnemies = input.ReadObject<List<EnemyHelper>>();
            scrn.LevelType = input.ReadString();
            scrn.LevelSize = input.ReadVector2();

            return scrn;
        }
    }
}
