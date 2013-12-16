using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class TargetAcquirer : GameObject
    {
        Vector2 _positionUpperLeft;
        Vector2 _positionUpperRight;
        Vector2 _positionBottomLeft;
        Vector2 _positionBottomRight;
        Vector2 _positionCenterCross;
        Vector2 _targetPosition;
        Vector2 _targetPositionOfBottomRight;
        int _speed = 6;
        bool _lockedIn;
        int _heightOfCorner = 36;
        int _widthOfCorner = 23;
        int _widthOfCross = 12;
        int _heightOfCross = 13;
        int _thresholdOfCloseness = 20;
        long _secondsLockedIn;

        public bool LockedIn
        {
            get { return _lockedIn; }
            set 
            {
                if(value != _lockedIn)
                {
                    _secondsLockedIn = 0;
                    _lockedIn = value; 
                }
            }
        }

        public TargetAcquirer(Game game, Vector2 position)
            : base(game,position)
        {
            StyleSheet = game.Content.Load<Texture2D>("Objects\\TargetAcquirer");
          
            BoxToDraw = new Rectangle(0, 0, StyleSheet.Width, StyleSheet.Height);
            _targetPosition = position;
            ResetPositions();
        }

        public void ResetPositions()
        {
            _targetPositionOfBottomRight = new Vector2(Position.X + 26, Position.Y + 38);
            _positionUpperLeft = new Vector2(((Screen)Game.Components[0]).CameraBounds.X, 0);
            _positionUpperRight = new Vector2(((Screen)Game.Components[0]).CameraBounds.X + ((Screen)Game.Components[0]).CameraBounds.Width, 0);
            _positionBottomLeft = new Vector2(((Screen)Game.Components[0]).CameraBounds.X, ((Screen)Game.Components[0]).CameraBounds.Y + ((Screen)Game.Components[0]).CameraBounds.Height);
            _positionBottomRight = new Vector2(((Screen)Game.Components[0]).CameraBounds.X + ((Screen)Game.Components[0]).CameraBounds.Width, ((Screen)Game.Components[0]).CameraBounds.Y + ((Screen)Game.Components[0]).CameraBounds.Height);
            _positionCenterCross = new Vector2(Position.X, (((Screen)Game.Components[0]).CameraBounds.Y + ((Screen)Game.Components[0]).CameraBounds.Height) / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_lockedIn)
            {
                //upper left corner
                if (_targetPosition.X > _positionUpperLeft.X)
                    _positionUpperLeft.X = _positionUpperLeft.X + _speed;
                else if (_targetPosition.X < _positionUpperLeft.X)
                    _positionUpperLeft.X = _positionUpperLeft.X - _speed;

                if (_targetPosition.Y > _positionUpperLeft.Y)
                    _positionUpperLeft.Y = _positionUpperLeft.Y + _speed;
                else if (_targetPosition.Y < _positionUpperLeft.Y)
                    _positionUpperLeft.Y = _positionUpperLeft.Y - _speed;

                //upper right corner
                if (_targetPosition.X + 26 > _positionUpperRight.X)
                    _positionUpperRight.X = _positionUpperRight.X + _speed;
                else if (_targetPosition.X + 26 < _positionUpperRight.X)
                    _positionUpperRight.X = _positionUpperRight.X - _speed;

                if (_targetPosition.Y > _positionUpperRight.Y)
                    _positionUpperRight.Y = _positionUpperRight.Y + _speed;
                else if (_targetPosition.Y < _positionUpperRight.Y)
                    _positionUpperRight.Y = _positionUpperRight.Y - _speed;

                //bottom left corner
                if (_targetPosition.X > _positionBottomLeft.X)
                    _positionBottomLeft.X = _positionBottomLeft.X + _speed;
                else if (_targetPosition.X < _positionBottomLeft.X)
                    _positionBottomLeft.X = _positionBottomLeft.X - _speed;

                if (_targetPosition.Y + 38 > _positionBottomLeft.Y)
                    _positionBottomLeft.Y = _positionBottomLeft.Y + _speed;
                else if (_targetPosition.Y + 38 < _positionBottomLeft.Y)
                    _positionBottomLeft.Y = _positionBottomLeft.Y - _speed;

                //bottom right corner
                if (_targetPosition.X + 26 > _positionBottomRight.X)
                    _positionBottomRight.X = _positionBottomRight.X + _speed;
                else if (_targetPosition.X + 26 < _positionBottomRight.X)
                    _positionBottomRight.X = _positionBottomRight.X - _speed;

                if (_targetPosition.Y + 38 > _positionBottomRight.Y)
                    _positionBottomRight.Y = _positionBottomRight.Y + _speed;
                else if (_targetPosition.Y + 38 < _positionBottomRight.Y)
                    _positionBottomRight.Y = _positionBottomRight.Y - _speed;

                //center cross
                if (_targetPosition.X + 19 > _positionCenterCross.X)
                    _positionCenterCross.X = _positionCenterCross.X + _speed;
                else if (_targetPosition.X + 19 < _positionCenterCross.X)
                    _positionCenterCross.X = _positionCenterCross.X - _speed;

                if (_targetPosition.Y + 31 > _positionCenterCross.Y)
                    _positionCenterCross.Y = _positionCenterCross.Y + _speed;
                else if (_targetPosition.Y + 31 < _positionCenterCross.Y)
                    _positionCenterCross.Y = _positionCenterCross.Y - _speed;
            }
            else
            {
                _secondsLockedIn += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (closeEnoughToLockIn())
                LockedIn = true;

            base.Update(gameTime);
        }

        private bool closeEnoughToLockIn()
        {
            if (Math.Abs(Vector2.Subtract(_positionBottomRight, _targetPositionOfBottomRight).X) < _thresholdOfCloseness)
                if (Math.Abs(Vector2.Subtract(_positionBottomRight, _targetPositionOfBottomRight).Y) < _thresholdOfCloseness)
                    if (Math.Abs(Vector2.Subtract(_positionUpperLeft, _targetPosition).X) < _thresholdOfCloseness)
                        if (Math.Abs(Vector2.Subtract(_positionUpperLeft, _targetPosition).Y) < _thresholdOfCloseness)
                            return true;
                

            return false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch sp)
        {
            if (_lockedIn)
            {
                if (_secondsLockedIn > 500)
                {
                    //sp.Draw(StyleSheet, Position, new Color(255, 255, 255, 5));

                    if(_secondsLockedIn > 1000)
                        _secondsLockedIn = 0;
                }
                else
                {
                    sp.Draw(StyleSheet, Position, new Color(255, 255, 255, 255));
                }
               
            }
            else
            {
                sp.Draw(StyleSheet, new Rectangle((int)_positionUpperLeft.X, (int)_positionUpperLeft.Y, _widthOfCorner, _heightOfCorner), new Rectangle(0, 0, _widthOfCorner, _heightOfCorner), Color.White);
                sp.Draw(StyleSheet, new Rectangle((int)_positionUpperRight.X, (int)_positionUpperRight.Y, _widthOfCorner, _heightOfCorner), new Rectangle(26, 0, _widthOfCorner, _heightOfCorner), Color.White);
                sp.Draw(StyleSheet, new Rectangle((int)_positionBottomLeft.X, (int)_positionBottomLeft.Y, _widthOfCorner, _heightOfCorner), new Rectangle(0, 38, _widthOfCorner, _heightOfCorner), Color.White);
                sp.Draw(StyleSheet, new Rectangle((int)_positionBottomRight.X, (int)_positionBottomRight.Y, _widthOfCorner, _heightOfCorner), new Rectangle(26, 38, _widthOfCorner, _heightOfCorner), Color.White);
                sp.Draw(StyleSheet, new Rectangle((int)_positionCenterCross.X, (int)_positionCenterCross.Y, _widthOfCross, _heightOfCross), new Rectangle(19, 31, _widthOfCross, _heightOfCross), Color.White);
            }
        }
    }
}
