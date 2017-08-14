using System;

namespace Gaze3DFixGUI.Model
{
    public class FixationData
    {
        private int id;
        private String startIdentifier = "";
        private String endIdentifier = "";
        private String participant = "";

        private Single fixationPosX;
        private Single fixationPosY;
        private Single fixationPosZ;
        private Single ellipsoidRadiusX;
        private Single ellipsoidRadiusY;
        private Single ellipsoidRadiusZ;
        private Single ellipsoidYaw;
        private Single ellipsoidPitch;
        private int saccadeDurationSamples;
        private double saccadeDurationMS;
        private int fixationDurationSamples;
        private double fixationDurationMS; 
        private int startSampleID;
        private int endSampleID;

        public FixationData(
            int id,
            String startIdentifier,
            String endIdentifier,
            String participant,
            Single pfXFixDelayed,
            Single pfYFixDelayed,
            Single pfZFixDelayed,
            Single pfXEllipsoidRDelayed,
            Single pfYEllipsoidRDelayed,
            Single pfZEllipsoidRDelayed,
            Single pfEllipsoidYawDelayed,
            Single pfEllipsoidPitchDelayed,
            int piSacDurationDelayed,
            double sacDurationMS,
            int piFixDurationDelayed,
            double fixDurationMS,
            int startSampleID,
            int endSampleID)
        {
            this.id = id;
            this.startIdentifier = startIdentifier;
            this.endIdentifier = endIdentifier;
            this.participant = participant;
            this.fixationPosX = pfXFixDelayed;
            this.fixationPosY = pfYFixDelayed;
            this.fixationPosZ = pfZFixDelayed;
            this.ellipsoidRadiusX = pfXEllipsoidRDelayed;
            this.ellipsoidRadiusY = pfYEllipsoidRDelayed;
            this.ellipsoidRadiusZ = pfZEllipsoidRDelayed;
            this.ellipsoidYaw = pfEllipsoidYawDelayed;
            this.ellipsoidPitch = pfEllipsoidPitchDelayed;
            this.saccadeDurationSamples = piSacDurationDelayed;
            this.saccadeDurationMS = sacDurationMS;
            this.fixationDurationSamples = piFixDurationDelayed;
            this.fixationDurationMS = fixDurationMS;
            this.startSampleID = startSampleID;
            this.endSampleID = endSampleID;
        }

        public int getStartSampleID()
        {
            return startSampleID;
        }

        public int getEndSampleID()
        {
            return endSampleID;
        }

        public String getStartIdentifier()
        {
            return startIdentifier;
        }

        public String getEndIdentifer()
        {
            return endIdentifier;
        }

        public String getParticipant()
        {
            return participant;
        }

        public Single getFixationPosX()
        {
            return fixationPosX;
        }

        public Single getFixationPosY()
        {
            return fixationPosY;
        }

        public Single getFixationPosZ()
        {
            return fixationPosZ;
        }

        public Single getEllipsoidRadiusX()
        {
            return ellipsoidRadiusX;
        }

        public Single getEllipsoidRadiusY()
        {
            return ellipsoidRadiusY;
        }

        public Single getEllipsoidRadiusZ()
        {
            return ellipsoidRadiusZ;
        }

        public Single getEllipsoidYaw()
        {
            return ellipsoidYaw;
        }

        public Single getEllipsoidPitch()
        {
            return ellipsoidPitch;
        }

        public int getSaccadeDurationSamples()
        {
            return saccadeDurationSamples;
        }

        public double getSaccadeDurationMS()
        {
            return saccadeDurationMS;
        }

        public int getFixationDurationSamples()
        {
            return fixationDurationSamples;
        }

        public double getFixationDurationMS()
        {
            return fixationDurationMS;
        }

        public String getFixationDataString(bool includeIdentifier, bool includeParticipant, String delimiter)
        {
            String fixation = "";
            fixation = fixation + id.ToString() + delimiter;
            if (includeIdentifier)
            {
                fixation = fixation + startIdentifier + delimiter;
            }
            if (includeParticipant)
            {
                fixation = fixation + participant + delimiter;
            }
            fixation = (fixation + fixationPosX.ToString() + delimiter
                                    + fixationPosY.ToString() + delimiter
                                    + fixationPosZ.ToString() + delimiter
                                    + ellipsoidRadiusX.ToString() + delimiter
                                    + ellipsoidRadiusY.ToString() + delimiter
                                    + ellipsoidRadiusZ.ToString() + delimiter
                                    + ellipsoidYaw.ToString() + delimiter
                                    + ellipsoidPitch.ToString() + delimiter
                                    + saccadeDurationSamples.ToString() + delimiter
                                    + saccadeDurationMS.ToString() + delimiter
                                    + fixationDurationSamples.ToString() + delimiter
                                    + fixationDurationMS.ToString() + delimiter
                                    + startSampleID.ToString() + delimiter
                                    + endSampleID.ToString());
            return fixation;
        }

        public String getShortFixationDataString(String delimiter)
        {
            String fixation = "";
            fixation = fixation + id.ToString() + delimiter;
            fixation = (fixation + fixationPosX.ToString() + delimiter
                                    + fixationPosY.ToString() + delimiter
                                    + fixationPosZ.ToString() + delimiter
                                    + ellipsoidRadiusX.ToString() + delimiter
                                    + ellipsoidRadiusY.ToString() + delimiter
                                    + ellipsoidRadiusZ.ToString() + delimiter
                                    + ellipsoidYaw.ToString() + delimiter
                                    + ellipsoidPitch.ToString());
            return fixation;
        }
    }
}