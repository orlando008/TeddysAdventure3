using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    class ZipperPowerup : Powerup
    {
        protected Texture2D _unzipAnimation;
        protected Texture2D _fluffSprite;
        protected Keys _unzipKey = Keys.Z;
        protected bool _startUnzip;
        protected bool _unzipping;
        private Rectangle _fluffBox;
        
        protected List<Rectangle> _animationRectangles = new List<Rectangle>();
        protected List<int> _animationFrameCount = new List<int>();
        private int _currentAnimationRectangle = 0;
        private int _currentAnimationFrameCount = 0;

        private List<TeddysAdventureLibrary.TeddyFluff.FluffWrapper> _fluffs;

        public ZipperPowerup(Game game)
            : base(game)
        {
            _unzipAnimation = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "UnzipAnimation"));
            _fluffSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "Fluff"));
            _fluffBox = new Rectangle(0, 0, _fluffSprite.Width / 2, _fluffSprite.Height / 2);

            _animationRectangles.Add(new Rectangle(0, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(50, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(100, 0, 50, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(150, 0, 62, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(213, 0, 103, 75));
            _animationFrameCount.Add(15);

            _animationRectangles.Add(new Rectangle(317, 0, 139, 75));
            _animationFrameCount.Add(15);

        }

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {
            if (teddy.Velocity.Y == 0 && teddy.Velocity.X == 0)
            {
                if (_startUnzip == true && keyState.IsKeyUp(_unzipKey))
                {
                    _unzipping = true;
                    CreateFluffs(teddy);
                }

                if (_startUnzip == false && keyState.IsKeyDown(_unzipKey))
                {
                    _startUnzip = true;
                    _currentAnimationRectangle = 0;
                    _currentAnimationFrameCount = 0;
                }
            }

            return base.Update(gameTime, screen, teddy, keyState);
        }

        public override bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {
            if (_unzipping)
            {
                DrawFluff(teddyBatch, gameTime);
                teddyBatch.Draw(_unzipAnimation, new Vector2(teddy.Position.X - ((_animationRectangles[_currentAnimationRectangle].Width - teddy.FrameSize.X)/2), teddy.Position.Y), _animationRectangles[_currentAnimationRectangle], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                _currentAnimationFrameCount++;

                if (_currentAnimationFrameCount >= _animationFrameCount[_currentAnimationRectangle])
                {
                    _currentAnimationRectangle++;
                    _currentAnimationFrameCount = 0;
                }

                if (_currentAnimationRectangle >= _animationRectangles.Count)
                {
                    _startUnzip = false;
                    _unzipping = false;

                    ((Screen)_game.Components[0]).Teddy = new TeddyFluff(_game, teddy.Position, new Vector2(50, 75));
                }
                return false;
            }

            return true;
        }

        private void DrawFluff(SpriteBatch teddyBatch, GameTime gameTime)
        {

            Vector2 origin = new Vector2(_fluffBox.Width / 2, _fluffBox.Height / 2);

            foreach (TeddysAdventureLibrary.TeddyFluff.FluffWrapper fw in _fluffs)
            {
                teddyBatch.Draw(_fluffSprite, fw.Fluff.Position, new Rectangle(0,0,20,20), Color.White, (float)0, origin, .5f, SpriteEffects.None, (float)0.0f);
            }

        }

        private List<TeddysAdventureLibrary.TeddyFluff.TeddySkeleton> GetCurrentTeddySkeleton(Teddy teddy)
        {
            var skeleton = new List<TeddysAdventureLibrary.TeddyFluff.TeddySkeleton>();
            skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 55), 12, 30, 3)); //Body

            //skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(8, 73), new Vector2(18, 64), 10, 2));//left leg
            //skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43, 73), new Vector2(30, 64), 10, 5)); //right leg

            //skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43, 64), new Vector2(43, 47), 10, 1));//left arm
            //skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(6, 64), new Vector2(6, 47), 10, 6)); //right arm

            skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 25), 12, 25, 4)); //Head

            skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(8, 7), 5, 5, 7)); //ear
            skeleton.Add(new TeddysAdventureLibrary.TeddyFluff.TeddySkeleton(TeddysAdventureLibrary.TeddyFluff.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(42, 7), 5, 5, 8)); //ear

            foreach (TeddysAdventureLibrary.TeddyFluff.TeddySkeleton ts in skeleton)
            {
                if (_fluffs == null || _fluffs.Count == 0)
                    ts.SetReferencePoints(teddy.CurrentFluff, 6, null);
                else
                {
                    int actualFluffCount = _fluffs.Count(c => c.BoneID == ts.ZOrder);
                    ts.SetReferencePoints(teddy.CurrentFluff, 6, actualFluffCount);
                }
            }
            return skeleton;
        }

        private void CreateFluffs(Teddy teddy)
        {

            var _oldFluffs = _fluffs;

            _fluffs = new List<TeddysAdventureLibrary.TeddyFluff.FluffWrapper>();

            //All frames have teh same makeup, so we can just use whichever is current
            List<TeddysAdventureLibrary.TeddyFluff.TeddySkeleton> skeleton = GetCurrentTeddySkeleton(teddy);

            foreach (TeddysAdventureLibrary.TeddyFluff.TeddySkeleton bone in skeleton)
            {
                for (int i = 0; i < bone.ReferencePoints.Count; i++)
                {

                    Vector2 refPoint = bone.ReferencePoints[i];
                    Vector2 point = bone.GetFlippedPoint(34, refPoint);

                    refPoint = new Vector2(teddy.Position.X + 15, teddy.Position.Y) + point;

                    Vector2 initialVelocity = Vector2.Zero;

                    if (_oldFluffs != null)
                    {
                        var oldFluff = _oldFluffs.FirstOrDefault(f => f.BoneID == bone.ZOrder && f.ReferenceID == i);
                        if (oldFluff != null)
                        {
                            refPoint = oldFluff.Fluff.Position;
                            initialVelocity = oldFluff.Fluff.Velocity;
                        }
                    }

                    _fluffs.Add(new TeddysAdventureLibrary.TeddyFluff.FluffWrapper(bone.ZOrder, i, refPoint, initialVelocity, _game, new Rectangle(0, 0, _fluffBox.Width / 2, _fluffBox.Height / 2)));
                }
            }


        }

    }
}
