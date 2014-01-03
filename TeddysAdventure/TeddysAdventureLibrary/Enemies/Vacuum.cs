using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeddysAdventureLibrary.Helpers;

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
        private Rectangle _stuffBoxToDraw;
        private int _currentStuffLevel = 0;
        private double _elapsedSeconds;
        private int _numberOfSucksPerformed = 0;
        private int _numberOfSucksToPerform = 10;
        private Texture2D _targetAcquirerSprites;
        private Vector2 _targetPosition = new Vector2(-999, -999);
        private TargetAcquirer _targetAcquirer;
        private int _widthOfCone = 61;
        private SpriteFont _font;

        public Vacuum(Game game, Vector2 position, Vector2 velocity)
            : base(game)
        {
            StyleSheet = game.Content.Load<Texture2D>("Enemies\\Vacuum");
            _coneSprites = game.Content.Load<Texture2D>("Enemies\\VacuumCone");
            _stuffSprites = game.Content.Load<Texture2D>("Enemies\\VacuumStuff");
            _targetAcquirerSprites = game.Content.Load<Texture2D>("Objects\\TargetAcquirer");
            _font = game.Content.Load<SpriteFont>("Fonts\\Arial12");

            HudIcon = game.Content.Load<Texture2D>("Enemies\\VacuumIcon");

            SpecialNamedEnemy = true;
            Name = "Victor The Vacuum";

            Health = 10;
            MaxHealth = 10;

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

            base.DrawDamage(gameTime, sp);

            if (_isSucking && _currentStuffLevel > 0)
            {
                switch (_currentStuffLevel)
                {
                    case 1:
                        sp.Draw(_stuffSprites, new Vector2(Position.X - _coneBoxToDraw.Width + 8, Position.Y + 117), _stuffBoxToDraw, enemyColor);
                        break;
                    case 2:
                        sp.Draw(_stuffSprites, new Vector2(Position.X - _coneBoxToDraw.Width + 15, Position.Y + 99), _stuffBoxToDraw, enemyColor);
                        break;
                    case 3:
                        sp.Draw(_stuffSprites, new Vector2(Position.X - _coneBoxToDraw.Width + 24, Position.Y + 90), _stuffBoxToDraw, enemyColor);
                        break;
                }
                
            }

            if (_isExtendingCone || _isSucking || _isRetractingCone)
            {
                sp.Draw(_coneSprites, new Vector2(Position.X - _coneBoxToDraw.Width + 5, Position.Y + 85), _coneBoxToDraw, enemyColor);
            }

            if (ChildrenEnemies != null)
            {
                foreach (Enemy childEnemy in ChildrenEnemies)
                {
                    childEnemy.DrawEnemy(gameTime, sp);
                }
            }


            DialogHelper.ShowDialogBubble(Game, GetTaunt(), _font, new Vector2(Position.X + BoxToDraw.Width, Position.Y), sp);

        }

        private string GetTaunt()
        {
            return "When I'm through with you there'll be nothing left.";
        }

        public override void Update(GameTime gameTime)
        {
            if (Destroyed)
                return;

            Screen currentScreen = (Screen)Game.Components[0];

            _elapsedSeconds += gameTime.ElapsedGameTime.Milliseconds;

            if (_elapsedSeconds > 2000 && _targetAcquirer == null && _isSucking == false && _isRetractingCone == false)
            {
                _targetPosition = ((Screen)Game.Components[0]).Teddy.Position;
                _targetAcquirer = new TargetAcquirer(Game, ((Screen)Game.Components[0]).Teddy.Position);
                currentScreen.GameObjects.Add(_targetAcquirer);

                if (this.ChildrenEnemies == null)
                {
                    this.ChildrenEnemies = new List<Enemy>();
                }
                
                this.ChildrenEnemies.Add(new LintMissile(Game, new Vector2(this.Position.X + 4, this.Position.Y + 85), new Vector2(this.Velocity.X * 3, this.Velocity.Y)));
            }


            if (Position.X - _widthOfCone == _targetPosition.X && _isSucking == false && _isRetractingCone == false)
            {
                _isExtendingCone = true;
                _elapsedSeconds = 0;
            }

            CanJumpOnToKill = _isExtendingCone || _isRetractingCone || _isSucking;

            if (_isExtendingCone || _isRetractingCone || _isSucking)
            {
                Velocity = new Vector2(0, 0);
            }
            else
            {
                if (_targetAcquirer == null)
                {
                    if (((Screen)Game.Components[0]).Teddy.Position.X < this.Position.X)
                    {
                        Velocity = new Vector2(-1, Velocity.Y);
                    }
                    else if (((Screen)Game.Components[0]).Teddy.Position.X > this.Position.X)
                    {
                        Velocity = new Vector2(1, Velocity.Y);
                    }
                }
                else
                {
                    if (_targetPosition.X < this.Position.X - _widthOfCone)
                    {
                        Velocity = new Vector2(-1, Velocity.Y);
                    }
                    else if (_targetPosition.X > this.Position.X - _widthOfCone)
                    {
                        Velocity = new Vector2(1, Velocity.Y);
                    }
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
                if (_coneFrameCount <= 25)
                {
                    _coneBoxToDraw = new Rectangle(0, 0, 57, 45);
                }
                else if (_coneFrameCount <= 50)
                {
                    _coneBoxToDraw = new Rectangle(58, 0, 57, 45);
                }
                else if (_coneFrameCount <= 75)
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
                _targetAcquirer.LockedIn = true;
                if (_coneFrameCount <= 4)
                {
                    _stuffBoxToDraw = new Rectangle(0,0, 15,12);
                    _currentStuffLevel = 1;
                }
                else if (_coneFrameCount <= 8)
                {
                    _stuffBoxToDraw = new Rectangle(16, 0, 9, 12);
                    _currentStuffLevel = 2;
                }
                else if (_coneFrameCount <= 12)
                {
                    _currentStuffLevel = 3;
                    _stuffBoxToDraw = new Rectangle(25, 0, 27, 12);
                }
                else
                {
                    _numberOfSucksPerformed++;

                    if (_numberOfSucksPerformed < _numberOfSucksToPerform)
                    {
                        _coneFrameCount = 0;
                        _currentStuffLevel = 0;
                    }
                    else
                    {
                        _isRetractingCone = true;
                        _isSucking = false;
                        _coneFrameCount = 0;
                        _currentStuffLevel = 0;
                        _numberOfSucksPerformed = 0;
                        RemoveSurfaceAtLocation();
                        currentScreen.GameObjects.Remove(_targetAcquirer);
                    }

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
                    _targetPosition = new Vector2(-999,-999);
                    _targetAcquirer = null;
                }
                _coneFrameCount++;
            }

            if (BeingDamaged)
            {
                CanJumpOnToKill = false;
                BoxToDraw = new Rectangle(142, 0, 71, 129);
            }

            if (ChildrenEnemies != null)
            {
                foreach (Enemy childEnemy in ChildrenEnemies)
                {
                    childEnemy.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        public override void DoDamage(int damage)
        {
            base.DoDamage(damage);
        }

        private void RemoveSurfaceAtLocation()
        {

            for (int i = ((Screen)Game.Components[0]).Surfaces.Count -1; i >= 0; i--)
			{
                if (((Screen)Game.Components[0]).Surfaces[i].Top >= Position.Y + BoxToDraw.Height)
                {
                    if (((Screen)Game.Components[0]).Surfaces[i].Rect.Intersects(new Rectangle((int)Position.X - _coneBoxToDraw.Width, (int)Position.Y + BoxToDraw.Height, 25, 1000)))
                    {
                        ((Screen)Game.Components[0]).Surfaces.RemoveAt(i);
                        return;
                    }
                    
               
                }
			}
        }
    }
}
