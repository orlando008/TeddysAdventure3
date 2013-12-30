using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TeddysAdventureLibrary
{
    public class Background
    {

        public string BackgroundName { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public bool Scrolls { get; set; }
        public Vector2 Offset { get; set; }
        public bool Foreground { get; set; }
        public string Motion { get; set; }

        public Background(BackgroundHelper bh)
        {
            this.BackgroundName = bh.Image;
            this.RepeatX = bh.RepeatX;
            this.RepeatY = bh.RepeatY;
            this.Scrolls = bh.Scrolls;
            this.Offset = bh.Offset;
            this.Foreground = bh.Foreground;
            this.Motion = bh.Motion;
        }

        public Background()
        { }

        public void SetOffsets(int x, int y)
        {
            this.Offset = new Vector2(x, y);
        }

        public string GetXMLString()
        {
            string xml;

            xml = "<Item><Image>{0}</Image><RepeatX>{1}</RepeatX><RepeatY>{2}</RepeatY><Scrolls>{3}</Scrolls><Offset>{4}</Offset><Foreground>{5}</Foreground><Motion>{6}</Motion></Item>";

            return string.Format(xml, new object[] { this.BackgroundName, BoolToXML(this.RepeatX), BoolToXML(this.RepeatY), BoolToXML(this.Scrolls), VectorToXML( this.Offset), BoolToXML( this.Foreground), this.Motion  });

        }

        public Vector2 GetHoverOffset(GameTime gameTime, Texture2D texture)
        {

            int currentFrameX = 0;
            int currentFrameY = 0;

            if (string.IsNullOrEmpty(this.Motion)) { return Vector2.Zero; }

            string[] motionComponents = this.Motion.Split(' ');

            if (motionComponents.Length != 4) { return Vector2.Zero; }


            double xRate = double.Parse(motionComponents[0]);
            double yRate = double.Parse(motionComponents[2]);


            switch (motionComponents[1])
            {
                case "C":

                    currentFrameX =  (int)(gameTime.TotalGameTime.TotalSeconds * xRate) % (int)((texture.Width));

                    break;

                case "O":
                    currentFrameX = (int)(gameTime.TotalGameTime.TotalSeconds * xRate) % (int)((xRate));
                    if (currentFrameX > xRate / 2)
                        currentFrameX = (int)xRate - currentFrameX;

                    break;
            }


            switch (motionComponents[3])
            {
                case "C":

                    currentFrameY =  (int)(gameTime.TotalGameTime.TotalSeconds * yRate) % (int)((texture.Height));

                    break;

                case "O":
                    currentFrameY = (int)(gameTime.TotalGameTime.TotalSeconds * yRate) % (int)((yRate));
                    if (currentFrameY > yRate / 2)
                        currentFrameY = (int)yRate - currentFrameY;

                    break;
            }


            return new Vector2(currentFrameX, currentFrameY);
        }

        public string VectorToXML(Vector2 v)
        {
            return string.Format("{0} {1}", v.X.ToString(), v.Y.ToString());
        }

        public string BoolToXML(bool b)
        {
            return b ? "1" : "0";
        }

    }


    public class BackgroundHelper
    {

        public string Image { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public bool Scrolls { get; set; }
        public Vector2 Offset { get; set; }
        public bool Foreground { get; set; }
        public string Motion { get; set; }

        public BackgroundHelper() { }


    }

    /// <summary>
    /// Read a Map object from the content pipeline.
    /// </summary>
    public class BackgroundReader : ContentTypeReader<BackgroundHelper>
    {
        protected override BackgroundHelper Read(ContentReader input, BackgroundHelper existingInstance)
        {
            BackgroundHelper sHelper = existingInstance;

            if (sHelper == null)
            {
                sHelper = new BackgroundHelper();
            }

            sHelper.Image = input.ReadString();
            sHelper.RepeatX = input.ReadBoolean();
            sHelper.RepeatY = input.ReadBoolean();
            sHelper.Scrolls = input.ReadBoolean();
            sHelper.Offset = input.ReadObject<Vector2>();
            sHelper.Foreground = input.ReadBoolean();
            sHelper.Motion = input.ReadString();

            return sHelper;
        }
    }


}
