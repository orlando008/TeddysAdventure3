using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Vacuum : Enemy
    {
        private Texture2D _coneSprites;
        private Texture2D _stuffSprites;
        private int _coneFrameCount;
        private bool _isExtendingCone;
        private bool _isRetractingCone;
        private bool _isSucking;
        private Rectangle _coneBoxToDraw;
        private double _elapsedSeconds;

        public Vacuum(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\Vacuum");
            _coneSprites = game.Content.Load<Texture2D>("Enemies\\VacuumCone");
            _stuffSprites = game.Content.Load<Texture2D>("Enemies\\VacuumStuff");

            Position = position;
            BoxToDraw = new Rectangle(0, 0,70, 129);
            base.Velocity = velocity;
            CanJumpOnToKill = false;
            _gravity = 0;
        }

        public override void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
            Color enemyColor = Color.White;

            if (this.Dying)
            {
                enemyColor.A = (byte)((1 - ((float)_deathFrameCount / _deathFrames)) * 255);
            }


            if (!Destroyed)
            {

                sp.Draw(StyleSheet, Position, BoxToDraw, enemyColor);

            }

            if (_isExtendingCone || _isSucking || _isRetractingCone)
            {
                sp.Draw(_coneSprites, new Vector2(Position.X - _coneBoxToDraw.Width + 5, Position.Y + 85), _coneBoxToDraw, enemyColor);
            }

        }

        public override void Update(GameTime gameTime)
        {
            _elapsedSeconds += gameTime.ElapsedGameTime.Milliseconds;

            if (_elapsedSeconds > 7000)
            {
                _isExtendingCone = true;
                _elapsedSeconds = 0;
            }

            if (Destroyed)
                return;

            Screen currentScreen = (Screen)Game.Components[0];

            if (_isExtendingCone || _isRetractingCone || _isSucking)
            {
                Velocity = new Vector2(0, 0);
            }
            else
            {
                if (currentScreen.Teddy.Position.X < this.Position.X)
                {
                    Velocity = new Vector2(-1, Velocity.Y);
                }
                else if (currentScreen.Teddy.Position.X > this.Position.X)
                {
                    Velocity = new Vector2(1, Velocity.Y);
                }

                if (_frameCount <= 10)
                {
                    BoxToDraw = new Rectangle(0, 0, 71, 129);
                }
                else if (_frameCount <= 20)
                {
                    BoxToDraw = new Rectangle(71, 0, 71, 129);
                }
                else
                {
                    _frameCount = 0;
                }

                _frameCount++;
            }




            if (_isExtendingCone)
            {
                if (_coneFrameCount <= 50)
                {
                    _coneBoxToDraw = new Rectangle(0, 0, 57, 45);
                }
                else if (_coneFrameCount <= 100)
                {
                    _coneBoxToDraw = new Rectangle(58, 0, 57, 45);
                }
                else if (_coneFrameCount <= 150)
                {
                    _coneBoxToDraw = new Rectangle(116, 0, 57, 45);
                }
                else
                {
                    _isExtendingCone = false;
                    _isSucking = true;
                    _coneFrameCount = 0;
                }
                
                _coneFrameCount++;
            }
            else if (_isSucking)
            {
                if (_coneFrameCount <= 10)
                {

                }
                else if (_coneFrameCount <= 100)
                {
                    
                }
                else if (_coneFrameCount <= 150)
                {
                    _isRetractingCone = true;
                    _isSucking = false;
                    _coneFrameCount = 0;
                }

                _coneFrameCount++;
            }
            else if (_isRetractingCone)
            {
                if (_coneFrameCount <= 50)
                {
                    _coneBoxToDraw = new Rectangle(116, 0, 57, 45);
                }
                else if (_coneFrameCount <= 100)
                {
                    _coneBoxToDraw = new Rectangle(58, 0, 57, 45);
                }
                else if (_coneFrameCount <= 150)
                {
                    _coneBoxToDraw = new Rectangle(0, 0, 57, 45);
                }
                else
                {
                    _isExtendingCone = false;
                    _isSucking = false;
                    _isRetractingCone = false;
                    _coneFrameCount = 0;
                    _elapsedSeconds = 0;
                }
                _coneFrameCount++;
            }

            base.Update(gameTime);
        }
    }
}
