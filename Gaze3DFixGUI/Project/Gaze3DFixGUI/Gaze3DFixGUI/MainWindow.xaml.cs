/*-----------------------------------------------------------------------------------------------------------------
 * Gaze3DFix
 * 
 * With Gaze3DFix you can calculate 3D gaze points and detect 3D fixations from binocular gaze data. 
 * Gaze3DFix is a simple graphical user interface for the offline use of the Gaze3D and Fixation3D libraries.
 * 
 * Version:     2016-05-30
 * Author:      Stefan Vogt 
 *              Engineering Psychology and Applied Cognitive Research, 
 *              Department of Psychology III, 
 *              Technische Universität Dresden, 
 *              Germany
 *---------------------------------------------------------------------------------------------------------------*/

using Gaze3DFixGUI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Gaze3DFixGUI.Controller;

namespace Gaze3DFixGUI
{
    /*-----------------------------------------------------------------------------------------------------------------
     * Main Window
     * 
     *---------------------------------------------------------------------------------------------------------------*/

    public partial class MainWindow : Window
    {
        // settings file
        private Settings settings = new Settings();
        private static NumberFormatInfo nfi = new CultureInfo("de-DE").NumberFormat;    
        // delegates
        public delegate void StatusDelegate(String status);                     //delegates status of calculation
        public delegate void ProgressBarValueDelegate(int v);                   //delegates current value of ProgressBar
        public delegate void ProgressBarMaxDelegate(int max);                   //delegates maximum value of ProgressBar                      
        public delegate void finishedDelegate();                                //delegates calculation finished signal
        // progressbar values
        private int progressBarMaxValue = 0;
        private int progressBarCurrentValue = 0;
        // calculation mode
        private bool gaze = true;
        private bool fixations = true;
        // input file paths
        private List<String> paths = new List<String>();
        // default delimiter
        private static int input_delimiter_ascii = 59;
        // list of loaded gaze input files
        List<GazeFile> gazefiles = new List<GazeFile>();
        // list of loaded or calculated 3d gaze files
        List<GazeFile3D> gazefiles3d = new List<GazeFile3D>();
        // list of calculated 3d fixation files
        List<FixationFile> fixationfiles = new List<FixationFile>();

        // import Gaze.dll function CalculateGaze3DFromEyePos
        [DllImport("Gaze.dll",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern int CalculateGaze3DFromEyePos(
                            int x_EyePos_Left, 
                            int y_EyePos_Left,
                            int z_EyePos_Left,
                            int x_GazePos_Left,
                            int y_GazePos_Left,
                            int z_GazePos_Left,
                            int x_EyePos_Right,
                            int y_EyePos_Right,
                            int z_EyePos_Right,
                            int x_GazePos_Right,
                            int y_GazePos_Right,
                            int z_GazePos_Right,
                            out int x_Gaze3D,
                            out int y_Gaze3D,
                            out int z_Gaze3D);

        // import Fixation3D.dll function Init3DFixation
        [DllImport("Fixation3D.dll",
           CallingConvention = CallingConvention.StdCall)]
        public static extern int Init3DFixation (
                            int iMinimumSamples,
                            int iMaxMissedSamples,
                            int iMaxOutSamples);
        // import Fixation3D.dll function Calculate3DFixation
        [DllImport("Fixation3D.dll",
           CallingConvention = CallingConvention.StdCall)]
        public static extern int Calculate3DFixation(
                            int bGazeVectorFound,
                            Single fXLeftEye,
                            Single fYLeftEye,
                            Single fZLeftEye,
                            Single fXRightEye,
                            Single fYRightEye,
                            Single fZRightEye,
                            Single fXGaze,
                            Single fYGaze,
                            Single fZGaze,
                            Single fAccuracyAngleRad,
                            int iMinimumSamples,
                            out int pbGazepointFoundDelayed,
                            out Single pfXGazeDelayed,
                            out Single pfYGazeDelayed,
                            out Single pfZGazeDelayed,
                            out Single pfXFixDelayed,
                            out Single pfYFixDelayed,
                            out Single pfZFixDelayed,
                            out Single pfXEllipsoidRDelayed,
                            out Single pfYEllipsoidRDelayed,
                            out Single pfZEllipsoidRDelayed,
                            out Single pfEllipsoidYawDelayed,
                            out Single pfEllipsoidPitchDelayed,
                            out int piSacDurationDelayed,
                            out int piFixDurationDelayed);


        public MainWindow()
        {
            InitializeComponent();

            // get settings and pass them to the UI controls
            // calculation mode
            gaze = settings.Gaze;
            fixations = settings.Fixations;
            if (gaze && fixations)
            {
                RB_GazeAndFixations.IsChecked = true;
                TI_Fixations.Visibility = Visibility.Visible;
            }
            else if (gaze && !fixations)
            {
                RB_Gaze.IsChecked = true;
                TI_Fixations.Visibility = Visibility.Collapsed;
            }
            else if (!gaze && fixations)
            {
                RB_Fixations.IsChecked = true;
                TI_Fixations.Visibility = Visibility.Visible;
            }

            // tab assign columns
            // header
            if (settings.Header == false)
            {
                CB_Header.IsChecked = false;
            }
            else
            {
                CB_Header.IsChecked = true;
            }

            // delimiter
            if (settings.Delimiter_Semicolon)
            {
                RB_DelimiterSemicolon.IsChecked = true;
                input_delimiter_ascii = 59;
            }
            if (settings.Delimiter_Tabulator)
            {
                RB_DelimiterTabulator.IsChecked = true;
                input_delimiter_ascii = 9;
            }

            // number of columns
            TB_NumberOfColumns.Text = settings.NumberOfColums.ToString();

            // identifier
            if (settings.IncludeIdentifier == false)
            {
                CB_Identifier.IsChecked = false;
                TB_Identifier.Opacity = 0.5;
                TB_Identifier.IsEnabled = false;
            }
            else
            {
                CB_Identifier.IsChecked = true;
                TB_Identifier.Opacity = 1;
                TB_Identifier.IsEnabled = true;
            }
            TB_Identifier.Text = settings.Column_Identifier.ToString();
            
            // participant
            if (settings.IncludeParticipant == false)
            {
                CB_Participant.IsChecked = false;
                TB_Participant.Opacity = 0.5;
                TB_Participant.IsEnabled = false;
            }
            else
            {
                CB_Participant.IsChecked = true;
                TB_Participant.Opacity = 1;
                TB_Participant.IsEnabled = true;
            }
            TB_Participant.Text = settings.Column_Participant.ToString();
            
            // validity
            if (settings.IncludeValidity == false)
            {
                CB_Validity.IsChecked = false;
                TB_Validity.Opacity = 0.5;
                TB_Validity.IsEnabled = false;
            }
            else
            {
                CB_Validity.IsChecked = true;
                TB_Validity.Opacity = 1;
                TB_Validity.IsEnabled = true;
            }
            TB_Validity.Text = settings.Column_Validity.ToString();

            // eye position in the uniform coordinate system
            TB_EyePosX_left.Text = settings.Column_EyePosX_left.ToString();
            TB_EyePosY_left.Text = settings.Column_EyePosY_left.ToString();
            TB_EyePosZ_left.Text = settings.Column_EyePosZ_left.ToString();
            TB_EyePosX_right.Text = settings.Column_EyePosX_right.ToString();
            TB_EyePosY_right.Text = settings.Column_EyePosY_right.ToString();
            TB_EyePosZ_right.Text = settings.Column_EyePosZ_right.ToString();
            // 2d gaze position in the uniform coordinate system
            TB_GazePosX_left.Text = settings.Column_GazePosX_left.ToString();
            TB_GazePosY_left.Text = settings.Column_GazePosY_left.ToString();
            TB_GazePosZ_left.Text = settings.Column_GazePosZ_left.ToString();
            TB_GazePosX_right.Text = settings.Column_GazePosX_right.ToString();
            TB_GazePosY_right.Text = settings.Column_GazePosY_right.ToString();
            TB_GazePosZ_right.Text = settings.Column_GazePosZ_right.ToString();

            // 3d gaze positions in the uniform coordinate system
            TB_Gaze3DX.Text = settings.Column_Gaze3DX.ToString();
            TB_Gaze3DY.Text = settings.Column_Gaze3DY.ToString();
            TB_Gaze3DZ.Text = settings.Column_Gaze3DZ.ToString();

            // show controls depending on calculation mode
            if (gaze && fixations)
            {
                L_GazePosX.IsEnabled = true;
                L_GazePosY.IsEnabled = true;
                L_GazePosZ.IsEnabled = true;
                TB_GazePosX_left.IsEnabled = true;
                TB_GazePosY_left.IsEnabled = true;
                TB_GazePosZ_left.IsEnabled = true;
                TB_GazePosX_right.IsEnabled = true;
                TB_GazePosY_right.IsEnabled = true;
                TB_GazePosZ_right.IsEnabled = true;

                L_Gaze3DX.IsEnabled = false;
                TB_Gaze3DX.IsEnabled = false;
                L_Gaze3DY.IsEnabled = false;
                TB_Gaze3DY.IsEnabled = false;
                L_Gaze3DZ.IsEnabled = false;
                TB_Gaze3DZ.IsEnabled = false;
            }
            else if (gaze && !fixations)
            {
                L_GazePosX.IsEnabled = true;
                L_GazePosY.IsEnabled = true;
                L_GazePosZ.IsEnabled = true;
                TB_GazePosX_left.IsEnabled = true;
                TB_GazePosY_left.IsEnabled = true;
                TB_GazePosZ_left.IsEnabled = true;
                TB_GazePosX_right.IsEnabled = true;
                TB_GazePosY_right.IsEnabled = true;
                TB_GazePosZ_right.IsEnabled = true;

                L_Gaze3DX.IsEnabled = false;
                TB_Gaze3DX.IsEnabled = false;
                L_Gaze3DY.IsEnabled = false;
                TB_Gaze3DY.IsEnabled = false;
                L_Gaze3DZ.IsEnabled = false;
                TB_Gaze3DZ.IsEnabled = false;
            }
            else if (!gaze && fixations)
            {
                L_GazePosX.IsEnabled = false;
                L_GazePosY.IsEnabled = false;
                L_GazePosZ.IsEnabled = false;
                TB_GazePosX_left.IsEnabled = false;
                TB_GazePosY_left.IsEnabled = false;
                TB_GazePosZ_left.IsEnabled = false;
                TB_GazePosX_right.IsEnabled = false;
                TB_GazePosY_right.IsEnabled = false;
                TB_GazePosZ_right.IsEnabled = false;

                L_Gaze3DX.IsEnabled = true;
                TB_Gaze3DX.IsEnabled = true;
                L_Gaze3DY.IsEnabled = true;
                TB_Gaze3DY.IsEnabled = true;
                L_Gaze3DZ.IsEnabled = true;
                TB_Gaze3DZ.IsEnabled = true;
            }

            // tab fixation parameters
            // samples
            if (settings.Samples >= 3)
            {
                TB_Samples.Text = settings.Samples.ToString();
            }
            else
            {
                TB_Samples.Text = "3";
            }

            // frequency
            if (settings.Frequency >= 1)
            {
                TB_Frequency.Text = settings.Frequency.ToString();
            }
            else
            {
                TB_Frequency.Text = "1";
            }

            // dispersion
            TB_Dispersion.Text = settings.ThresholdDispersion.ToString();

            // outliers and invalids
            TB_Outliers.Text = settings.MaxNumberOutliers.ToString();
            TB_Invalids.Text = settings.MaxNumberInvalids.ToString();

            // include case crossing
            if (settings.IncludeCaseCrossing == false || settings.IncludeIdentifier == false)
            {
                CB_SpecialFixationCaseCrossing.IsChecked = false;
            }
            else
            {
                CB_SpecialFixationCaseCrossing.IsChecked = true;
            }

            // include ongoing fixation from input file start
            if (settings.IncludeFirstOngoingFixation == false)
            {
                CB_SpecialFixationCaseStartFile.IsChecked = false;
            }
            else
            {
                CB_SpecialFixationCaseStartFile.IsChecked = true;
            }

            // tab output
            // output file types
            if (settings.Output_CSV)
            {
                RB_OutputCSV.IsChecked = true;
            }
            if (settings.Output_TXT)
            {
                RB_OutputTXT.IsChecked = true;
            }

            // open output files
            if (settings.OpenOutputFile == false)
            {
                CB_OpenOutputFile.IsChecked = false;
            }
            else
            {
                CB_OpenOutputFile.IsChecked = true;
            }

            // init status and ProgressBar
            initStatus();
            initProgressBar(1);
        }

        // radiobutton gaze and fixation clicked
        private void GazeAndFixationsClicked(object sender, RoutedEventArgs e)
        {
            gaze = true;
            fixations = true;
            TI_Gaze.Visibility = Visibility.Visible;
            TI_Fixations.Visibility = Visibility.Visible;
            TI_Gaze.IsSelected = true;

            L_GazePosX.IsEnabled = true;
            L_GazePosY.IsEnabled = true;
            L_GazePosZ.IsEnabled = true;
            TB_GazePosX_left.IsEnabled = true;
            TB_GazePosY_left.IsEnabled = true;
            TB_GazePosZ_left.IsEnabled = true;
            TB_GazePosX_right.IsEnabled = true;
            TB_GazePosY_right.IsEnabled = true;
            TB_GazePosZ_right.IsEnabled = true;

            L_Gaze3DX.IsEnabled = false;
            TB_Gaze3DX.IsEnabled = false;
            L_Gaze3DY.IsEnabled = false;
            TB_Gaze3DY.IsEnabled = false;
            L_Gaze3DZ.IsEnabled = false;
            TB_Gaze3DZ.IsEnabled = false;
        }

        // radiobutton gaze only clicked
        private void GazeClicked(object sender, RoutedEventArgs e)
        {
            gaze = true;
            fixations = false;
            TI_Gaze.Visibility = Visibility.Visible;
            TI_Fixations.Visibility = Visibility.Collapsed;
            TI_Gaze.IsSelected = true;

            L_GazePosX.IsEnabled = true;
            L_GazePosY.IsEnabled = true;
            L_GazePosZ.IsEnabled = true;
            TB_GazePosX_left.IsEnabled = true;
            TB_GazePosY_left.IsEnabled = true;
            TB_GazePosZ_left.IsEnabled = true;
            TB_GazePosX_right.IsEnabled = true;
            TB_GazePosY_right.IsEnabled = true;
            TB_GazePosZ_right.IsEnabled = true;

            L_Gaze3DX.IsEnabled = false;
            TB_Gaze3DX.IsEnabled = false;
            L_Gaze3DY.IsEnabled = false;
            TB_Gaze3DY.IsEnabled = false;
            L_Gaze3DZ.IsEnabled = false;
            TB_Gaze3DZ.IsEnabled = false;
        }

        // radiobutton fixations only clicked
        private void FixationsClicked(object sender, RoutedEventArgs e)
        {
            gaze = false;
            fixations = true;
            TI_Gaze.Visibility = Visibility.Visible;
            TI_Fixations.Visibility = Visibility.Visible;
            TI_Gaze.IsSelected = true;

            L_GazePosX.IsEnabled = false;
            L_GazePosY.IsEnabled = false;
            L_GazePosZ.IsEnabled = false;
            TB_GazePosX_left.IsEnabled = false;
            TB_GazePosY_left.IsEnabled = false;
            TB_GazePosZ_left.IsEnabled = false;
            TB_GazePosX_right.IsEnabled = false;
            TB_GazePosY_right.IsEnabled = false;
            TB_GazePosZ_right.IsEnabled = false;

            L_Gaze3DX.IsEnabled = true;
            TB_Gaze3DX.IsEnabled = true;
            L_Gaze3DY.IsEnabled = true;
            TB_Gaze3DY.IsEnabled = true;
            L_Gaze3DZ.IsEnabled = true;
            TB_Gaze3DZ.IsEnabled = true;
        }

        // radiobutton delimiter semicolon clicked
        private void DelimiterSemicolonClicked(object sender, RoutedEventArgs e)
        {
            input_delimiter_ascii = 59;
        }

        // radiobutton delimiter tabulator clicked
        private void DelimiterTabulatorClicked(object sender, RoutedEventArgs e)
        {
            input_delimiter_ascii = 9;
        }

        // checkbox identifier checked
        private void IdentifierChecked(object sender, RoutedEventArgs e)
        {
            TB_Identifier.Opacity = 1;
            TB_Identifier.IsEnabled = true;

            if (CB_SpecialFixationCaseCrossing != null)
            {
                CB_SpecialFixationCaseCrossing.Opacity = 1;
                CB_SpecialFixationCaseCrossing.IsEnabled = true;
            }
        }
        // checkbox identifier unchecked
        private void IdentifierUnChecked(object sender, RoutedEventArgs e)
        {
            TB_Identifier.Opacity = 0.5;
            TB_Identifier.IsEnabled = false;

            if (CB_SpecialFixationCaseCrossing != null)
            {
                CB_SpecialFixationCaseCrossing.IsChecked = false;
                CB_SpecialFixationCaseCrossing.Opacity = 0.5;
                CB_SpecialFixationCaseCrossing.IsEnabled = false;
            }
        }

        // checkbox participant checked
        private void ParticipantChecked(object sender, RoutedEventArgs e)
        {
            TB_Participant.IsEnabled = true;
            TB_Participant.Opacity = 1;
        }
        // checkbox participant unchecked
        private void ParticipantUnChecked(object sender, RoutedEventArgs e)
        {
            TB_Participant.Opacity = 0.5;
            TB_Participant.IsEnabled = false;
        }

        // checkbox validity checked
        private void ValidityChecked(object sender, RoutedEventArgs e)
        {
            TB_Validity.Opacity = 1;
            TB_Validity.IsEnabled = true;
        }
        // checkbox validity unchecked
        private void ValidityUnChecked(object sender, RoutedEventArgs e)
        {
            TB_Validity.Opacity = 0.5;
            TB_Validity.IsEnabled = false;
        }

        // read user input from UI
        private bool readUserInput()
        {
            statusupdate("read input parameters");

            try
            {
                // write settings
                foreach (String filepath in paths)
                {
                    settings.Paths.Add(filepath);
                }

                settings.Gaze = gaze;
                settings.Fixations = fixations;
                settings.Header = CB_Header.IsChecked.Value;
                settings.Delimiter_Semicolon = RB_DelimiterSemicolon.IsChecked.Value;
                settings.Delimiter_Tabulator = RB_DelimiterTabulator.IsChecked.Value;
                settings.NumberOfColums = int.Parse(TB_NumberOfColumns.Text);
                settings.IncludeIdentifier = CB_Identifier.IsChecked.Value;
                settings.Column_Identifier = int.Parse(TB_Identifier.Text);
                settings.IncludeParticipant = CB_Participant.IsChecked.Value;
                settings.Column_Participant = int.Parse(TB_Participant.Text);
                settings.IncludeValidity = CB_Validity.IsChecked.Value;
                settings.Column_Validity = int.Parse(TB_Validity.Text);
                settings.Column_EyePosX_left = int.Parse(TB_EyePosX_left.Text);
                settings.Column_EyePosY_left = int.Parse(TB_EyePosY_left.Text);
                settings.Column_EyePosZ_left = int.Parse(TB_EyePosZ_left.Text);
                settings.Column_EyePosX_right = int.Parse(TB_EyePosX_right.Text);
                settings.Column_EyePosY_right = int.Parse(TB_EyePosY_right.Text);
                settings.Column_EyePosZ_right = int.Parse(TB_EyePosZ_right.Text);
                settings.Column_GazePosX_left = int.Parse(TB_GazePosX_left.Text);
                settings.Column_GazePosY_left = int.Parse(TB_GazePosY_left.Text);
                settings.Column_GazePosZ_left = int.Parse(TB_GazePosZ_left.Text);
                settings.Column_GazePosX_right = int.Parse(TB_GazePosX_right.Text);
                settings.Column_GazePosY_right = int.Parse(TB_GazePosY_right.Text);
                settings.Column_GazePosZ_right = int.Parse(TB_GazePosZ_right.Text);
                settings.Column_Gaze3DX = int.Parse(TB_Gaze3DX.Text);
                settings.Column_Gaze3DY = int.Parse(TB_Gaze3DY.Text);
                settings.Column_Gaze3DZ = int.Parse(TB_Gaze3DZ.Text);

                settings.Samples = int.Parse(TB_Samples.Text);
                settings.Frequency = int.Parse(TB_Frequency.Text);
                settings.ThresholdDuration = int.Parse(L_DurationValue.Content.ToString());
                settings.ThresholdDispersion = double.Parse(TB_Dispersion.Text, nfi);
                settings.MaxNumberOutliers = int.Parse(TB_Outliers.Text);
                settings.MaxNumberInvalids = int.Parse(TB_Invalids.Text);
                settings.IncludeCaseCrossing = CB_SpecialFixationCaseCrossing.IsChecked.Value;
                settings.IncludeFirstOngoingFixation = CB_SpecialFixationCaseStartFile.IsChecked.Value;

                settings.Output_CSV = RB_OutputCSV.IsChecked.Value;
                settings.Output_TXT = RB_OutputTXT.IsChecked.Value;
                settings.OpenOutputFile = CB_OpenOutputFile.IsChecked.Value;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        // check user input
        private bool checkUserInput()
        {

            bool check = true;
            String error = "";

            if (settings.NumberOfColums < settings.Column_Identifier && settings.IncludeIdentifier)
            {
                error = error + ("\n - Column for Case Identifier exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_Participant && settings.IncludeParticipant)
            {
                error = error + ("\n - Column for Participant exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_Validity && settings.IncludeValidity)
            {
                error = error + ("\n - Column for Validity exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosX_left)
            {
                error = error + ("\n - Column for left EyePosition X exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosY_left)
            {
                error = error + ("\n - Column for left EyePosition Y exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosZ_left)
            {
                error = error + ("\n - Column for left EyePosition Z exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosX_right)
            {
                error = error + ("\n - Column for right EyePosition X exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosY_right)
            {
                error = error + ("\n - Column for right EyePosition Y exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_EyePosZ_right)
            {
                error = error + ("\n - Column for right EyePosition Z exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosX_left && settings.Gaze)
            {
                error = error + ("\n - Column for left 2D GazePosition X exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosY_left && settings.Gaze)
            {
                error = error + ("\n - Column for left 2D GazePosition Y exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosZ_left && settings.Gaze)
            {
                error = error + ("\n - Column for left 2D GazePosition Z exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosX_right && settings.Gaze)
            {
                error = error + ("\n - Column for right 2D GazePosition X exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosY_right && settings.Gaze)
            {
                error = error + ("\n - Column for right 2D GazePosition Y exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_GazePosZ_right && settings.Gaze)
            {
                error = error + ("\n - Column for right 2D GazePosition Z exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_Gaze3DX && !settings.Gaze)
            {
                error = error + ("\n - Column for 3D GazePosition X exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_Gaze3DY && !settings.Gaze)
            {
                error = error + ("\n - Column for 3D GazePosition Y exceeds Number of Columns");
                check = false;
            }
            if (settings.NumberOfColums < settings.Column_Gaze3DZ && !settings.Gaze)
            {
                error = error + ("\n - Column for 3D GazePosition Z exceeds Number of Columns");
                check = false;
            }

            if (!check)
            {
                MessageBoxResult openfile = MessageBox.Show(("Please correct your input. Errors found: " + error),
                     "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return check;
        }

        // check input file access and structure
        private bool checkInputFiles()
        {
            bool check = true;

            if (settings.Paths.Count != 0)
            {

                foreach (String filepath in paths)
                {
                    try
                    {
                        using (StreamReader countReader = new StreamReader(File.OpenRead(filepath)))
                        {

                        }
                    }
                    catch
                    {
                        MessageBoxResult openfile = MessageBox.Show("Could not open input file. Please close the input file first.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        check = false;
                    }
                    if (check)
                    {
                        int columncount = CountColums(filepath);
                        if (settings.NumberOfColums != columncount)
                        {
                            MessageBoxResult columncheck = MessageBox.Show("The number of columns does not match the number of columns in the input file at path: " + filepath.ToString() +
                                ". The input file has " + columncount + " colums. Do you still want to continue?",
                                "Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            switch (columncheck)
                            {
                                case MessageBoxResult.Yes:
                                    check = true;
                                    break;
                                case MessageBoxResult.No:
                                    check = false;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                MessageBoxResult openfile = MessageBox.Show("No input file selected.",
                     "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                check = false;
            }

            return check;
        }

        // read input files
        private void readInputFiles()
        {
            statusupdate("read input files");

            gazefiles.Clear();
            gazefiles3d.Clear();

            if (gaze)
            {
                foreach (String path in settings.Paths)
                {
                    GazeFile gazefile = InputFileController.newGazeFile(
                        path,
                        input_delimiter_ascii,
                        settings.IncludeIdentifier,
                        settings.Column_Identifier,
                        settings.IncludeParticipant,
                        settings.Column_Participant,
                        settings.IncludeValidity,
                        settings.Column_Validity,
                        settings.Header,
                        settings.Column_EyePosX_left,
                        settings.Column_EyePosY_left,
                        settings.Column_EyePosZ_left,
                        settings.Column_EyePosX_right,
                        settings.Column_EyePosY_right,
                        settings.Column_EyePosZ_right,
                        settings.Column_GazePosX_left,
                        settings.Column_GazePosY_left,
                        settings.Column_GazePosZ_left,
                        settings.Column_GazePosX_right,
                        settings.Column_GazePosY_right,
                        settings.Column_GazePosZ_right);

                    gazefiles.Add(gazefile);
                    progressBarMaxValue = progressBarMaxValue + gazefile.list_GazeData.Count();
                }
            }
            else if (!gaze)
            {
                foreach (String path in settings.Paths)
                {
                    GazeFile3D gazefile3d = InputFileController.newGazeFile3D(
                        path,
                        input_delimiter_ascii,
                        settings.IncludeIdentifier,
                        settings.Column_Identifier,
                        settings.IncludeParticipant,
                        settings.Column_Participant,
                        settings.IncludeValidity,
                        settings.Column_Validity,
                        settings.Header,
                        settings.Column_EyePosX_left,
                        settings.Column_EyePosY_left,
                        settings.Column_EyePosZ_left,
                        settings.Column_EyePosX_right,
                        settings.Column_EyePosY_right,
                        settings.Column_EyePosZ_right,
                        settings.Column_Gaze3DX,
                        settings.Column_Gaze3DY,
                        settings.Column_Gaze3DZ);

                    gazefiles3d.Add(gazefile3d);
                    progressBarMaxValue = progressBarMaxValue + gazefile3d.list_GazeData3D.Count();
                }
            }
        }

        // get samples from settings
        private int getSamples()
        {
            return settings.Samples;
        }

        // calculate
        private void calculate()
        {
            if (gaze && fixations)
            {
                progressBarMaxValue = progressBarMaxValue * 2;
            }
            progressbarmaxupdate(progressBarMaxValue);

            if (gaze)
            {
                // calculate 3d gaze files
                statusupdate("calculating 3d gaze positions");

                foreach (GazeFile gazefile in gazefiles)
                {
                    GazeFile3D gazefile3d = new GazeFile3D();
                    gazefile3d.filename = gazefile.filename;

                    foreach (GazeData gazedata in gazefile.list_GazeData)
                    {
                        int gaze3dx; 
                        int gaze3dy; 
                        int gaze3dz; 

                        int result = CalculateGaze3DFromEyePos(
                             gazedata.getEyePosXLeft(),
                             gazedata.getEyePosYLeft(),
                             gazedata.getEyePosZLeft(),
                             gazedata.getGazePosXLeft(),
                             gazedata.getGazePosYLeft(),
                             gazedata.getGazePosZLeft(),
                             gazedata.getEyePosXRight(),
                             gazedata.getEyePosYRight(),
                             gazedata.getEyePosZRight(),
                             gazedata.getGazePosXRight(),
                             gazedata.getGazePosYRight(),
                             gazedata.getGazePosZRight(),
                             out gaze3dx,
                             out gaze3dy,
                             out gaze3dz); 

                        GazeData3D gazedata3d = new GazeData3D(gazedata, gaze3dx, gaze3dy, gaze3dz);
                        gazefile3d.addGazeData3D(gazedata3d);

                        progressBarCurrentValue++;
                        progressbarvalueupdate(progressBarCurrentValue);
                    }

                    gazefiles3d.Add(gazefile3d);
                }
            }
            if (fixations)
            {
                // calculate fixations
                statusupdate("calculating fixations");

                int iMinimumSamples = getSamples();
                Single rad = Convert.ToSingle(57.3);
                Single fAccuracyAngleRad = Convert.ToSingle(settings.ThresholdDispersion / rad);

                foreach (GazeFile3D gazefile3d in gazefiles3d)
                {
                    int resultcount = 0;
                    int filesize = gazefile3d.list_GazeData3D.Count();

                    int init = Init3DFixation(iMinimumSamples, settings.MaxNumberInvalids, settings.MaxNumberOutliers);
                    int id = 0;
                    FixationFile fixationfile = new FixationFile();
                    fixationfile.filename = gazefile3d.filename;
                    List<GazeData3D> bufferlist = new List<GazeData3D>();
                    GazeData3D outlierbufferelement = null;

                    // add iMinimum Samples to finish algorithm and return last fixation
                    for (int i = 1; i <= iMinimumSamples; i++)
                    {
                        GazeData gd = new GazeData(0,"", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        GazeData3D gd3d = new GazeData3D(gd, 0, 0, 0);
                        gazefile3d.addGazeData3D(gd3d);
                        bufferlist.Add(gd3d);
                    }

                    foreach (GazeData3D gazedata3d in gazefile3d.list_GazeData3D)
                    {
                        bufferlist.Add(gazedata3d);

                        int bGazeVectorFound = gazedata3d.getGazeData().getValidity();
                        Single fXLeftEye = gazedata3d.getGazeData().getEyePosXLeft();
                        Single fYLeftEye = gazedata3d.getGazeData().getEyePosYLeft();
                        Single fZLeftEye = gazedata3d.getGazeData().getEyePosZLeft();
                        Single fXRightEye = gazedata3d.getGazeData().getEyePosXRight();
                        Single fYRightEye = gazedata3d.getGazeData().getEyePosYRight();
                        Single fZRightEye = gazedata3d.getGazeData().getEyePosZRight();
                        Single fXGaze = gazedata3d.getGaze3DX();
                        Single fYGaze = gazedata3d.getGaze3DY();
                        Single fZGaze = gazedata3d.getGaze3DZ();

                        int pbGazepointFoundDelayed;
                        Single pfXGazeDelayed;
                        Single pfYGazeDelayed;
                        Single pfZGazeDelayed;
                        Single pfXFixDelayed;
                        Single pfYFixDelayed;
                        Single pfZFixDelayed;
                        Single pfXEllipsoidRDelayed;
                        Single pfYEllipsoidRDelayed;
                        Single pfZEllipsoidRDelayed;
                        Single pfEllipsoidYawDelayed;
                        Single pfEllipsoidPitchDelayed;
                        int piSacDurationDelayed;
                        int piFixDurationDelayed;

                        int result = Calculate3DFixation(
                                        bGazeVectorFound,
                                        fXLeftEye,
                                        fYLeftEye,
                                        fZLeftEye,
                                        fXRightEye,
                                        fYRightEye,
                                        fZRightEye,
                                        fXGaze,
                                        fYGaze,
                                        fZGaze,
                                        fAccuracyAngleRad,
                                        iMinimumSamples,
                                        out pbGazepointFoundDelayed,
                                        out pfXGazeDelayed,
                                        out pfYGazeDelayed,
                                        out pfZGazeDelayed,
                                        out pfXFixDelayed,
                                        out pfYFixDelayed,
                                        out pfZFixDelayed,
                                        out pfXEllipsoidRDelayed,
                                        out pfYEllipsoidRDelayed,
                                        out pfZEllipsoidRDelayed,
                                        out pfEllipsoidYawDelayed,
                                        out pfEllipsoidPitchDelayed,
                                        out piSacDurationDelayed,
                                        out piFixDurationDelayed);

                        //Console.WriteLine("Result " + result.ToString());

                        if (result == 0)
                        {
                            outlierbufferelement = bufferlist.First();
                            bufferlist.RemoveAt(0);
                        }
                        
                        if (result == 1)
                        {
                            resultcount++;
                            if (resultcount != piFixDurationDelayed)
                            {
                                //Console.WriteLine("Outlier found");
                                bufferlist.Insert(0, outlierbufferelement);
                                resultcount++;
                            }
                        }

                        if (result == 2)
                        {
                            resultcount++;
                            id++;

                            FixationData fixationdata = new FixationData(
                                            id,
                                            bufferlist.First().getGazeData().getIdentifier(),
                                            bufferlist.ElementAt(piFixDurationDelayed - 1).getGazeData().getIdentifier(),
                                            bufferlist.First().getGazeData().getParticipant(),
                                            pfXFixDelayed,
                                            pfYFixDelayed,
                                            pfZFixDelayed,
                                            pfXEllipsoidRDelayed,
                                            pfYEllipsoidRDelayed,
                                            pfZEllipsoidRDelayed,
                                            pfEllipsoidYawDelayed,
                                            pfEllipsoidPitchDelayed,
                                            piSacDurationDelayed,
                                            (piSacDurationDelayed*(1000.0/settings.Frequency)),
                                            piFixDurationDelayed,
                                            (piFixDurationDelayed *(1000.0/settings.Frequency)),
                                            bufferlist.First().getGazeData().getID(),
                                            bufferlist.ElementAt(piFixDurationDelayed - 1).getGazeData().getID());


                            if ((fixationdata.getStartSampleID() == 1) && (!settings.IncludeFirstOngoingFixation))
                            {
                                //Console.WriteLine("first fixation starts with first sample");
                                // don't include first fixation
                            }
                            else if (((settings.IncludeIdentifier) && (!settings.IncludeCaseCrossing) && (fixationdata.getStartIdentifier() != fixationdata.getEndIdentifer())) || (fixationdata.getEndSampleID() > filesize))
                            {
                                //Console.WriteLine("case crossing. don't include fixation");
                                // don't include fixation crossing cases
                            }
                            else
                            {
                                //Console.WriteLine("add fixation");
                                fixationfile.addFixationData(fixationdata);
                            }

                            bufferlist.RemoveRange(0, piFixDurationDelayed);
                            resultcount = 0;
                        }

                        progressBarCurrentValue++;
                        progressbarvalueupdate(progressBarCurrentValue);
                    }
                    fixationfiles.Add(fixationfile);
                    
                    // remove iMinimum samples from end of gazefile3d
                    for (int i = 1; i <= iMinimumSamples; i++)
                    {
                        gazefile3d.list_GazeData3D.RemoveAt(gazefile3d.list_GazeData3D.Count()-1);
                    }
                }
            }
        }

        // output
        private void output()
        {
            statusupdate("writing output files");

            String output_delimiter = " ";
            String extension = "";
            if (settings.Output_CSV)
            {
                output_delimiter = ";";
                extension = ".csv";
            }
            else if (settings.Output_TXT)
            {
                output_delimiter = "\t";
                extension = ".txt";
            }

            // write 3d gazefile to output file
            if (gaze)
            {
                foreach (GazeFile3D gazefile3d in gazefiles3d)
                {
                    using (StreamWriter writer = new StreamWriter("output_Gaze3D_" + gazefile3d.filename + extension))
                    {
                        String header = "";
                        header = header + "Gaze3D_SampleID" + output_delimiter;
                        if (settings.IncludeIdentifier)
                        {
                            header = (header + "CaseIdentifier" + output_delimiter);
                        }
                        if (settings.IncludeParticipant)
                        {
                            header = (header + "Participant" + output_delimiter);
                        }
                        if (settings.IncludeValidity)
                        {
                            header = (header + "Validity" + output_delimiter);
                        }
                        header = header + "EyePosX_left" + output_delimiter 
                            + "EyePosY_left" + output_delimiter 
                            + "EyePosZ_left" + output_delimiter 
                            + "EyePosX_right" + output_delimiter 
                            + "EyePosY_right" + output_delimiter 
                            + "EyePosZ_right" + output_delimiter;
                        header = header + "Gaze3DX" + output_delimiter + "Gaze3DY" + output_delimiter + "Gaze3DZ";

                        writer.WriteLine(header);

                        foreach (GazeData3D gazedata3d in gazefile3d.list_GazeData3D)
                        {
                            writer.WriteLine(gazedata3d.getGazeData3DString(settings.IncludeIdentifier, settings.IncludeParticipant, settings.IncludeValidity, output_delimiter));
                        }
                    }

                    if (settings.OpenOutputFile)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "output_Gaze3D_" + gazefile3d.filename + extension);
                        }
                        catch
                        {
                            Console.WriteLine("Could not open output file " + "output_Gaze3D_" + gazefile3d.filename + extension);
                        }
                    }
                }
            }

            // write 3d fixations to output file
            if (fixations)
            {
                foreach (FixationFile fixationfile in fixationfiles)
                {
                    using (StreamWriter writer = new StreamWriter("output_Fixations3D_" + fixationfile.filename + extension))
                    {
                        String header = "";
                        header = header + "Fixation3D_SampleID" + output_delimiter;
                        if (settings.IncludeIdentifier)
                        {
                            header = (header + "CaseIdentifier" + output_delimiter);
                        }
                        if (settings.IncludeParticipant)
                        {
                            header = (header + "Participant" + output_delimiter);
                        }

                        header = header + "FixationPosX" + output_delimiter
                                        + "FixationPosY" + output_delimiter
                                        + "FixationPosZ" + output_delimiter
                                        + "EllipsoidRadiusX" + output_delimiter
                                        + "EllipsoidRadiusY" + output_delimiter
                                        + "EllipsoidRadiusZ" + output_delimiter
                                        + "EllipsoidYaw" + output_delimiter
                                        + "EllipsoidPitch" + output_delimiter
                                        + "SaccadeDuration_Samples" + output_delimiter
                                        + "SaccadeDuration_MS" + output_delimiter
                                        + "FixationDuration_Samples" + output_delimiter
                                        + "FixationDuration_MS" + output_delimiter
                                        + "Start_Gaze3D_SampleID" + output_delimiter
                                        + "End_Gaze3D_SampleID";

                        writer.WriteLine(header);

                        foreach (FixationData fixationdata in fixationfile.list_FixationData)
                        {
                            writer.WriteLine(fixationdata.getFixationDataString(settings.IncludeIdentifier, settings.IncludeParticipant, output_delimiter));
                        }
                    }

                    // open output files
                    if (settings.OpenOutputFile)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "output_Fixations3D_" + fixationfile.filename + extension);
                        }
                        catch
                        {
                            Console.WriteLine("Could not open output file " + "output_Fixations3D_" + fixationfile.filename + extension);
                        }
                    }
                }
            }
        }

        // load input file button clicked
        private void B_InputFile_Click(object sender, RoutedEventArgs e)
        {
            paths.Clear();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "inputfile";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Text (*.txt;*.csv)|*.txt;*.csv";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                int filecount = dlg.FileNames.Count();
                if (filecount > 1)
                {
                    TB_InputFile.Text = dlg.FileNames.First() + " ... and " + (filecount - 1).ToString() + " more file(s)";
                }
                else
                {
                    TB_InputFile.Text = dlg.FileNames.First();
                }
                foreach (var file in dlg.FileNames)
                {
                    Console.WriteLine(file);
                    paths.Add(file);
                }
            }
        }

        // calculate button clicked
        private void B_Calculate_Click(object sender, RoutedEventArgs e)
        {
            progressBarMaxValue = 0;
            progressBarCurrentValue = 0;
            ProgressBar_Status.Visibility = Visibility.Visible;
            if (readUserInput())
            {
                if (checkUserInput())
                {
                    if (checkInputFiles())
                    {
                        runlock();
                        readInputFiles();
                        try
                        {
                            calculate();
                        }
                        catch
                        {
                            MessageBoxResult openfile = MessageBox.Show("An unknown error occured. Please check your input file(s) and make sure that both Dynamic Link Libraries (Gaze.dll and Fixation3D.dll) are in the same directory as the application.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        output();
                        statusupdate("done");
                        fixationfiles.Clear();
                        gazefiles3d.Clear();
                        gazefiles.Clear();
                        settings.Paths.Clear();
                        donelock();
                    }
                }
            }
            else
            {
                MessageBoxResult openfile = MessageBox.Show("An unknown error occured. Could not read user input. Please check your input.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // done button clicked
        private void B_Done_Click(object sender, RoutedEventArgs e)
        {
            initStatus();
            initProgressBar(1);
        }

        // cancel button clicked
        private void B_Cancel_Click(object sender, RoutedEventArgs e)
        {
            initStatus();
            initProgressBar(1);
        }

        // lock UI while calculating
        private void runlock()
        {
            B_Calculate.IsEnabled = false;
            rectangle3.Visibility = Visibility.Visible;
            B_Cancel.Visibility = Visibility.Visible;
        }

        // lock UI when finished
        private void donelock()
        {
            if (B_Cancel.Dispatcher.CheckAccess())
            {
                B_Calculate.IsEnabled = false;
                B_Calculate.Visibility = Visibility.Visible;
                B_Cancel.Visibility = Visibility.Hidden;
            }
            else
            {
                finishedDelegate fa = new finishedDelegate(donelock);
                B_Cancel.Dispatcher.Invoke(fa, new object[] { });
            }

            if (B_Done.Dispatcher.CheckAccess())
            {
                R_Done.Visibility = Visibility.Visible;
                L_Done.Visibility = Visibility.Visible;
                B_Done.Visibility = Visibility.Visible;
            }
            else
            {
                finishedDelegate ff = new finishedDelegate(donelock);
                B_Done.Dispatcher.Invoke(ff, new object[] { });
            }
        }

        // lock UI for abort
        private void abortlock()
        {
            if (B_Cancel.Dispatcher.CheckAccess())
            {
                B_Cancel.Visibility = Visibility.Hidden;
            }
            else
            {
                finishedDelegate fa = new finishedDelegate(donelock);
                B_Cancel.Dispatcher.Invoke(fa, new object[] { });
            }

            if (B_Done.Dispatcher.CheckAccess())
            {
                R_Done.Visibility = Visibility.Visible;
                L_Done.Visibility = Visibility.Visible;
                B_Done.Visibility = Visibility.Visible;
            }
            else
            {
                finishedDelegate ff = new finishedDelegate(donelock);
                B_Done.Dispatcher.Invoke(ff, new object[] { });
            }
        }

        // initialize status
        private void initStatus()
        {
            B_Calculate.Visibility = Visibility.Visible;
            B_Calculate.IsEnabled = true;
            rectangle3.Visibility = Visibility.Hidden;
            R_Done.Visibility = Visibility.Hidden;
            L_Done.Visibility = Visibility.Hidden;
            B_Done.Visibility = Visibility.Hidden;
            B_Cancel.Visibility = Visibility.Hidden;
            L_StatusOut.Content = "ready";

            ProgressBar_Status.Visibility = Visibility.Hidden;
        }

        // initialize progress bar
        private void initProgressBar(int maxbar)
        {
            ProgressBar_Status.Value = 0;
            ProgressBar_Status.Maximum = maxbar;
        }

        // status update
        private void statusupdate(String statusout)
        {
            if (L_StatusOut.Dispatcher.CheckAccess())
            {
                L_StatusOut.Content = statusout;
            }
            else
            {
                StatusDelegate s = new StatusDelegate(statusupdate);
                L_StatusOut.Dispatcher.Invoke(s, new object[] { statusout });
            }
        }

        // progress bar value update
        private void progressbarvalueupdate(int v)
        {
            if (ProgressBar_Status.Dispatcher.CheckAccess())
            {
                ProgressBar_Status.Value = v;
            }
            else
            {
                ProgressBarValueDelegate pbv = new ProgressBarValueDelegate(progressbarvalueupdate);
                ProgressBar_Status.Dispatcher.Invoke(pbv, new object[] { v });
            }
        }

        // progress bar maximum update
        private void progressbarmaxupdate(int max)
        {
            if (ProgressBar_Status.Dispatcher.CheckAccess())
            {
                ProgressBar_Status.Value = 0;
                ProgressBar_Status.Maximum = max;
            }
            else
            {
                ProgressBarMaxDelegate pbm = new ProgressBarMaxDelegate(progressbarmaxupdate);
                ProgressBar_Status.Dispatcher.Invoke(pbm, new object[] { max });
            }
        }

        // text samples changed
        private void TB_SamplesChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            String textin = textbox.Text.ToString();
            char[] buffer = new char[textin.Length];
            int idx = 0;

            foreach (char c in textin)
            {
                if ((c >= '0' && c <= '9' && idx > 0) || (c >= '1' && c <= '9' && idx == 0))
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            if ((textin.Length == 0) || (textin.Length == 1 && (int.Parse(textbox.Text) < 3)))
            {
                TT_NumberofSamples.IsOpen = true;
            }
            else
            {
                TT_NumberofSamples.IsOpen = false;
            }

            textin = new string(buffer, 0, idx);
            textbox.Text = textin;
            textbox.CaretIndex = idx;

            if (textin != "0" && textin != "")
            {
                updateDurationValue();
            }
            else
            {
                L_DurationValue.Content = "";
            }
        }

        // textbox samples lost focus 
        private void TB_Samples_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "" || int.Parse(textbox.Text) < 3)
            {
                textbox.Text = "3";
            }
            TT_NumberofSamples.IsOpen = false;
        }

        // text frequency changed
        private void TB_FrequencyChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            String textin = textbox.Text.ToString();
            char[] buffer = new char[textin.Length];
            int idx = 0;

            foreach (char c in textin)
            {
                if ((c >= '0' && c <= '9' && idx > 0) || (c >= '1' && c <= '9' && idx == 0))
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            textin = new string(buffer, 0, idx);
            textbox.Text = textin;
            textbox.CaretIndex = idx;

            if (textin != "0" && textin != "")
            {
                updateDurationValue();
            }
            else
            {
                L_DurationValue.Content = "";
            }
        }

        // textbox frequency lost focus
        private void TB_Frequency_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "" || int.Parse(textbox.Text) <= 1)
            {
                textbox.Text = "1";
            }
        }

        // duration threshold label update
        private void updateDurationValue()
        {
            if (TB_Frequency != null && TB_Samples != null && L_DurationValue != null)
            {
                double sec = 1;
                double ms = (sec / int.Parse(TB_Frequency.Text)) * 1000;
                L_DurationValue.Content = (ms * int.Parse(TB_Samples.Text)).ToString();
            }
            else
            {
                L_DurationValue.Content = "0";
            }
        }

        // text dispersion changed
        private void TB_DispersionChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            String textin = textbox.Text.ToString();
            char[] buffer = new char[textin.Length];
            int idx = 0;
            bool comma = false;

            foreach (char c in textin)
            {
                if (c >= '0' && c <= '9')
                {
                    buffer[idx] = c;
                    idx++;
                }
                else if (c == ',' && comma == false)
                {
                    buffer[idx] = c;
                    idx++;
                    comma = true;
                }
            }

            textin = new string(buffer, 0, idx);
            textbox.Text = textin;
            textbox.CaretIndex = idx;
        }

        // textbox dispersion lost focus 
        private void TB_Dispersion_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "")
            {
                textbox.Text = "0";
            }
        }

        // textbox lost focus 
        private void TB_Outliers_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "")
            {
                textbox.Text = "0";
            }
        }

        // textbox lost focus 
        private void TB_Invalids_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "")
            {
                textbox.Text = "0";
            }
        }

        // text changed
        private void TB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // numbers only and cursor at the end
            TextBox textbox = sender as TextBox;
            String textin = textbox.Text.ToString();
            char[] buffer = new char[textin.Length];
            int idx = 0;

            foreach (char c in textin)
            {
                if (c >= '0' && c <= '9')
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            textin = new string(buffer, 0, idx);
            textbox.Text = textin;
            textbox.CaretIndex = idx;
        }

        // textbox lost focus 
        private void TB_Column_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "")
            {
                textbox.Text = "1";
            }
        }

        // count columns in input files
        private int CountColums(string fileToCount)
        {
            int counter = 0;
            int colums = 0;
            string l = String.Empty;
            char s = Convert.ToChar(input_delimiter_ascii);
            string[] w;

            using (StreamReader countReader = new StreamReader(File.OpenRead(fileToCount)))
            {
                while ((l = countReader.ReadLine()) != null)
                {
                    counter++;
                    w = l.Split(s);
                    colums = w.Count();
                }
            }

            return colums;
        }

        //--XML----------------------------------------------------------------
        // serializer
        private void SerializeToXML(string file, Settings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter textWriter = new StreamWriter(file);
            serializer.Serialize(textWriter, settings);
            textWriter.Close();
            Console.WriteLine("Serialize to XML");
        }

        // deserializer
        private Settings DeserializeFromXML(string file)
        {
            Settings settings = new Settings();   
            if (File.Exists(file) && new FileInfo(file).Length > 0)
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
                TextReader textReader = new StreamReader(file);
                settings = (Settings)deserializer.Deserialize(textReader);
                textReader.Close();
                Console.WriteLine("Deserialize from XML");
            }
            else
            {
                Console.WriteLine("XML File Error");
                launchConfigXML();
                DeserializeFromXML(file);
            }
            return settings;
        }

        // XML path
        internal string getXMLPath()
        {
            string path;
            path = AppDomain.CurrentDomain.BaseDirectory + "settings.xml";
            Console.WriteLine(path);
            return path;
        }

        // configure XML
        private void launchConfigXML()
        {
            XmlTextWriter textwriter = new XmlTextWriter(getXMLPath(), null);
            textwriter.WriteStartElement("Settings");
            textwriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            textwriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            textwriter.WriteEndElement();
            textwriter.Flush();
            textwriter.Close();
        }

        //--Window-------------------------------------------------------------
        // init
        private void Window_Initialized(object sender, EventArgs e)
        {
            settings = DeserializeFromXML(getXMLPath());
            settings.Paths.Clear();
        }

        // closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            readUserInput();
            settings.Paths.Clear();
            SerializeToXML(getXMLPath(), settings);
        }
    }
}