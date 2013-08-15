using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class GameObjectHelper
    {
        private string _enemyType;
        private Vector2 _position;
        private Vector2 _velocity;

        public string Type
        {
            get { return _enemyType; }
            set { _enemyType = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public GameObjectHelper()
        {

        }
    }

    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class GameObjectReader : ContentTypeReader<GameObjectHelper>
    {
        protected override GameObjectHelper Read(ContentReader input, GameObjectHelper existingInstance)
        {
            GameObjectHelper gobjHelper = existingInstance;

            if (gobjHelper == null)
            {
                gobjHelper = new GameObjectHelper();
            }

            gobjHelper.Type = input.ReadString();
            gobjHelper.Position = input.ReadVector2();
            return gobjHelper;
        }
    }
}
