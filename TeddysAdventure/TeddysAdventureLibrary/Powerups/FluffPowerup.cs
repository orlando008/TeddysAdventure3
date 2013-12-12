using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeddysAdventureLibrary {
    class FluffPowerup:Powerup
    {



        //Teddy Resources
        private Texture2D _fluffSprite;

#if COLLISIONS
        private Texture2D _pixelSprite;
#endif

        private Dictionary<Teddy.TeddySpriteState, List<TeddySkeleton>> _spriteSkeletons = new Dictionary<Teddy.TeddySpriteState, List<TeddySkeleton>>();

        private Game _game;
        private Rectangle _fluffBox;
        private List<FluffWrapper> _fluffs;

        private float _m = 10f;
        private float _k = .1f;
        private float _c = .3f;

        protected Keys _zipKey = Keys.Up;

        private FluffMotionTypeEnum _fluffMotionType = FluffMotionTypeEnum.Spring;
        private bool _showTeddyOutline = false;
        
        private enum FluffMotionTypeEnum
        {
            Satellite = 0,
            Spring = 1,
            Stationary = 2,
            Exploding =3,
            Collapsing = 4,
            Launching = 5
        }

        public FluffPowerup(Game game, Teddy teddy) :base(game)
        {
	        _game = game;
#if COLLISIONS
            _pixelSprite  = new Texture2D(_game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
#endif

            _fluffSprite = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Objects", "Fluff")); 
           // _styleSheet = game.Content.Load<Texture2D>(System.IO.Path.Combine(@"Teddy", "TeddyRunGhost"));
            _fluffBox = new Rectangle(0, 0, _fluffSprite.Width / 2, _fluffSprite.Height / 2);
            CreateFluffs(teddy);
        }




        public override bool HandleFluffGrab(Teddy teddy, Fluff f)
        {
            if (!_fluffs.Any(fw => fw.Fluff.Equals(f)))
            {
                ReevaluateSkeletonAndFluff(teddy);
                return true;
            }
            else {
                return false;
            }
           
        }

        public override bool AfterTeddyDamage(Teddy teddy)
        {
            ReevaluateSkeletonAndFluff(teddy);
            return false; //Teddy does not lose fluff powerup after getting hit
        }


        private void SetFluffMode(FluffMotionTypeEnum t, Teddy teddy)
        {
            int i;
            int fluffCount;
            float step;
            float startAngle;

            switch (t)
            {

                case FluffMotionTypeEnum.Exploding:
                     fluffCount = _fluffs.Count;

                     step = (float)( (2 *Math.PI) /fluffCount);
                     startAngle = 0;

                     i = 0;

                    foreach (FluffWrapper fw in _fluffs)
                    {
                            int dx = (int)(Math.Cos(startAngle + (step * i)) * 10  );
                        int dy =- (int)(Math.Sin(startAngle + (step * i)) * 10);

                        fw.Fluff.SetAccelleration(Vector2.Zero);
                        fw.Fluff.SetVelocity(new Vector2(dx, dy));
                        fw.Fluff.SetApplyGravity(true);
                        i++;
                    }
                    _preventsJump = false;
                    break;



                case FluffMotionTypeEnum.Launching:
                     fluffCount = _fluffs.Count;


                     Random r = new Random();

                     float range = (float) Math.PI / 16;

                     //step = (float)( (Math.PI /8) /fluffCount);
                     startAngle = (float)Math.PI / 4;

                     i = 0;

                    foreach (FluffWrapper fw in _fluffs)
                    {
                        double fluffAngle = startAngle + (range * r.NextDouble());

                        int dx = (int)(Math.Cos(fluffAngle) * 10  );
                        int dy =- (int)(Math.Sin(fluffAngle) * 10);

                        fw.Fluff.SetAccelleration(Vector2.Zero);
                        fw.Fluff.SetVelocity(new Vector2(dx, dy));
                        fw.Fluff.SetApplyGravity(true);
                        i++;
                    }
                    _preventsJump = false;
                    break;

                case FluffMotionTypeEnum.Collapsing:
                    _preventsJump = true;

                    foreach (FluffWrapper fw in _fluffs)
                    {
                        fw.Fluff.SetAccelleration(Vector2.Zero);
                        fw.Fluff.SetVelocity(Vector2.Zero);
                        fw.Fluff.SetApplyGravity(true);
                    }

                    break;

               default:
                    if (_fluffMotionType == FluffMotionTypeEnum.Collapsing){
                        foreach (FluffWrapper fw in _fluffs)
                        {
                            fw.Fluff.SetAccelleration(Vector2.Zero);
                            fw.Fluff.SetVelocity(Vector2.Zero);
                            fw.Fluff.SetApplyGravity(false);
                        }
                        _preventsJump = false;
                    }
                    else if (_fluffMotionType == FluffMotionTypeEnum.Launching)
                    {
                        //Teddy is comming down from a launch.
                        //Calculate the center point of all the fluffs to be his new position
                        //Calculate the average velocity to be his new velocity

                        float avgX =  _fluffs.Average<FluffWrapper>(f => f.Fluff.Position.X);
                        float avgY =  _fluffs.Average<FluffWrapper>(f => f.Fluff.Position.Y);

                        float avgVelocityX =  _fluffs.Average<FluffWrapper>(f => f.Fluff.Velocity.X);
                        float avgVelocityY =  _fluffs.Average<FluffWrapper>(f => f.Fluff.Velocity.Y);

                        teddy.SetVelocity(new Vector2(avgVelocityX, avgVelocityY));
                        teddy.Position = new Vector2(avgX, avgY);

                        foreach (FluffWrapper fw in _fluffs)
                            fw.Fluff.SetApplyGravity(false);

                    }

                    break;
            }

            _fluffMotionType = t;

        }

        private Keys? _keyDown;

        public override GeometryMethods.RectangleF GetExpandedPowerupRectangle(Teddy teddy)
        {
            if (_fluffMotionType == FluffMotionTypeEnum.Collapsing)
                return new GeometryMethods.RectangleF(teddy.Position.X, teddy.Position.Y, teddy.FrameSize.X, _fluffBox.Height);
            else
                return base.GetExpandedPowerupRectangle(teddy);
        }
        //{
        //    get
        //    {
        //        //var fluffRects = new List<GeometryMethods.RectangleF>();
        //        //foreach(FluffWrapper fw in _fluffs){
        //        //  fluffRects.Add( fw.Fluff.CollisionRectangle);
        //        //}

        //        //return new GeometryMethods.MultiRectangleF(base.TeddyRectangle.Width, base.TeddyRectangle.Height,   fluffRects.ToArray());

        //        return base.TeddyRectangle;
        //    }
        //}

        private Vector2 _preUpdatePosition;

        public override bool Update(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {

            if (_keyDown == null)
            {
                if (keyState.IsKeyDown(Keys.Tab))
                {
                    SetFluffMode(_fluffMotionType + 1, teddy);

                    if (_fluffMotionType > FluffMotionTypeEnum.Exploding)
                        SetFluffMode(0, teddy);

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


                if (keyState.IsKeyDown(_zipKey))
                {
                    if (_fluffMotionType != FluffMotionTypeEnum.Collapsing) { 
                        _keyDown = _zipKey;
                        teddy.SetPowerup(new ZipperPowerup(_game, teddy, true));
                        return false;
                    }
                }


                if (keyState.IsKeyDown(Keys.Down))
                {
                    _keyDown = Keys.Down;
                    SetFluffMode(FluffMotionTypeEnum.Collapsing, teddy);
                }

                if (keyState.IsKeyDown(Keys.Up))
                {
                    _keyDown = Keys.Up;
                    SetFluffMode(FluffMotionTypeEnum.Spring, teddy);
                }

                if (keyState.IsKeyDown(Keys.D1))
                {
                    _keyDown = Keys.D1;
                    SetFluffMode(FluffMotionTypeEnum.Exploding,teddy);

                }
                if (keyState.IsKeyDown(Keys.D2))
                {
                    _keyDown = Keys.D2;
                    if (_fluffMotionType == FluffMotionTypeEnum.Launching)
                        SetFluffMode(FluffMotionTypeEnum.Spring, teddy);                   
                    else
                        SetFluffMode(FluffMotionTypeEnum.Launching, teddy);                   
                }


            }else
            {
                if (keyState.IsKeyUp(_keyDown.Value))
                    _keyDown = null;
            }

           _preUpdatePosition = teddy.Position;

            return true;
        }



        public override void AfterUpdate(GameTime gameTime, Screen screen, Teddy teddy, KeyboardState keyState)
        {


            foreach (FluffWrapper fw in _fluffs)
            {
                int boneID = fw.BoneID;
                TeddySkeleton bone = GetCurrentTeddySkeleton(teddy).FirstOrDefault(b => b.ZOrder == boneID);

                //todo: figure out a better way to ensure the number of ref points is the same across bones
                if (bone.ReferencePoints.Count > fw.ReferenceID)
                {

                    Vector2 refPoint = bone.ReferencePoints[fw.ReferenceID];
                    Vector2 point = teddy.Facing == Teddy.Direction.Right ? refPoint : bone.GetFlippedPoint(50, refPoint);

                    if (_fluffMotionType == FluffMotionTypeEnum.Collapsing)
                    {
                        refPoint = new Vector2(refPoint.X, 3);
                        point = new Vector2(point.X, 3);
                    }

                    //We want the fluff to be centered around its ref point, not with its topright on the point
                    Vector2 fluffOffset = new Vector2(_fluffBox.Width / 2, _fluffBox.Height / 2);

                    refPoint = teddy.Position + point - fluffOffset;
                    Vector2 vRF =  fw.Fluff.Position - refPoint; //Vector from ref point to fluff
                    float dist = vRF.Length();
                    vRF.Normalize();

                    Vector2 fSpring;
                    Vector2 fTotal;
                    Vector2 fd;

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

                            
                             fSpring = -_k * vRF * dist;
                            if (dist == 0)
                            {
                                fSpring = Vector2.Zero;
                            }

                            
                             fd = -_c * fw.Fluff.Velocity;
                             fTotal = fSpring + fd;

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
                            
                          //  fSpring = -_k * vRF * dist;
                          //  if (dist == 0)
                          //  {
                          //      fSpring = Vector2.Zero;
                          //  }

                            
                          //   fd = -_c * fw.Fluff.Velocity;
                          //   fTotal = fSpring + fd;
                          //   fTotal = fTotal / _m;



                          //  //We only want to apply the X direction force
                          ////   fTotal.Y = fw.Fluff.Acceleration.Y;

                          //  fw.Fluff.SetAccelleration(fTotal );


                            //How for did teddy move this frame
                            Vector2 teddyMovement = teddy.Position - _preUpdatePosition;
                            fw.Fluff.SetPosition(fw.Fluff.Position + new Vector2(teddyMovement.X,0));

                            fw.FluffRotation += teddyMovement.X / (_fluffBox.Width / 2);

                            break;

                    }
             
                }


                Vector2 prevPosition = fw.Fluff.Position;
                
                fw.Fluff.Update(gameTime);

                Vector2 fluffMovement = fw.Fluff.Position - prevPosition;

                fw.FluffRotation += (double) fluffMovement.X / (_fluffBox.Width / 2);

            }

        }

        public override bool BeforeDraw(GameTime gameTime, SpriteBatch teddyBatch, Teddy teddy)
        {

            //todo: apply angular accel with damping to have each fluff spin down
           // _fluffRotation += .5f;



#if COLLISIONS
            _pixelSprite.SetData<Color>(new Color[] { Color.Red });
            teddyBatch.Draw(_pixelSprite, teddy.Position, teddy.TeddyRectangle.AsRect(), Color.Red);
#endif

            DrawFluff(teddyBatch, gameTime);

            return _showTeddyOutline;


        }


        private void CreateFluffs(Teddy teddy)
       
       {

           var _oldFluffs = _fluffs;

            _fluffs = new List<FluffWrapper>();

            bool applyGravity = (_fluffMotionType == FluffMotionTypeEnum.Collapsing);

            //All frames have teh same makeup, so we can just use whichever is current
            List<TeddySkeleton> skeleton = GetCurrentTeddySkeleton(teddy);

            foreach (TeddySkeleton bone in skeleton)
            {
                for (int i = 0; i < bone.ReferencePoints.Count; i++)
                {

                    Vector2 refPoint = bone.ReferencePoints[i];
                    Vector2 point = teddy.Facing == Teddy.Direction.Right ? refPoint : bone.GetFlippedPoint(50, refPoint);

                    if (_fluffMotionType == FluffMotionTypeEnum.Collapsing)
                        refPoint = new Vector2(teddy.Position.X + point.X, teddy.Position.Y + _fluffBox.Height / 2);  
                    else
                        refPoint = teddy.Position + point;



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

                    _fluffs.Add(new FluffWrapper(bone.ZOrder, i, refPoint, initialVelocity , _game, new Rectangle(0, 0, _fluffBox.Width / 2, _fluffBox.Height / 2), applyGravity));
                }
            }
        }

        public void ResetFluff(Teddy teddy)
        {
            _fluffs = null;
            CreateFluffs(teddy);
        }

        private void ReevaluateSkeletonAndFluff(Teddy teddy)
        {

            //Quick and dirty implementation' throw everything out and start over
            _spriteSkeletons.Clear();

            CreateFluffs(teddy);


        }


        private List<TeddySkeleton> GetCurrentTeddySkeleton(Teddy teddy)
        {

            if (_spriteSkeletons.ContainsKey(teddy.SpriteState))
            {
                return _spriteSkeletons[teddy.SpriteState];
            }
            else
            {

                var skeleton = new List<TeddySkeleton>();
                _spriteSkeletons.Add(teddy.SpriteState, skeleton);

                switch (teddy.SpriteState)
                {
                    case Teddy.TeddySpriteState.Run1:

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(46, 45), new Vector2(21,51), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(6, 71), new Vector2(20,50), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(42,70), new Vector2(29,59), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(5, 58), new Vector2(19, 47), 10, 6)); //near arm

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear

                        break;
                    case Teddy.TeddySpriteState.Run2:
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(42, 49), new Vector2(21,51), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(10,72), new Vector2(21,59), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(31,60), new Vector2(31,72), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(20, 58), new Vector2(18, 47), 10, 6)); // near arm

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear

                        break;
                    case Teddy.TeddySpriteState.Run3:
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(7, 57), new Vector2(20,48), 10, 1)); //far arm
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43,72), new Vector2(30,59), 10, 2)); //far leg

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 53), 14, 30, 3)); //body
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 25), 20, 30, 4)); //head

                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(20,61), new Vector2(8,72), 10, 5)); // near leg
                        skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(44, 46), new Vector2(26, 51), 10, 6)); //near arm

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(22, 6), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(30, 4), 5,  5, 8)); //ear
                        break;
                    case Teddy.TeddySpriteState.Blink1:
                    case Teddy.TeddySpriteState.Blink2:
                    case Teddy.TeddySpriteState.Blink3:
                    case Teddy.TeddySpriteState.Blink4:
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 55), 14, 30, 3)); //Body

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(8,73), new Vector2(18,64), 10, 2));//left leg
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43,73), new Vector2(30,64), 10, 5)); //right leg

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(43, 64), new Vector2(43, 47), 10, 1));//left arm
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Straight, new Vector2(6, 64), new Vector2(6, 47), 10, 6)); //right arm

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 25),  20, 30, 4)); //Head

                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(8, 7), 5,  5, 7)); //ear
                       skeleton.Add(new TeddySkeleton(TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(42, 7), 5,  5, 8)); //ear
                        break;
                    case Teddy.TeddySpriteState.BlinkNoArms:

                        skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 55), 12, 30, 3)); //Body

                        //skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.SkeletonTypeEnum.Straight, new Vector2(8, 73), new Vector2(18, 64), 10, 2));//left leg
                        //skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.SkeletonTypeEnum.Straight, new Vector2(43, 73), new Vector2(30, 64), 10, 5)); //right leg

                        //skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.SkeletonTypeEnum.Straight, new Vector2(43, 64), new Vector2(43, 47), 10, 1));//left arm
                        //skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.SkeletonTypeEnum.Straight, new Vector2(6, 64), new Vector2(6, 47), 10, 6)); //right arm

                        skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(24, 25), 12, 25, 4)); //Head

                        skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(8, 7), 5, 5, 7)); //ear
                        skeleton.Add(new TeddysAdventureLibrary.FluffPowerup.TeddySkeleton(TeddysAdventureLibrary.FluffPowerup.TeddySkeleton.SkeletonTypeEnum.Circle, new Vector2(42, 7), 5, 5, 8)); //ear

                        break;

                }

                foreach (TeddySkeleton ts in skeleton)
                {
                    if ( _fluffs == null || _fluffs.Count == 0 )
                        ts.SetReferencePoints(teddy.CurrentFluff, 6, null);
                    else
                    {
                        int actualFluffCount = _fluffs.Count(c => c.BoneID == ts.ZOrder);
                        ts.SetReferencePoints(teddy.CurrentFluff, 6, actualFluffCount);
                    }

                }
         

                return skeleton;

            }

        }



        public void DrawFluff(SpriteBatch teddyBatch, GameTime gameTime)
        {

            Vector2 origin = new Vector2(_fluffBox.Width , _fluffBox.Height );

            foreach (FluffWrapper fw in _fluffs)
            {
                teddyBatch.Draw(_fluffSprite, fw.Fluff.Position + origin/4   , null, Color.White, (float)fw.FluffRotation, origin  , .5f, SpriteEffects.None, (float)0.0f);

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

            public double FluffRotation { get; set; }

            public FluffWrapper(int boneID, int referenceID, Vector2 startPosition, Vector2 initialVelocity, Game g, Rectangle box, bool applyGravity)
            {
                this.BoneID = boneID;
                this.ReferenceID = referenceID;
                this.Fluff = new Fluff(g, startPosition, false, 0, 0, box, true,  false);
                this.Fluff.SetVelocity(initialVelocity);
                this.FluffRotation = 0;
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

