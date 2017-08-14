using System;
using System.IO;
using System.Linq;
using Gaze3DFixGUI.Model;
using System.Globalization;

namespace Gaze3DFixGUI.Controller
{
    static class InputFileController
    {
        private static NumberFormatInfo nfi = new CultureInfo("de-DE").NumberFormat;

        public static GazeFile newGazeFile(
                    String inputfilepath,
                    int delimiter,
                    bool includeIdentifier,
                    int column_Identifier,
                    bool includeParticipant,
                    int column_Participant,
                    bool includeValidity,
                    int column_Validity,
                    bool header,
                    int column_EyePosX_left,
                    int column_EyePosY_left,
                    int column_EyePosZ_left,
                    int column_EyePosX_right,
                    int column_EyePosY_right,
                    int column_EyePosZ_right,
                    int column_GazePosX_left,
                    int column_GazePosY_left,
                    int column_GazePosZ_left,
                    int column_GazePosX_right,
                    int column_GazePosY_right,
                    int column_GazePosZ_right)
        {
            GazeFile gazefile = new GazeFile();
            // split filename
            char[] separators = new char[2];
            separators[0] = Convert.ToChar(92);                       // \
            separators[1] = Convert.ToChar(46);                       // point
            string[] file = inputfilepath.Split(separators);
            gazefile.filename = file[file.Length - 2];

            // streamreader
            try
            {
                StreamReader fileReader = new StreamReader(File.OpenRead(inputfilepath));
                string currentLine = String.Empty;
                nfi.NumberDecimalSeparator = ",";
                nfi.NumberGroupSeparator = "";
                char split = Convert.ToChar(delimiter);
                bool headline = header;
                int columns;
                int linecount = 0;

                int id = 0;
                String identifier;
                String participant;
                int validity;
                int eyePosX_left;
                int eyePosY_left;
                int eyePosZ_left;
                int eyePosX_right;
                int eyePosY_right;
                int eyePosZ_right;
                int gazePosX_left;
                int gazePosY_left;
                int gazePosZ_left;
                int gazePosX_right;
                int gazePosY_right;
                int gazePosZ_right;

                while ((currentLine = fileReader.ReadLine()) != null)
                {
                    linecount++;
                    string[] words = currentLine.Split(split);
                    columns = words.Count();

                    if (headline)
                    {
                        headline = false;
                    }
                    else
                    {
                        id++;

                        if(includeParticipant)
                        {
                            participant = words[column_Participant - 1];
                        }
                        else
                        {
                            participant = "";
                        }
                        if(includeIdentifier)
                        {
                            identifier = words[column_Identifier - 1];
                        }
                        else
                        {
                            identifier = "";
                        }
                        if (includeValidity)
                        {
                            validity = Convert.ToInt32(words[column_Validity - 1], nfi);
                        }
                        else
                        {
                            validity = 1;
                        }
                        eyePosX_left = Convert.ToInt32(double.Parse(words[column_EyePosX_left - 1], nfi));
                        eyePosY_left = Convert.ToInt32(double.Parse(words[column_EyePosY_left - 1], nfi));
                        eyePosZ_left = Convert.ToInt32(double.Parse(words[column_EyePosZ_left - 1], nfi));
                        eyePosX_right = Convert.ToInt32(double.Parse(words[column_EyePosX_right - 1], nfi));
                        eyePosY_right = Convert.ToInt32(double.Parse(words[column_EyePosY_right - 1], nfi));
                        eyePosZ_right = Convert.ToInt32(double.Parse(words[column_EyePosZ_right - 1], nfi));
                        gazePosX_left = Convert.ToInt32(double.Parse(words[column_GazePosX_left - 1], nfi));
                        gazePosY_left = Convert.ToInt32(double.Parse(words[column_GazePosY_left - 1], nfi));
                        gazePosZ_left = Convert.ToInt32(double.Parse(words[column_GazePosZ_left - 1], nfi));
                        gazePosX_right = Convert.ToInt32(double.Parse(words[column_GazePosX_right - 1], nfi));
                        gazePosY_right = Convert.ToInt32(double.Parse(words[column_GazePosY_right - 1], nfi));
                        gazePosZ_right = Convert.ToInt32(double.Parse(words[column_GazePosZ_right - 1], nfi));

                        GazeData gazedata = new GazeData(
                            id,
                            identifier,
                            participant,
                            validity,
                            eyePosX_left,
                            eyePosY_left, 
                            eyePosZ_left, 
                            eyePosX_right, 
                            eyePosY_right,
                            eyePosZ_right, 
                            gazePosX_left, 
                            gazePosY_left, 
                            gazePosZ_left, 
                            gazePosX_right, 
                            gazePosY_right, 
                            gazePosZ_right);
                        gazefile.addGazeData(gazedata);
                    }
                }
            }
            catch
            {

            }
            return gazefile;
        }

        public static GazeFile3D newGazeFile3D(
                    String inputfilepath,
                    int delimiter,
                    bool includeIdentifier,
                    int column_Identifier,
                    bool includeParticipant,
                    int column_Participant,
                    bool includeValidity,
                    int column_Validity,
                    bool header,
                    int column_EyePosX_left,
                    int column_EyePosY_left,
                    int column_EyePosZ_left,
                    int column_EyePosX_right,
                    int column_EyePosY_right,
                    int column_EyePosZ_right,
                    int column_Gaze3DX,
                    int column_Gaze3DY,
                    int column_Gaze3DZ)
        {
            GazeFile3D gazefile3d = new GazeFile3D();

            // split filename
            char[] separators = new char[2];
            separators[0] = Convert.ToChar(92);                       // \
            separators[1] = Convert.ToChar(46);                       // point
            string[] file = inputfilepath.Split(separators);
            gazefile3d.filename = file[file.Length - 2];

            // streamreader
            try
            {
                StreamReader fileReader = new StreamReader(inputfilepath);
                string currentLine = String.Empty;
                nfi.NumberDecimalSeparator = ",";
                nfi.NumberGroupSeparator = ""; 
                char split = Convert.ToChar(delimiter);
                bool headline = header;
                int columns;
                int linecount = 0;

                int id = 0;
                String identifier;
                String participant;
                int validity;
                int eyePosX_left;
                int eyePosY_left;
                int eyePosZ_left;
                int eyePosX_right;
                int eyePosY_right;
                int eyePosZ_right;

                int gaze3DX;
                int gaze3DY;
                int gaze3DZ;

                while ((currentLine = fileReader.ReadLine()) != null)
                {
                    linecount++;
                    string[] words = currentLine.Split(split);
                    columns = words.Count();

                    if (headline)
                    {
                        headline = false;
                    }
                    else
                    {
                        id++;

                        if (includeIdentifier)
                        {
                            identifier = words[column_Identifier - 1];
                        }
                        else
                        {
                            identifier = "";
                        }
                        if (includeParticipant)
                        {
                            participant = words[column_Participant - 1];
                        }
                        else
                        {
                            participant = "";
                        }
                        if (includeValidity)
                        {
                            validity = Convert.ToInt32(words[column_Validity - 1], nfi);
                        }
                        else
                        {
                            validity = 1;
                        }
                        eyePosX_left = Convert.ToInt32(double.Parse(words[column_EyePosX_left - 1], nfi));
                        eyePosY_left = Convert.ToInt32(double.Parse(words[column_EyePosY_left - 1], nfi));
                        eyePosZ_left = Convert.ToInt32(double.Parse(words[column_EyePosZ_left - 1], nfi));
                        eyePosX_right = Convert.ToInt32(double.Parse(words[column_EyePosX_right - 1], nfi));
                        eyePosY_right = Convert.ToInt32(double.Parse(words[column_EyePosY_right - 1], nfi));
                        eyePosZ_right = Convert.ToInt32(double.Parse(words[column_EyePosZ_right - 1], nfi));

                        GazeData gazedata = new GazeData(
                            id,
                            identifier,
                            participant,
                            validity,
                            eyePosX_left,
                            eyePosY_left,
                            eyePosZ_left,
                            eyePosX_right,
                            eyePosY_right,
                            eyePosZ_right,
                            -1,
                            -1,
                            -1,
                            -1,
                            -1,
                            -1);

                        gaze3DX = Convert.ToInt32(double.Parse(words[column_Gaze3DX - 1], nfi));
                        gaze3DY = Convert.ToInt32(double.Parse(words[column_Gaze3DY - 1], nfi));
                        gaze3DZ = Convert.ToInt32(double.Parse(words[column_Gaze3DZ - 1], nfi));

                        GazeData3D gazedata3d = new GazeData3D(
                            gazedata,
                            gaze3DX,
                            gaze3DY,
                            gaze3DZ);
                        gazefile3d.addGazeData3D(gazedata3d);
                    }
                }
            }
            catch
            {

            }
            return gazefile3d;
        }
    }
}