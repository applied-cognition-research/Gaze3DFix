using System;
using System.Collections.Generic;

namespace Gaze3DFixGUI.Model
{
    class GazeFile3D
    {
        public String filename = "";

        public List<GazeData3D> list_GazeData3D = new List<GazeData3D>();

        public GazeFile3D ()
        {

        }

        public void addGazeData3D(GazeData3D gazedata3d)
        {
            list_GazeData3D.Add(gazedata3d);
        }
    }
}