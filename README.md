# Gaze3DFix - Toolkit for the computation of 3D gaze points and 3D fixations

Nowadays the use of eye tracking to determine 2D gaze positions is common practice and several approaches for the detection of 2D fixations exist, but ready-to-use algorithms to determine eye movements in 3D are still missing. We present algorithms for the computation of 3D gaze points, defined as the midpoint of the shortest straight line between the two gaze vectors from the eyes directed to the fixation point. Furthermore, to detect 3D fixations, we apply a dispersion-based algorithm with an ellipsoid tolerance area.

The described algorithms are implemented in two dynamic link libraries 

	\dll\Gaze3D.dll
	\dll\Fixation3D.dll

source code is located at

	\src folder

The algorihms can be used online during the recording of eye movements or offline after data collection. 

For offline use we provide a simple graphical user interface that is designed for importing 2D binocular eye tracking data and calculating 3D gaze points and 3D fixations using the libraries.

	\Gaze3DFixGUI\Gaze3DFixGUI.exe


