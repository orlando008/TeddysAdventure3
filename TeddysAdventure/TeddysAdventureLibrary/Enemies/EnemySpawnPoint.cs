using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class EnemySpawnPoint : Enemy
    {

        
        private int _interval;
        private string _enemyType;
        private int _millisecondsElapsed = 0;

        public EnemySpawnPoint(Game game, Vector2 position, string enemyType, int interval, Vector2 velocity)
            : base(game)
        {
            this.Position = position;
            this.Velocity = velocity;
            _enemyType = enemyType;
            _interval = interval;
            _playerCanPassThrough = true;
        }

        public override void Update(GameTime gameTime)
        {
            _millisecondsElapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (_millisecondsElapsed > _interval)
            {
                spawnEnemy();
                _millisecondsElapsed = 0;
            }

            if (ChildrenEnemies != null)
            {
                foreach (Enemy e in ChildrenEnemies)
                {
                    e.Update(gameTime);
                }
            }

        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            if (ChildrenEnemies != null)
            {
                foreach (Enemy e in ChildrenEnemies)
                {
                    e.DrawEnemy(gameTime, sp);
                }
            }

        }

        private void spawnEnemy()
        {
            if (ChildrenEnemies == null)
            {
                ChildrenEnemies = new List<Enemy>();
            }

            Enemy newEnemy = FindDeadEnemy();
            if (newEnemy == null)
            {
                switch (_enemyType)
                {
                    case "BowlingBall":
                        newEnemy = new BowlingBall(Game, Position, Velocity);
                        break;
                    case "MatchBoxCar":
                        newEnemy = new MatchBoxCar(Game, Position, Velocity);
                        break;
                    case "FlyingBook":
                        newEnemy = new FlyingBook(Game, Position, Velocity);
                        break;
                    case "Airplane":
                        newEnemy = new PlaneEnemy(Game, Position, Velocity);
                        break;
                    case "LadyBug":
                        newEnemy = new LadyBug(Game, Position, Velocity);
                        break;
                    case "OrangeBomb":
                        newEnemy = new OrangeBomb(Game, Position, Velocity);
                        break;
                    case "OrangeCrayon":
                        newEnemy = new OrangeCrayon(Game, Position, Velocity);
                        break;
                    case "Eagle":
                        newEnemy = new Eagle(Game, Position, Velocity);
                        break;
                    case "DustBunny":
                        newEnemy = new DustBunny(Game, Position, Velocity);
                        break;
                    case "Vacuum":
                        newEnemy = new Vacuum(Game, Position, Velocity);
                        break;
                }

                ChildrenEnemies.Add(newEnemy);
            }
            else
            {
                newEnemy.BringToLife();
            }

            newEnemy.Velocity = this.Velocity;
            newEnemy.Position = this.Position; 

        }

        //if we have a dead enemy use that rather than taking up more memory and creating a brand new one
        private Enemy FindDeadEnemy()
        {
            for (int i = 0; i < ChildrenEnemies.Count - 1; i++)
            {
                if (ChildrenEnemies[i].Destroyed)
                    return ChildrenEnemies[i];
            }

            return null;
        }
    }
}
