using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace TeddyMapEditor
{
    public class Surface
    {
        private string _surfaceTexture;
        private string _name;
        private Rectangle _surfaceBounds;

        public string SurfaceTexture
        {
          get { return _surfaceTexture; }
          set { _surfaceTexture = value; }
        }

        public Surface()
        {
            _surfaceTexture = "";
        }

        public string GetXMLString()
        {
            string s = "";
            s += "<Item>" + System.Environment.NewLine;
            s += "<Type>" + _name + "</Type>" + System.Environment.NewLine;
            s += "<Position>" + _surfaceBounds + "</Position>" + System.Environment.NewLine;
            s += "</Item>" + System.Environment.NewLine;

            return s;
        }
    }
}
