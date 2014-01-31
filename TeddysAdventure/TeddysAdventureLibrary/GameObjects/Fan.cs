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
        private long _elapsedMilliseconds = 0;
        private long _millisecondsPerFrame = 55;
        private int _width = 50;
        private int _height = 75;

        public Fan(Game game, Vector2 position, Enumerations.Direction direction)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\Fan");

            BoxToDraw = new Rectangle(0, 0, _width, _height);

            _blowDirection = direction;
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

            base.Update(gameTime);
        }
    }
}
