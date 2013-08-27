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
    public class EnemyHelper
    {
        private string _enemyType;
        private Vector2 _position;
        private Vector2 _velocity;
        private bool _isSpawnPoint;
        private int _spawnInterval;

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

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public bool IsSpawnPoint
        {
            get { return _isSpawnPoint; }
            set { _isSpawnPoint = value; }
        }

        public int SpawnInterval
        {
            get { return _spawnInterval; }
            set { _spawnInterval = value; }
        }

        public EnemyHelper()
        {

        }
    }


    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class EnemyReader : ContentTypeReader<EnemyHelper>
    {
        protected override EnemyHelper Read(ContentReader input, EnemyHelper existingInstance)
        {
            EnemyHelper enemyHelper = existingInstance;

            if (enemyHelper == null)
            {
                enemyHelper = new EnemyHelper();
            }

            enemyHelper.Type = input.ReadString();
            enemyHelper.Position = input.ReadVector2();
            enemyHelper.Velocity = input.ReadVector2();
            enemyHelper.IsSpawnPoint = input.ReadBoolean();
            enemyHelper.SpawnInterval = input.ReadInt32();
            return enemyHelper;
        }
    }
}
