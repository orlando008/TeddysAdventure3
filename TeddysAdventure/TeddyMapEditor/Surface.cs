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

        public string SurfaceTexture
        {
          get { return _surfaceTexture; }
          set { _surfaceTexture = value; }
        }

    }
}
