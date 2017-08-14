using System;

namespace Gaze3DFixGUI.Model
{
    public class GazeData3D
    {
        private GazeData gazedata;
        private int gaze3DX;
        private int gaze3DY;
        private int gaze3DZ;

        public GazeData3D(
            GazeData gazedata,
            int gaze3DX,
            int gaze3DY,
            int gaze3DZ)
        {
            this.gazedata = gazedata;
            this.gaze3DX = gaze3DX;
            this.gaze3DY = gaze3DY;
            this.gaze3DZ = gaze3DZ;
        }

        public GazeData getGazeData()
        {
            return gazedata;
        }

        public int getGaze3DX()
        {
            return gaze3DX;
        }

        public int getGaze3DY()
        {
            return gaze3DY;
        }

        public int getGaze3DZ()
        {
            return gaze3DZ;
        }

        public String getGazeData3DString(bool includeIdentifier, bool includeParticipant, bool includeValidity, String delimiter)
        {
            String gaze3d = "";
            gaze3d = gazedata.getGazeDataString(includeIdentifier, includeParticipant, includeValidity, delimiter);
            gaze3d = (gaze3d + delimiter + gaze3DX.ToString() + delimiter + gaze3DY.ToString() + delimiter + gaze3DZ.ToString());
            return gaze3d;
        }
    }
}