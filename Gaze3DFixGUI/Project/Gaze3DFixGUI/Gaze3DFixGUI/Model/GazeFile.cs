using System;
using System.Collections.Generic;

namespace Gaze3DFixGUI.Model
{
    class GazeFile
    {
        public String filename = "";

        public List<GazeData> list_GazeData = new List<GazeData>();

        public GazeFile ()
        {

        }

        public void addGazeData(GazeData gazedata)
        {
            list_GazeData.Add(gazedata);
        }

        public void printGazeFile()
        {
            Console.WriteLine("GazeData:");
            Console.WriteLine("Identifier,Participant,EyePosX_left,EyePosY_left,EyePosZ_left,EyePosX_right,EyePosY_right,EyePosZ_right,GazePosX_left,GazePosY_left,GazePosZ_left,GazePosX_right,GazePosY_right,GazePosZ_right");
            foreach(GazeData gazedata in list_GazeData)
            {
                Console.WriteLine(gazedata.getIdentifier() + ", " 
                    + gazedata.getParticipant() + "," 
                    + gazedata.getEyePosXLeft() + "," 
                    + gazedata.getEyePosYLeft() + "," 
                    + gazedata.getEyePosZLeft() + "," 
                    + gazedata.getEyePosXRight() + "," 
                    + gazedata.getEyePosYRight() + "," 
                    + gazedata.getEyePosZRight() + "," 
                    + gazedata.getGazePosXLeft() + "," 
                    + gazedata.getGazePosXRight() + "," 
                    + gazedata.getGazePosYLeft() + "," 
                    + gazedata.getGazePosYRight() + "," 
                    + gazedata.getGazePosZLeft() + "," 
                    + gazedata.getGazePosZRight());
            }
        }
    }
}