using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Fan : GameObject
    {
        private Enumerations.Direction _blowDirection;

        public Enumerations.Direction BlowDirection
        {
            get { return _blowDirection; }
            set 
            { 
                _blowDirection = value;

                switch (_blowDirection)
                {
                    case Enumerations.Direction.Up:
                        _affectedArea = new Rectangle((int)this.Position.X, (int)this.Position.Y - (BoxToDraw.Height * 3), BoxToDraw.Width, BoxToDraw.Height * 3);
                        break;
                    case Enumerations.Direction.Down:
                        _affectedArea = new Rectangle((int)this.Position.X, (int)this.Position.Y + BoxToDraw.Height, BoxToDraw.Width, BoxToDraw.Height * 3);
                        break;
                    case Enumerations.Direction.Left:
                        _affectedArea = new Rectangle((int)this.Position.X - (BoxToDraw.Height * 3), (int)this.Position.Y, BoxToDraw.Height * 3, BoxToDraw.Height);
                        break;
                    case Enumerations.Direction.Right:
                        _affectedArea = new Rectangle((int)this.Position.X + BoxToDraw.Width, (int)this.Position.Y, BoxToDraw.Height * 3, BoxToDraw.Height);
                        break;
                }
                
            }
        }
        private long _elapsedMilliseconds = 0;
        private long _millisecondsPerFrame = 55;
        private int _width = 50;
        private int _height = 75;
        private Rectangle _affectedArea;

        public Fan(Game game, Vector2 position, Enumerations.Direction direction)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fan");

            BoxToDraw = new Rectangle(0, 0, _width, _height);

            BlowDirection = direction;
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

            if (_elapsedMilliseconds <= _millisecondsPerFrame)
                BoxToDraw = new Rectangle(0, 0, _width, _height);
            else if (_elapsedMilliseconds <= _millisecondsPerFrame * 2)
                BoxToDraw = new Rectangle(_width, 0, _width, _height);
            else if (_elapsedMilliseconds <= _millisecondsPerFrame * 3)
                BoxToDraw = new Rectangle(_width * 2, 0, _width, _height);
            else if (_elapsedMilliseconds <= _millisecondsPerFrame * 4)
                BoxToDraw = new Rectangle(_width * 3, 0, _width, _height);
            else
                _elapsedMilliseconds = 0;

            if (((Screen)(Game.Components[0])).Teddy.TeddyRectangle.Intersects(_affectedArea))
            {
                if (((Screen)(Game.Components[0])).Teddy.Facing == Teddy.Direction.Right)
                {
                    ((Screen)(Game.Components[0])).Teddy.SpeedModifier = 1.3f;
                }
                else
                {
                    ((Screen)(Game.Components[0])).Teddy.SpeedModifier = .3f;
                }
                
            }
            else
            {
                ((Screen)(Game.Components[0])).Teddy.SpeedModifier = 1f;
            }

            base.Update(gameTime);
        }
    }
}
