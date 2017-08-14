using System;

namespace Gaze3DFixGUI.Model
{
    public class GazeData
    {
        private int id;
        private String identifier = "";
        private String participant = "";
        private int validity;
        private int eyePosX_left;
        private int eyePosY_left;
        private int eyePosZ_left;
        private int eyePosX_right;
        private int eyePosY_right;
        private int eyePosZ_right;
        private int gazePosX_left;
        private int gazePosY_left;
        private int gazePosZ_left;
        private int gazePosX_right;
        private int gazePosY_right;
        private int gazePosZ_right;

        public GazeData(
            int id,
            String identifier,
            String participant,  
            int validity,
            int eyePosX_left, 
            int eyePosY_left, 
            int eyePosZ_left, 
            int eyePosX_right, 
            int eyePosY_right, 
            int eyePosZ_right,
            int gazePosX_left,
            int gazePosY_left,
            int gazePosZ_left,
            int gazePosX_right,
            int gazePosY_right,
            int gazePosZ_right)
        {
            this.id = id;
            this.identifier = identifier;
            this.participant = participant;
            this.validity = validity;
            this.eyePosX_left = eyePosX_left;
            this.eyePosY_left = eyePosY_left;
            this.eyePosZ_left = eyePosZ_left;
            this.eyePosX_right = eyePosX_right;
            this.eyePosY_right = eyePosY_right;
            this.eyePosZ_right = eyePosZ_right;
            this.gazePosX_left = gazePosX_left;
            this.gazePosY_left = gazePosY_left;
            this.gazePosZ_left = gazePosZ_left;
            this.gazePosX_right = gazePosX_right;
            this.gazePosY_right = gazePosY_right;
            this.gazePosZ_right = gazePosZ_right;
        }

        public int getID()
        {
            return id;
        }

        public String getIdentifier()
        {
            return identifier;
        }

        public String getParticipant()
        {
            return participant;
        }

        public int getValidity()
        {
            return validity;
        }

        public int getEyePosXLeft()
        {
            return eyePosX_left;
        }

        public int getEyePosYLeft()
        {
            return eyePosY_left;
        }

        public int getEyePosZLeft()
        {
            return eyePosZ_left;
        }

        public int getEyePosXRight()
        {
            return eyePosX_right;
        }

        public int getEyePosYRight()
        {
            return eyePosY_right;
        }

        public int getEyePosZRight()
        {
            return eyePosZ_right;
        }

        public int getGazePosXLeft()
        {
            return gazePosX_left;
        }

        public int getGazePosYLeft()
        {
            return gazePosY_left;
        }

        public int getGazePosZLeft()
        {
            return gazePosZ_left;
        }

        public int getGazePosXRight()
        {
            return gazePosX_right;
        }

        public int getGazePosYRight()
        {
            return gazePosY_right;
        }

        public int getGazePosZRight()
        {
            return gazePosZ_right;
        }

        public String getGazeDataString(bool includeIdentifier,  bool includeParticipant, bool includeValidity, String delimiter)
        {
            String gaze = "";
            gaze = gaze + id.ToString() + delimiter;
            if (includeIdentifier)
            {
                gaze = gaze + identifier + delimiter;
            }
            if (includeParticipant)
            {
                gaze = gaze + participant + delimiter;
            }
            if (includeValidity)
            {
                gaze = gaze + validity + delimiter;
            }
            gaze = (gaze + eyePosX_left.ToString() + delimiter  
                        + eyePosY_left.ToString() + delimiter 
                        + eyePosZ_left.ToString() + delimiter 
                        + eyePosX_right.ToString() + delimiter 
                        + eyePosY_right.ToString() + delimiter 
                        + eyePosZ_right.ToString());
            return gaze;
        }
    }
}