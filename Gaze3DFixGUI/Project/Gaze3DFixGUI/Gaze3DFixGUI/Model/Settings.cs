using System;
using System.Collections.Generic;

namespace Gaze3DFixGUI.Model
{
    [Serializable]
    public class Settings
    {
        private List<String> paths = new List<String>();
        private bool gaze = true;
        private bool fixations = true;

        private bool header = true;
        private bool delimiter_semicolon = true;
        private bool delimiter_tabulator = false;
        private int numberOfColumns = 14;
        private bool includeIdentifier = false;
        private int column_Identifier = 1;
        private bool includeParticipant = false;
        private int column_Participant = 2;
        private bool includeValiditiy = false;
        private int column_Validitiy = 15;
        private int column_EyePosX_left = 9;
        private int column_EyePosY_left = 11;
        private int column_EyePosZ_left = 13;
        private int column_EyePosX_right = 10;
        private int column_EyePosY_right = 12;
        private int column_EyePosZ_right = 14;
        private int column_GazePosX_left = 3;
        private int column_GazePosY_left = 5;
        private int column_GazePosZ_left = 7;
        private int column_GazePosX_right = 4;
        private int column_GazePosY_right = 6;
        private int column_GazePosZ_right = 8;

        private int column_Gaze3DX = 3;
        private int column_Gaze3DY = 4;
        private int column_Gaze3DZ = 5;

        private int samples = 6;
        private int frequency = 60;
        private int threshold_duration = 100;
        private double threshold_dispersion = 2.0;

        private int maxNumberOutliers = 1;
        private int maxNumberInvalids = 3;

        private bool includeCaseCrossing = true;
        private bool includeFirstOngoingFixation = true;

        private bool outputCSV = true;
        private bool outputTXT = false;             
        private bool openOutputFile = false;

        public Settings() { }

        public List<String> Paths
        {
            get { return paths; }
            set { paths = value; }
        }

        public bool Gaze
        {
            get { return gaze; }
            set { gaze = value; }
        }

        public bool Fixations
        {
            get { return fixations; }
            set { fixations = value; }
        }

        public bool Header
        {
            get { return header; }
            set { header = value; }
        }

        public bool Delimiter_Semicolon
        {
            get { return delimiter_semicolon; }
            set { delimiter_semicolon = value; }
        }

        public bool Delimiter_Tabulator
        {
            get { return delimiter_tabulator; }
            set { delimiter_tabulator = value; }
        }

        public int NumberOfColums
        {
            get { return numberOfColumns; }
            set { numberOfColumns = value; }
        }

        public bool IncludeIdentifier
        {
            get { return includeIdentifier; }
            set { includeIdentifier = value; }
        }

        public int Column_Identifier
        {
            get { return column_Identifier; }
            set { column_Identifier = value; }
        }

        public bool IncludeParticipant
        {
            get { return includeParticipant; }
            set { includeParticipant = value; }
        }

        public int Column_Participant
        {
            get { return column_Participant; }
            set { column_Participant = value; }
        }

        public bool IncludeValidity
        {
            get { return includeValiditiy; }
            set { includeValiditiy = value; }
        }

        public int Column_Validity
        {
            get { return column_Validitiy; }
            set { column_Validitiy = value; }
        }

        public int Column_EyePosX_left
        {
            get { return column_EyePosX_left; }
            set { column_EyePosX_left = value; }
        }

        public int Column_EyePosY_left
        {
            get { return column_EyePosY_left; }
            set { column_EyePosY_left = value; }
        }

        public int Column_EyePosZ_left
        {
            get { return column_EyePosZ_left; }
            set { column_EyePosZ_left = value; }
        }

        public int Column_EyePosX_right
        {
            get { return column_EyePosX_right; }
            set { column_EyePosX_right = value; }
        }

        public int Column_EyePosY_right
        {
            get { return column_EyePosY_right; }
            set { column_EyePosY_right = value; }
        }

        public int Column_EyePosZ_right
        {
            get { return column_EyePosZ_right; }
            set { column_EyePosZ_right = value; }
        }

        public int Column_GazePosX_left
        {
            get { return column_GazePosX_left; }
            set { column_GazePosX_left = value; }
        }

        public int Column_GazePosY_left
        {
            get { return column_GazePosY_left; }
            set { column_GazePosY_left = value; }
        }

        public int Column_GazePosZ_left
        {
            get { return column_GazePosZ_left; }
            set { column_GazePosZ_left = value; }
        }

        public int Column_GazePosX_right
        {
            get { return column_GazePosX_right; }
            set { column_GazePosX_right = value; }
        }

        public int Column_GazePosY_right
        {
            get { return column_GazePosY_right; }
            set { column_GazePosY_right = value; }
        }

        public int Column_GazePosZ_right
        {
            get { return column_GazePosZ_right; }
            set { column_GazePosZ_right = value; }
        }

        public int Column_Gaze3DX
        {
            get { return column_Gaze3DX; }
            set { column_Gaze3DX = value; }
        }

        public int Column_Gaze3DY
        {
            get { return column_Gaze3DY; }
            set { column_Gaze3DY = value; }
        }

        public int Column_Gaze3DZ
        {
            get { return column_Gaze3DZ; }
            set { column_Gaze3DZ = value; }
        }

        public int Samples
        {
            get { return samples; }
            set { samples = value; }
        }

        public int Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        public int ThresholdDuration
        {
            get { return threshold_duration; }
            set { threshold_duration = value; }
        }

        public double ThresholdDispersion
        {
            get { return threshold_dispersion; }
            set { threshold_dispersion = value; }
        }

        public int MaxNumberOutliers
        {
            get { return maxNumberOutliers; }
            set { maxNumberOutliers = value; }
        }

        public int MaxNumberInvalids
        {
            get { return maxNumberInvalids; }
            set { maxNumberInvalids = value; }
        }

        public bool IncludeCaseCrossing
        {
            get { return includeCaseCrossing; }
            set { includeCaseCrossing = value; }
        }

        public bool IncludeFirstOngoingFixation
        {
            get { return includeFirstOngoingFixation; }
            set { includeFirstOngoingFixation = value; }
        }

        public bool Output_CSV
        {
            get { return outputCSV; }
            set { outputCSV = value; }
        }

        public bool Output_TXT
        {
            get { return outputTXT; }
            set { outputTXT = value; }
        }

        public bool OpenOutputFile
        {
            get { return openOutputFile; }
            set { openOutputFile = value; }
        }
    }
}