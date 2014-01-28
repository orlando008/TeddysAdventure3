using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeddysAdventureLibrary
{
    public class Enemy:DrawableGameComponent
    {

#if COLLISIONS
        protected Texture2D _redFill;
#endif


        private Texture2D _styleSheet;
        private Vector2 _position;
        private Rectangle _boxToDraw;
        private Vector2 _frameSize;
        private List<TeddysAdventureLibrary.GeometryMethods.RectangleF> _hitBoxes;
        protected Game _game;

        //Drawing Animations
        protected int _frameCount = 0;
        protected int _deathFrameCount = 0;
        protected int _deathFrames = 12;
        protected float _layerDepth = 0;   //0 = front of screen, 1 = back  
        protected int _beingDamagedFrameCount = 0;
        protected int _beingDamagedFrames = 150;
                
        //Enemy State
        private bool _beingDamaged = false;
        private bool _dying = false;
        private bool _destroyed = false;
        protected Vector2 _velocity = new Vector2(1, -3);     
        private Surface _mySurface; 

        //Enemy Characteristics
        protected bool _canJumpOnToKill;
        protected float _gravity = .3f; //m/s2
        protected float _collisionDampingFactor = .6f;
        protected bool _fallsOffSurface = true;
        protected bool _playerCanRide = false;
        protected bool _playerCanPassThrough = false;
        protected bool _changeDirectionUponSurfaceHit = false;
        protected bool _bounceOffSidesOfViewport = false;
        protected bool _passesThroughSurfaces = false;
        protected int _damageCurrentlyBeingTaken = 0;
        private bool _specialNamedEnemy = false;
        private string _name = "";
        private Texture2D _hudIcon;

        private int _steeringDownFrames = 4;
        private int _steeringDownCount = 0;
        private bool _playerIsSteering = false;
        private int _steeringDirectionY = 0;

        private BossIcon _bossIcon;

        private SpriteFont _damageFont;
        private Texture2D _pixelImage;

        public Texture2D PixelImage
        {
            get { return _pixelImage; }
            set { _pixelImage = value; }
        }

        private List<Enemy> _childrenEnemies;

        // 15 by default since, only testing with plane
        private int _damage = 15;

        private int _health = 1;
        private int _maxHealth = 1;

        public int MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }

        public bool SpecialNamedEnemy
        {
            get { return _specialNamedEnemy; }
            set { _specialNamedEnemy = value; }
        }

        public Texture2D HudIcon
        {
            get { return _hudIcon; }
            set { _hudIcon = value; }
        }

        public BossIcon BossIconObject
        {
            get { return _bossIcon; }
            set { _bossIcon = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public Texture2D StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_hitBoxes == null)
                {
                    _hitBoxes = new List<GeometryMethods.RectangleF>();
                    _hitBoxes.Add(new GeometryMethods.RectangleF(CollisionRectangle));
                }
                else
                {
                    _hitBoxes[0] = new GeometryMethods.RectangleF(CollisionRectangle);
                }
            }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 FrameSize
        {
            get { return _frameSize; }
            set { _frameSize = value; }
        }

        public Rectangle BoxToDraw
        {
            get { return _boxToDraw; }
            set { _boxToDraw = value; }
        }

        public Rectangle DestinationBoxToDraw
        {
            get
            {
                return new Rectangle((int)_position.X + _boxToDraw.Width / 2, (int)_position.Y + _boxToDraw.Height / 2, (int)_boxToDraw.Width, (int)_boxToDraw.Height);
            }
        }

        public List<TeddysAdventureLibrary.GeometryMethods.RectangleF> HitBoxes
        {
            get { return _hitBoxes; }
            set { _hitBoxes = value; }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, BoxToDraw.Width, BoxToDraw.Height);
            }
        }

        public bool BeingDamaged
        {
            get { return _beingDamaged; }
            set
            {
                _beingDamaged = value;

                if (_beingDamaged == false)
                {
                    _damageCurrentlyBeingTaken = 0;
                    _canJumpOnToKill = true;
                    _playerCanPassThrough = !_playerCanPassThrough;
                }
                else
                {
                    _canJumpOnToKill = false;
                    _playerCanPassThrough = !_playerCanPassThrough;
                }
            }
        }

        public  bool Dying
        {
            get { return _dying; }
        }

        public  bool Destroyed
        {
            get { return _destroyed; }
        }

        public  bool CanInteractWithPlayer
        {
            get { return !(_destroyed || _dying || _playerCanPassThrough || _beingDamaged); }
        }

        public bool PlayerCanRide
        {
            get { return _playerCanRide; } 
        }

        public bool CanJumpOnToKill
        {
            get { return _canJumpOnToKill; }
            set
            {
                _canJumpOnToKill = value;
            }
        }

        public List<Enemy> ChildrenEnemies
        {
            get 
            { 
                return _childrenEnemies; 
            }
            set
            {
                _childrenEnemies = value;
            }
        }

        public int Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }

        public bool PlayerIsSteering
        {
            get { return _playerIsSteering; }
            set { _playerIsSteering = value; }
        }

        public Enemy(Game game)
            : base(game)
        {
            _game = game;
            _damageFont = game.Content.Load<SpriteFont>("Fonts\\Arial12");
            _pixelImage = game.Content.Load<Texture2D>("Icons\\HealthUnit");

 #if COLLISIONS
            _redFill = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _redFill.SetData<Color>(new Color[] { Color.Red });
#endif
        }

        public void PlayerIsSteeringEnemyDown(int framesToMove, int direction)
        {
            if (_playerIsSteering == false)
            {
                _steeringDownFrames = framesToMove;
                _steeringDownCount = 0;
                _playerIsSteering = true;
                _steeringDirectionY = direction;
            }
        }

        public virtual void DoDamage(int damage)
        {
            if (!_beingDamaged)
            {
                Health -= damage;
                _damageCurrentlyBeingTaken = damage;

                if (_health <= 0)
                {
                    _destroyed = false;
                    _dying = true;
                    _deathFrameCount = 0;
                }
                else
                {
                    BeingDamaged = true;
                    _beingDamagedFrameCount = 0;
                }
            }

        }

        public void HardKill()
        {
            _destroyed = true;
        }

        public void BringToLife()
        {
            _destroyed = false;
            _dying = false;
            _deathFrameCount = 0;
        }


        public void MoveByX(int x, bool moveChildren)
        {
            this.Position = new Vector2((int)Position.X + x, (int)Position.Y);

            if (moveChildren)
            {
                if (this.ChildrenEnemies != null)
                {
                    foreach (Enemy e in ChildrenEnemies)
                        e.MoveByX(x, moveChildren);
                }
            }


            for (int i = _hitBoxes.Count - 1; i >= 0; i--)
            {
                _hitBoxes[i] = new GeometryMethods.RectangleF(_hitBoxes[i].X + x, _hitBoxes[i].Y, _hitBoxes[i].Width, _hitBoxes[i].Height);
            }

        }

        public void MoveByY(float y, bool moveChildren)
        {
            this.Position = new Vector2(Position.X, Position.Y + y);

            if (moveChildren)
            {
                if (this.ChildrenEnemies != null)
                {
                    foreach (Enemy e in ChildrenEnemies)
                        e.MoveByY(y, moveChildren);
                }
            }

            for (int i = _hitBoxes.Count - 1; i >= 0; i--)
            {
                _hitBoxes[i] = new GeometryMethods.RectangleF(_hitBoxes[i].X, _hitBoxes[i].Y + y, _hitBoxes[i].Width, _hitBoxes[i].Height);
            }
        }

        public virtual void DrawEnemy(GameTime gameTime, SpriteBatch sp)
        {
        }

        public virtual void DrawDamage(GameTime gameTime, SpriteBatch sp)
        {
            if (BeingDamaged)
            {
                sp.DrawString(_damageFont, _damageCurrentlyBeingTaken.ToString(), new Vector2(Position.X - 20, Position.Y - 20), Color.Red);
            }
        }

        public virtual bool RectangleInsersectsWithHitBoxes(GeometryMethods.RectangleF r, ref GeometryMethods.RectangleF rHit)
        {


            foreach (GeometryMethods.RectangleF rec in _hitBoxes)
            {
                if (rec.Intersects(r))
                {
                    rHit = rec;
                    return true;
                }
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {

            if (_dying)
            {
                if (_deathFrameCount > _deathFrames)
                {
                    //Enemy is done dying.  Remove.
                    _dying = false;
                    _destroyed = true;
                    _position = new Vector2(-100, -100);
                }
                _deathFrameCount += 1; 
                return;

            }else if (_destroyed)
            {
                return;
            }
            else if (_beingDamaged)
            {
                _beingDamagedFrameCount += 1;

                if (_beingDamagedFrameCount > _beingDamagedFrames)
                {
                    BeingDamaged = false;
                    _beingDamagedFrameCount = 0;
                }
            }


            Screen currentScreen = (Screen)Game.Components[0];

            MoveByX((int) _velocity.X, false);

            //wait till this car lands on a surface, then you only need to check against that surface (for whether or not you hit the end of the surface)
            if (!_fallsOffSurface)
            {
                if (_mySurface != null)
                {
                    if (_mySurface.Left > CollisionRectangle.Left | _mySurface.Right < CollisionRectangle.Right)
                    {
                        _velocity = new Vector2(-Velocity.X, Velocity.Y);
                    }
                }
            }

            if (!_passesThroughSurfaces)
            {
                foreach (Surface surface in currentScreen.Surfaces)
                {

                    if (surface.Rect.Intersects(CollisionRectangle))
                    {
                        //We must figure out which surface we are hitting to know if we should change direction and which way to shift 
                        if (Math.Abs(surface.Left - CollisionRectangle.Left) > Math.Abs(surface.Left - CollisionRectangle.Right))
                        {
                            Position = new Vector2(surface.Left - CollisionRectangle.Width - 1, Position.Y);
                            if (_changeDirectionUponSurfaceHit)
                            {
                                _velocity.X *= -1;
                            }
                            else
                            {
                                if (_velocity.X > 0)
                                    _velocity.X *= -_collisionDampingFactor;
                            }

                        }
                        else
                        {
                            Position = new Vector2(surface.Right + 1, Position.Y);

                            if (_changeDirectionUponSurfaceHit)
                            {
                                _velocity.X *= -1;
                            }
                            else
                            {
                                if (_velocity.X < 0)
                                    _velocity.X *= -_collisionDampingFactor;
                            }

                        }

                        if (Math.Abs(_velocity.X) < 1)
                            _velocity.X = 0f;
                        break;
                    }
                }

                //when moving up, check for hitting your head on a surface
                if (Velocity.Y < 0)
                {
                    foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                    {
                        if (CollisionRectangle.Intersects(surface.Rect) & (CollisionRectangle.Top < surface.Bottom))
                        {
                            Position = new Vector2(Position.X, surface.Bottom + 1);

                            if (_changeDirectionUponSurfaceHit)
                            {
                                _velocity.Y *= -1;
                            }
                            else
                            {
                                _velocity.Y = 0f;
                            }


                            break;
                        }
                    }
                }

                //Bounce off the sides of the viewport if necessary
                if (_bounceOffSidesOfViewport)
                {

                    if (Position.Y < currentScreen.CameraBounds.Top || (Position.Y + BoxToDraw.Height > currentScreen.CameraBounds.Bottom))
                    {
                        if (Position.Y < 0)
                            Position = new Vector2(Position.X, currentScreen.CameraBounds.Top);
                        else
                            Position = new Vector2(Position.X, currentScreen.CameraBounds.Bottom - BoxToDraw.Height);

                        Velocity = new Vector2(Velocity.X, -1 * Velocity.Y);
                    }


                    if (this.Position.X < currentScreen.CameraBounds.Left || this.Position.X + BoxToDraw.Width > currentScreen.CameraBounds.Right)
                    {
                        if (this.Position.X < currentScreen.CameraBounds.Left)
                            this.Position = new Vector2(currentScreen.CameraBounds.Left, this.Position.Y);
                        else
                            this.Position = new Vector2(currentScreen.CameraBounds.Right - BoxToDraw.Width, this.Position.Y);

                        this.Velocity = new Vector2(Velocity.X * -1, Velocity.Y);
                    }

                }

            }

            if (_playerIsSteering)
            {
                if (_steeringDownCount < _steeringDownFrames)
                {
                    MoveByY(_steeringDirectionY, true);
                    _steeringDownCount++;
                }
                else
                {
                    if (_steeringDownCount > _steeringDownFrames + 1)
                    {
                        _playerIsSteering = false;
                        _steeringDownCount = 0;
                    }
                    _steeringDownCount++;
                }
            }

            applyGravity();
        }

        private void applyGravity()
        {

            MoveByY(_velocity.Y, false);
            _velocity.Y += _gravity; //Down is positive

            if (!_passesThroughSurfaces)
            {
                foreach (Surface surface in ((Screen)Game.Components[0]).Surfaces)
                {
                    if (CollisionRectangle.Intersects(surface.Rect) & (CollisionRectangle.Bottom > surface.Top))
                    {
                        Position = new Vector2(Position.X, surface.Top - BoxToDraw.Height);

                        if (_changeDirectionUponSurfaceHit)
                        {
                            _velocity.Y *= -1;
                        }

                        _velocity.Y *= -_collisionDampingFactor;

                        //once it hits a base surface for the first time, claim that surface as "_mySurface", stop applying gravity
                        if (!_fallsOffSurface)
                        {
                            _mySurface = surface;
                        }

                        if (Math.Abs(_velocity.Y) < 1)
                            _velocity.Y = 0f;


                        break;
                    }
                }
            }
        }

    }
}
