using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary
{
    class TeddyFluff : Teddy
    {

        //Teddy Resources
        private Texture2D _fluffSprite;

        private Dictionary<TeddySpriteState, List<TeddySkeleton>> _spriteSkeletons = new Dictionary<TeddySpriteState, List<TeddySkeleton>>();

        private Game _game;
        private Rectangle _fluffBox;
        private List<FluffWrapper> _fluffs;

        private float _m = 10f;
        private float _k = .1f;
        private float _c = .3f;

        private FluffMotionTypeEnum _fluffMotionType = FluffMotionTypeEnum.Spring;
        private bool _showTeddyOutline = true;

        private enum FluffMotionTypeEnum
        {
            Satellite = 0,
            Spring = 1,
            Stationary = 2
        }


        public TeddyFluff(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame):base(game,  initialPosition, sizeOfFrame)
        {
	        _game = game;
            _fluffSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "Fluff")); 
            _styleSheet = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRunGhost"));
            _fluffBox = new Rectangle(0, 0, _fluffSprite.Width / 2, _fluffSprite.Height / 2);
            _currentFluff = 100;
            CreateFluffs();
        }


        protected override  void checkForObjectInteractions(Screen currentScreen)
        {
            foreach (GameObject f in currentScreen.GameObjects)
            {
                if (f.GetType() == typeof(Goal))
                {
                    if (this.TeddyRectangle.Intersects(f.CollisionRectangle))
                    {
                        Position = new Vector2(-100, -100);
                        this.LevelComplete = true;
                    }
                }
               else if (f.GetType() == typeof(Fluff))
               {
                   if (!_fluffs.Any(fw => fw.Fluff.Equals(f)))
                   {
                        if (!f.Destroyed & this.TeddyRectangle.Intersects(f.CollisionRectangle) == true)
                        {
                            _currentFluff++;
                            f.Destroyed = true;
                        }
                   }

                   
                }

            }
        }

        private Keys? _keyDown;

        public override void Update(GameTime gameTime)
        {

            KeyboardState keyState = Keyboard.GetState();

            if (_keyDown == null)
            {
                if (keyState.IsKeyDown(Keys.Tab))
                {
                    _fluffMotionType += 1;

                    if (_fluffMotionType > FluffMotionTypeEnum.Stationary)
                        _fluffMotionType = 0;

                    _keyDown = Keys.Tab;
                }

                if (keyState.IsKeyDown(Keys.Q)) 
                {
                    _k += .01f;
                    _keyDown = Keys.Q;
                }

                if (keyState.IsKeyDown(Keys.A)) 
                {
                    _k -= .01f;
                    _keyDown = Keys.A;
                }

                if (keyState.IsKeyDown(Keys.W)) 
                {
                    _c += .01f;
                    _keyDown = Keys.W;
                }

                if (keyState.IsKeyDown(Keys.S)) 
                {
                    _c -= .01f;
                    _keyDown = Keys.S;
                }

                if (keyState.IsKeyDown(Keys.T))
                {
                    _keyDown = Keys.T;
                    _showTeddyOutline = !_showTeddyOutline;
                }

            }else
            {
                if (keyState.IsKeyUp(_keyDown.Value))
                    _keyDown = null;
            }


            foreach (FluffWrapper fw in _fluffs)
            {
                int boneID = fw.BoneID;
                TeddySkeleton bone = GetCurrentTeddySkeleton().FirstOrDefault(b => b.ZOrder == boneID);

                //todo: figure out a better way to ensure the number of ref points is the same across bones
                if (bone.ReferencePoints.Count > fw.ReferenceID)
                {

                    Vector2 refPoint = bone.ReferencePoints[fw.ReferenceID];
                    Vector2 point = _facing == Direction.Right ? refPoint : bone.GetFlippedPoint(50, refPoint);

                    refPoint = _position + point;
                    Vector2 vRF =  fw.Fluff.Position - refPoint; //Vector from ref point to fluff
                    float dist = vRF.Length();
                    vRF.Normalize();

                    switch (_fluffMotionType)
                    {
                        case FluffMotionTypeEnum.Satellite:

                            if (dist < fw.Fluff.Velocity.Length() * 2)
                            {
                               fw.Fluff.SetVelocity(fw.Fluff.Velocity *.9f);
                            }
                            fw.Fluff.SetAccelleration( -vRF);     
                            
                            break;
                        case FluffMotionTypeEnum.Spring:


                            Vector2 fSpring = -_k * vRF * dist;
                            Vector2 fd = -_c * fw.Fluff.Velocity;

                            Vector2 fTotal = fSpring + fd;

                            fw.Fluff.SetAccelleration(fTotal / _m);
                    

                            break;
                        case FluffMotionTypeEnum.Stationary:

                            fw.Fluff.SetAccelleration(Vector2.Zero);
                            fw.Fluff.SetVelocity(Vector2.Zero);
                            fw.Fluff.SetPosition(refPoint);
                            break;
                    }


             
                }


                fw.Fluff.Update(gameTime);
            }
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {
            //_fluffRotation += .5f;

            DrawFluff(teddyBatch, gameTime);

            if (_showTeddyOutline)
                base.Draw(gameTime, teddyBatch);
        }

        private void CreateFluffs()
        {
            _fluffs = new List<FluffWrapper>();

            //All frames have teh same makeup, so we can just use whichever is current
            List<TeddySkeleton> skeleton = GetCurrentTeddySkeleton();

            foreach (TeddySkeleton bone in skeleton)
            {
                for (int i = 0; i < bone.ReferencePoints.Count; i++)
                {
                    _fluffs.Add(new FluffWrapper(bone.ZOrder, i, bone.ReferencePoints[i], _game, new Rectangle(0, 0, _fluffBox.Width / 2, _fluffBox.Height / 2)));
                }
            }
        }


        private List<TeddySkeleton> GetCurrentTeddySkeleton()
        {

            if (_spriteSkeletons.ContainsKey(_currentSprite))
            {
                return _spriteSkeletons[_currentSprite];
            }
            else
            {

                var skeleton = new List<TeddySkeleton>();
                _spriteSkeletons.Add(_currentSprite, skeleton);

                switch (_currentSprite)
                {
                    case TeddySpriteState.Run1:

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(46, 45), new Vector2(21,51), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(6, 71), new Vector2(20,50), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(42,70), new Vector2(29,59), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(5, 58), new Vector2(19, 47), 10, 6)); //near arm

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear

                        break;
                    case TeddySpriteState.Run2:
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(42, 49), new Vector2(21,51), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(10,72), new Vector2(21,59), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(31,60), new Vector2(31,72), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(20, 58), new Vector2(18, 47), 10, 6)); // near arm

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear

                        break;
                    case TeddySpriteState.Run3:
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(7, 57), new Vector2(20,48), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43,72), new Vector2(30,59), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(20,61), new Vector2(8,72), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(44, 46), new Vector2(26, 51), 10, 6)); //near arm

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear
                        break;
                    case TeddySpriteState.Blink1:
                    case TeddySpriteState.Blink2:
                    case TeddySpriteState.Blink3:
                    case TeddySpriteState.Blink4:
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 55), 14, 30, 3)); //Body

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(8,73), new Vector2(18,64), 10, 2));//left leg
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43,73), new Vector2(30,64), 10, 5)); //right leg

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43, 64), new Vector2(43, 47), 10, 1));//left arm
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(6, 64), new Vector2(6, 47), 10, 6)); //right arm

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 25),  20, 30, 4)); //Head

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(8, 7), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(42, 7), 5,  5, 8)); //ear
                        break;
                }

                foreach (TeddySkeleton ts in skeleton)
                {
                    if ( _fluffs == null || _fluffs.Count == 0 )
                        ts.SetReferencePoints(_currentFluff, 6, null);
                    else
                    {
                        int actualFluffCount = _fluffs.Count(c => c.BoneID == ts.ZOrder);
                        ts.SetReferencePoints(_currentFluff, 6, actualFluffCount);
                    }
                }
         

                return skeleton;

            }

        }



        private double _fluffRotation = 0.0f;


        private void DrawFluff(SpriteBatch teddyBatch, GameTime gameTime)
        {

            Vector2 origin = new Vector2(_fluffBox.Width / 2, _fluffBox.Height / 2);

            foreach (FluffWrapper fw in _fluffs)
            {
                teddyBatch.Draw(_fluffSprite, fw.Fluff.Position - origin / 2, null, Color.White, (float)_fluffRotation, origin, .5f, SpriteEffects.None, (float)0.0f);
            }

        }


        class FluffWrapper
        {
            public int BoneID { get; set; }
            public int ReferenceID { get; set; }

            public Fluff Fluff { get; set; }

            public FluffWrapper(int boneID, int referenceID, Vector2 startPosition, Game g, Rectangle box)
            {
                this.BoneID = boneID;
                this.ReferenceID = referenceID;
                this.Fluff = new Fluff(g, startPosition, false, 0, 0, box);
            }

        }


        class TeddySkeleton
        {

            public enum SkeletonTypeEnum
            {
                Straight = 0,
                Circle =1
            }

            public List<Vector2> ReferencePoints { get; set; }

            public SkeletonTypeEnum Type { get; set; }
            public Vector2 StartPoint { get; set; }
            
            public Vector2 EndPoint { get; set; }
            
            public Vector2 CenterPoint
            {
                get
                {
                    return this.StartPoint;
                }
                set
                {
                    this.StartPoint = value;
                }
            }

            public int Radius { get; set; }
            public int BodyPercentage { get; set; }
            public int ZOrder { get; set; }

            public Vector2 GetFlippedStartPoint(int spriteWidth) {
                return new Vector2(spriteWidth - this.StartPoint.X, this.StartPoint.Y);
            }

            public Vector2 GetFlippedPoint(int spriteWidth, Vector2 point)
            {
                return new Vector2(spriteWidth - point.X, point.Y);
            }

            public Vector2 GetFlippedEndPoint(int spriteWidth)
            {
                return new Vector2(spriteWidth - this.EndPoint.X, this.EndPoint.Y);
            }

            public Vector2 GetFlippedCenterPoint(int spriteWidth)
            {
                return GetFlippedStartPoint(spriteWidth);
            }

            public TeddySkeleton(SkeletonTypeEnum type, Vector2 startPoint, Vector2 endPoint, int bodyPercentage, int zOrder)
            {
                this.Type = type;
                this.StartPoint = startPoint;
                this.EndPoint = endPoint;
                this.BodyPercentage = bodyPercentage;
                this.ZOrder = zOrder;
            }

            public TeddySkeleton(SkeletonTypeEnum type, Vector2 centerPoint, int radius, int bodyPercentage, int zOrder)
            {
                this.Type = type;
                this.CenterPoint = centerPoint;
                this.Radius = radius;
                this.BodyPercentage = bodyPercentage;
                this.ZOrder = zOrder;

            }


            public void SetReferencePoints(int totalPoints, int pointLength, int? actualPoints)
            {
                if (this.ReferencePoints == null)
                    this.ReferencePoints = new List<Vector2>();

                this.ReferencePoints.Clear();

                int maxPointCount = (int)(totalPoints * ((float)this.BodyPercentage / 100));

                switch (this.Type)
                {
                    case SkeletonTypeEnum.Straight:

                        Vector2 vSE = this.EndPoint - this.StartPoint;

                        int pointPerLine = (int)( vSE.Length() / pointLength);

                        pointPerLine = Math.Min(maxPointCount, pointPerLine);

                        if (actualPoints != null)
                            pointPerLine = actualPoints.Value;

                        float step = vSE.Length() / pointPerLine;
                        vSE.Normalize();

                        for (int i = 0; i <= pointPerLine; i++)
                        {
                            Vector2 refPoint = this.StartPoint + (vSE * (step * i));
                            this.ReferencePoints.Add(refPoint);
                        }

                        break;

                    case SkeletonTypeEnum.Circle:


                        int pointsWithinRadius = (int)(this.Radius / pointLength) ;
                        float radiusStep = (this.Radius / Math.Max( pointsWithinRadius, 1)); //Evently space 

                        for (int i = 0; i <= pointsWithinRadius; i++)
                        {
                            int radius = (int)( i * radiusStep);

                            if (i == 0)
                            {
                                this.ReferencePoints.Add(this.CenterPoint);
                            }
                            else
                            {
                                //At current radius, how many points fit around the circle
                                float thetaActualStep = (float)pointLength/radius;
                                int stepCount = (int)(2 * Math.PI / thetaActualStep);
                                //try to evenly space the points around the circle
                                float stepAngle = (float)(2 * Math.PI / stepCount);

                                for (int j = 0; j < stepCount; j++)
                                {
                                    float dx = (float)Math.Cos(stepAngle * j) * radius;
                                    float dy = (float)Math.Sin(stepAngle * j) * radius;

                                    Vector2 position = this.CenterPoint + new Vector2(dx, dy);
                                    this.ReferencePoints.Add(position);
                                }

                            }

                        }
                        //Want to draw inside portion of teddy last
                        this.ReferencePoints.Reverse();

                        break;
                }


            }


        }

    }
}
