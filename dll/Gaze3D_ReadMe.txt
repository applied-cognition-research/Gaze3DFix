

	Gaze3D.dll - Calculation of 3D gaze point
        =========================================

        02.06.2016
        Sascha Weber (sascha.weber@tu-dresden.de)


	Description:	  This library encapsulates the functions of the 3D gazepoint calculation, defined as the midpoint of the shortest straight line between the two gaze
			  vectors from the eyes directed to the fixation point. For detailled specifications of the parameters and functionality of the algorithm see the comments 
			  in the source code:

			  Gaze3DFix\src\Gaze3D.dll\



	Usage:		  function CalculateGaze3DFromEyePos(   x_EyePos_Left,			// *  3D coordinates of the left eye
                          				        y_EyePos_Left,
                                				z_EyePos_Left,

                                     				x_EyePos_Right,			// *  3D coordinates of the right eye
                                     				y_EyePos_Right,
                                     				z_EyePos_Right,

                                     				x_GazePos_Left,			// *  3D coordinates of the left gaze position at the eyetracker measuring area
                                     				y_GazePos_Left,
                                     				z_GazePos_Left,

                                     				x_GazePos_Right,		// *  3D coordinates of the right gaze position at the eyetracker measuring area
                                     				y_GazePos_Right,
                                     				z_GazePos_Right: Int32;

                                 			out 	x_Gaze3D,			// *  3D coordinates of the spatial gaze position
                                     				y_Gaze3D,
                                     				z_Gaze3D: Int32) : Int32; stdcall;




			  function CalculateGaze3DFromGazeVec (	x_EyePos_Left,			// *  3D coordinates of the left eye
                                     				y_EyePos_Left,
                                     				z_EyePos_Left,

                                     				x_EyePos_Right,			// *  3D coordinates of the right eye
                                     				y_EyePos_Right,
                                     				z_EyePos_Right,

                                     				x_GazeVec_Left,			// *  gaze vector of the left eye
                                     				y_GazeVec_Left,
                                     				z_GazeVec_Left,

                                     				x_GazeVec_Right,		// *  gaze vector of the right eye
                                     				y_GazeVec_Right,
                                     				z_GazeVec_Right: Int32;

                                 			out 	x_Gaze3D,			// *  3D coordinates of the spatial gaze position
                                     				y_Gaze3D,
                                     				z_Gaze3D: Int32): Int32; stdcall;




			  function CalculateVergenceAngle     (	x_GazeVec_Left,
                                     				y_GazeVec_Left,
                                     				z_GazeVec_Left,

                                     				x_GazeVec_Right,
                                     				y_GazeVec_Right,
                                     				z_GazeVec_Right: Int32;

                                 			out 	VergenceAngle: Double): Int32; stdcall;

	


	




















