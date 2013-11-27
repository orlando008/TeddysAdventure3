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

#if COLLISIONS
        private Texture2D _pixelSprite;
#endif

        private Dictionary<TeddySpriteState, List<TeddySkeleton>> _spriteSkeletons = new Dictionary<TeddySpriteState, List<TeddySkeleton>>();

        private Game _game;
        private Rectangle _fluffBox;
        private List<FluffWrapper> _fluffs;

        private float _m = 10f;
        private float _k = .1f;
        private float _c = .3f;

        private FluffMotionTypeEnum _fluffMotionType = FluffMotionTypeEnum.Stationary;
        private bool _showTeddyOutline = false;
        
        private enum FluffMotionTypeEnum
        {
            Satellite = 0,
            Spring = 1,
            Stationary = 2,
            Exploding =3,
            Collapsing = 4,
        }


        public TeddyFluff(Game game,  Vector2 initialPosition, Vector2 sizeOfFrame):base(game,  initialPosition, sizeOfFrame)
        {
	        _game = game;
#if COLLISIONS
            _pixelSprite  = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
#endif


            _fluffSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "Fluff")); 
            _styleSheet = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRunGhost"));
            _fluffBox = new Rectangle(0, 0, _fluffSprite.Width / 2, _fluffSprite.Height / 2);
            _currentFluff = 40;
            CreateFluffs();
        }


        protected override void HandleFluffGrab(Fluff f)
        {
            if (!_fluffs.Any(fw => fw.Fluff.Equals(f)))
            {
                    _currentFluff++;
                    ReevaluateSkeletonAndFluff();
                    f.Destroyed = true;
            }
        }

        protected override void HandleEnemyInteraction(Enemy e, Screen currentScreen, GeometryMethods.RectangleF enemyHitBox)
        {
            if (!_isHit)
            {
                base.HandleEnemyInteraction(e, currentScreen, enemyHitBox);

                if (_isHit)
                {
                    ReevaluateSkeletonAndFluff();
                }
            }
                        
        }

        private void SetFluffMode(FluffMotionTypeEnum t)
        {
            _fluffMotionType = t;

            switch (t)
            {
                case FluffMotionTypeEnum.Exploding:
                    int fluffCount = _fluffs.Count;

                    float step = (float)(2 * Math.PI / fluffCount);


                    int i = 0;

                    foreach (FluffWrapper fw in _fluffs)
                    {
                        int dx = (int)(Math.Cos(step * i) * 10);
                        int dy = (int)(Math.Sin(step * i) * 10);

                        fw.Fluff.SetAccelleration(Vector2.Zero);
                        fw.Fluff.SetVelocity(new Vector2(dx, dy));
                        fw.Fluff.SetApplyGravity(true);
                        i++;
                    }
                    break;

                case FluffMotionTypeEnum.Collapsing:

                    foreach (FluffWrapper fw in _fluffs)
                    {
                        fw.Fluff.SetAccelleration(Vector2.Zero);
                        fw.Fluff.SetVelocity(Vector2.Zero);
                        fw.Fluff.SetApplyGravity(true);
                    }

                    break;
            }


        }

        private Keys? _keyDown;

        public override GeometryMethods.RectangleF TeddyRectangle
        {
            get
            {
                //var fluffRects = new List<GeometryMethods.RectangleF>();
                //foreach(FluffWrapper fw in _fluffs){
                //  fluffRects.Add( fw.Fluff.CollisionRectangle);
                //}

                //return new GeometryMethods.MultiRectangleF(base.TeddyRectangle.Width, base.TeddyRectangle.Height,   fluffRects.ToArray());

                return base.TeddyRectangle;
            }
        }
      

        public override void Update(GameTime gameTime)
        {

            KeyboardState keyState = Keyboard.GetState();

            if (_keyDown == null)
            {
                if (keyState.IsKeyDown(Keys.Tab))
                {
                    SetFluffMode(_fluffMotionType + 1);

                    if (_fluffMotionType > FluffMotionTypeEnum.Exploding)
                        SetFluffMode(0);

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

                if (keyState.IsKeyDown(Keys.Down))
                {
                    _keyDown = Keys.Down;
                    SetFluffMode(FluffMotionTypeEnum.Collapsing);
                }

                if (keyState.IsKeyDown(Keys.Up))
                {
                    _keyDown = Keys.Up;
                    SetFluffMode(FluffMotionTypeEnum.Spring);
                }

            }else
            {
                if (keyState.IsKeyUp(_keyDown.Value))
                    _keyDown = null;
            }

            Vector2 prePosition = this.Position;

            base.Update(gameTime);


            foreach (FluffWrapper fw in _fluffs)
            {
                int boneID = fw.BoneID;
                TeddySkeleton bone = GetCurrentTeddySkeleton().FirstOrDefault(b => b.ZOrder == boneID);

                //todo: figure out a better way to ensure the number of ref points is the same across bones
                if (bone.ReferencePoints.Count > fw.ReferenceID)
                {

                    Vector2 refPoint = bone.ReferencePoints[fw.ReferenceID];
                    Vector2 point = Facing == Direction.Right ? refPoint : bone.GetFlippedPoint(50, refPoint);

                    //We want the fluff to be centered around its ref point, not with its topright on the point
                    Vector2 fluffOffset = new Vector2(_fluffBox.Width / 2, _fluffBox.Height / 2);

                    refPoint = _position + point - fluffOffset;
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

                            if (dist == 0)
                                fw.Fluff.SetAccelleration(Vector2.Zero);
                            else
                                fw.Fluff.SetAccelleration(-vRF);     
                            
                            break;
                        case FluffMotionTypeEnum.Spring:


                            Vector2 fSpring = -_k * vRF * dist;
                            if (dist == 0)
                            {
                                fSpring = Vector2.Zero;
                            }

                            
                            Vector2 fd = -_c * fw.Fluff.Velocity;
                            Vector2 fTotal = fSpring + fd;

                            fw.Fluff.SetAccelleration(fTotal / _m);

                            break;
                        case FluffMotionTypeEnum.Stationary:

                            fw.Fluff.SetAccelleration(Vector2.Zero);
                            fw.Fluff.SetVelocity(Vector2.Zero);
                            fw.Fluff.SetPosition(refPoint);
                            break;

                        case FluffMotionTypeEnum.Exploding:

                            break;
                        case FluffMotionTypeEnum.Collapsing:

                            //How for did teddy move this frame
                            Vector2 teddyMovement = this.Position - prePosition;
                            fw.Fluff.SetPosition(fw.Fluff.Position + new Vector2(teddyMovement.X,0));



                            _fluffRotation += teddyMovement.X / (_fluffBox.Width / 2);



                            break;

                    }
             
                }


                fw.Fluff.Update(gameTime);
            }

        }
        
        public override void Draw(GameTime gameTime, SpriteBatch teddyBatch)
        {
            //todo: apply angular accel with damping to have each fluff spin down
           // _fluffRotation += .5f;



#if COLLISIONS
            _pixelSprite.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(_pixelSprite, this.Position, this.TeddyRectangle.AsRect(), Color.Red);
#endif

            DrawFluff(teddyBatch, gameTime);

            if (_showTeddyOutline)
                base.Draw(gameTime, teddyBatch);
        }

        private void CreateFluffs()
       
       {

           var _oldFluffs = _fluffs;

            _fluffs = new List<FluffWrapper>();

            //All frames have teh same makeup, so we can just use whichever is current
            List<TeddySkeleton> skeleton = GetCurrentTeddySkeleton();

            foreach (TeddySkeleton bone in skeleton)
            {
                for (int i = 0; i < bone.ReferencePoints.Count; i++)
                {

                    Vector2 refPoint = bone.ReferencePoints[i];
                    Vector2 point = Facing == Direction.Right ? refPoint : bone.GetFlippedPoint(50, refPoint);

                    refPoint = _position + point;

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

                    _fluffs.Add(new FluffWrapper(bone.ZOrder, i, refPoint, initialVelocity , _game, new Rectangle(0, 0, _fluffBox.Width / 2, _fluffBox.Height / 2)));
                }
            }
        }

        private void ReevaluateSkeletonAndFluff()
        {

            //Quick and dirty implementation' throw everything out and start over
            _spriteSkeletons.Clear();

            CreateFluffs();


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

            Vector2 origin = new Vector2(_fluffBox.Width , _fluffBox.Height );

            foreach (FluffWrapper fw in _fluffs)
            {
                teddyBatch.Draw(_fluffSprite, fw.Fluff.Position + origin/4   , null, Color.White, (float)_fluffRotation, origin  , .5f, SpriteEffects.None, (float)0.0f);

#if COLLISIONS
                _pixelSprite.SetData<Color>(new Color[] { Color.Green });
                teddyBatch.Draw(_pixelSprite, fw.Fluff.Position, (Rectangle?)fw.Fluff.CollisionRectangle.AsRect(), Color.Green);
#endif


            }

        }


       public class FluffWrapper
        {
            public int BoneID { get; set; }
            public int ReferenceID { get; set; }

            public Fluff Fluff { get; set; }

            public FluffWrapper(int boneID, int referenceID, Vector2 startPosition, Vector2 initialVelocity, Game g, Rectangle box)
            {
                this.BoneID = boneID;
                this.ReferenceID = referenceID;
                this.Fluff = new Fluff(g, startPosition, false, 0, 0, box, false, false);
                this.Fluff.SetVelocity(initialVelocity);
            }

        }


        public class TeddySkeleton
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
                if (maxPointCount == 0) return;

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
                        int totalPointCount = 0;

                        for (int i = 0; i <= pointsWithinRadius; i++)
                        {
                            int radius = (int)( i * radiusStep);

                            if (i == 0)
                            {
                                this.ReferencePoints.Add(this.CenterPoint);
                                totalPointCount++;
                                if (totalPointCount >= maxPointCount) break;
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
                                    totalPointCount++;
                                    if (totalPointCount >= maxPointCount) { break; }
                                }

                            }
                            if (totalPointCount >= maxPointCount) break;
                        }
                        //Want to draw inside portion of teddy last
                        this.ReferencePoints.Reverse();

                        break;
                }


            }


        }

    }
}
