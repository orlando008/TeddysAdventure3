using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;

namespace TeddyMapEditor
{
    public class Surface:IMovableGameItem
    {
        private string _surfaceTexture;
      //  private string _name;

        public string SurfaceTexture
        {
          get { return _surfaceTexture; }
          set { _surfaceTexture = value; }
        }

        public Rectangle SurfaceBounds { get; set; }

        public Rectangle Parent
        {
            get { return this.SurfaceBounds; }
            set { this.SurfaceBounds = value; }
        }

        public Point Location
        {
            get 
            {
                return new Point((double)SurfaceBounds.GetValue(System.Windows.Controls.Canvas.LeftProperty), (double)SurfaceBounds.GetValue(System.Windows.Controls.Canvas.TopProperty));
            }
            set 
            {
                this.SurfaceBounds.SetValue(System.Windows.Controls.Canvas.LeftProperty, value.X);
                this.SurfaceBounds.SetValue(System.Windows.Controls.Canvas.TopProperty, value.Y);
            }
        }


        public Surface()
        {
            _surfaceTexture = "SurfaceTexture1";
        }

        public string GetXMLString()
        {
            string s = "";
            s += "<Item>" + System.Environment.NewLine;
            s += "<Rect>" +  Convert.ToInt32((this.SurfaceBounds).GetValue(System.Windows.Controls.Canvas.LeftProperty)).ToString() + " " + Convert.ToInt32((this.SurfaceBounds).GetValue(System.Windows.Controls.Canvas.TopProperty)).ToString() + " " + Convert.ToInt32(((Rectangle)this.SurfaceBounds).Width).ToString() + " " + Convert.ToInt32(((Rectangle)this.SurfaceBounds).Height).ToString()+ "</Rect>" + System.Environment.NewLine;
            s += "<Sprite>" + _surfaceTexture + "</Sprite>" + System.Environment.NewLine;
            s += "</Item>" + System.Environment.NewLine;

            return s;
        }
    }
}
