

	Fixation3D.dll - dispersion based detection of 3D fixations with ellipsoid accanptance space
        ============================================================================================

        02.06.2016
        Sascha Weber (sascha.weber@tu-dresden.de)


	Description:	  This library encapsulates the functions of the dispersion based detection algorithm of 3D fixations with ellipsoid tolerance area. For 
			  detailled specifications of the parameters and functionality of the algorithm see the comments in the source code:

			  Gaze3DFix\src\Fixation3D.dll\units\FixFuncClass3dCalcByEllipsoid.pas


        Implementation:   The spatial fixation detection logic with the ellipsoid tolerance area was implemented into the LC Technologies, Inc.
                          fixfunc in cooperation with Dixon Cleveland and extends the estimation of 2D fixations to the third dimension.
 
                          original 2D fixation detection             File Name:       FIXFUNC.C
                                                                     Program Name:    Eye Fixation Analysis Functions
                                                                     Company:         LC Technologies, Inc.
                                                                                      10363 Democracy Lane
                                                                                      Fairfax, VA 22030
                                                                                      (703) 385-8800
                                                                     Date Created:    10/20/89
                                                                                      04/12/95 modified: turned into collection of functions


	


	
	Usage:		  1. 	Initializing of the 3D fixation detection.
	
				
				function Init3DFixation (iMinimumFixSamplesInit, iMaxMissedSamplesInit, iMaxOutSamplesInit: Integer): Integer; stdcall; 
	 

  				// *  This function clears the previous, present and new fixation hypotheses,
  				// *  and it initializes DetectFixation()'s internal ring buffers of prior
				// *  gazepoint data.  InitFixation() should be called prior to a sequence
  				// *  of calls to DetectFixation().

  				// *  iMinimumFixSamples = minimum number of gaze samples that can be considered a fixation
 				// *  Note: if the input value is less than 3, the function sets it to 3


				// *  iMaxMissedSamples  = the maximum allowable number of consecutive samples that may go untracked within a 
				// *                       fixation. default = 3

				// *  iMaxOutSamples     = the maximum allowable number of consecutive samples that may go outside the fixation 
				// *                       acceptance ellipsoid.default = 1

				// *  Returns: 0 if the initialization was not successful
				// *           1 if the initialization was successful

				// *  This function clears the previous, present and new fixation hypotheses, and it initializes DetectFixation()'s
				// *  internal ring buffers of prior gazepoint data.  InitFixation() should be called prior to a sequence of calls
				// *  to DetectFixation().





       			   2.	Calulation of 3D fixations by 3D gazepoints.
 
				function Calculate3DFixation(


                                          		// *  INPUT PARAMETERs:

                                   		       	bValidSample: Integer; 	      	// *  flag indicating whether or not the image processing algorithm
                                                      		                        // *    detected booth eyes and computed a valid 3d gazepoint (TRUE/FALSE)

                                    		      	fXLeftEye: Single;              // *  3D coordinates of the left eye
                                          		fYLeftEye: Single;
                                          		fZLeftEye: Single;

                                          		fXRightEye: Single;             // *  3D coordinates of the right eye
                                          		fYRightEye: Single;
                                          		fZRightEye: Single;

                                          		fXGaze: Single; 		// *  3D coordinates of the current gazepoint
                                          		fYGaze: Single;
                                         		fZGaze: Single;                                                                                                                                                                                                  
					  		fAccuracyAngleRad: Single;      // *  threshold for the ellipsoid dimensions related to the fixation distance

                                          		iMinimumFixSamples: Integer; 	// *  minimum number of gaze samples that can be considered a fixation
                                                                              		// *    Note: if the input value is less than 3, the function sets it to 3
	


                                         		// * OUTPUT PARAMETERS: delayed gazepoint data with 3D fixation annotations:

                          		 	out	pbValidSampleDelayed: Integer;  // *  bValidSample, iMinimumFixSamples ago

                           			out     pfXGazeDelayed,  		// *  3D coordinates of gazepoint, iMinimumFixSamples ago
                                          		pfYGazeDelayed,
                                          		pfZGazeDelayed: Single;

                           			out     pfXFixDelayed, 		      	// *  3D coordinates of fixation point, iMinimumFixSamples ago
                                          		pfYFixDelayed,
                                          		pfZFixDelayed: Single;

                          			out     pfXEllipsoidRDelayed,           // *  3D coordinaties and orientation of the ellipsoid, iMinimumFixSamples ago
                                          		pfYEllipsoidRDelayed,
                                          		pfZEllipsoidRDelayed: Single;

		                           	out     pfEllipsoidYawDelayed,          	// *    yaw angle
                		                        pfEllipsoidPitchDelayed: Single;    	// *    pitch angle

		                           	out     piSacDurationDelayed, 	            	// *  duration of the saccade preceeding the preset fixation (samples)
                		                        piFixDurationDelayed: Integer): Integer; stdcall; 	// *  duration of the present fixation (samples)


				// *  RETURN VALUES - Eye Motion State:
				// *
				// *    MOVING               0   The eye was in motion, iMinimumFixSamples ago.
				// *    FIXATING             1   The eye was fixating, iMinimumFixSamples ago.
				// *    FIXATION_COMPLETED   2   A completed fixation has just been detected, iMinimumFixSamples ago. With respect to the sample that reports
				// *                               FIXATION_COMPLETED, the fixation started (iMinimumFixSamples + *piSaccadeDurationDelayed) ago and ended
				// *                               iMinimumFixSamples ago.
				// *
				// *  SUMMARY
				// *
				// *    This function converts a series of uniformly-sampled (raw) gaze points into a series of variable-duration saccades and fixations.
				// *    Fixation analysis may be performed in real time or after the fact.  To allow eye fixation analysis during real-time eyegaze data collection,
				// *    the function is designed to be called once per sample.
				// *
				// *    When the eye is in motion, ie during saccades, the function returns 0 (MOVING).
				// *
				// *    When the eye is still, ie during fixations, the function returns 1 (FIXATING).
				// *
				// *    Upon the detected completion of a fixation, the function returns 2 (FIXATION_COMPLETED) and produces:
				// *      a) the time duration of the saccade between the last and present eye fixation (eyegaze samples)
				// *      b) the time duration of the present, just completed fixation (eyegaze samples)
				// *      c) the average x, y and z coordinates of the eye fixation (in user defined units)
				// *
				// *    Note: Although this function is intended to work in "real time", there is a delay of iMinimumFixSamples in the filter which detects the
				// *    motion/fixation condition of the eye.
				// *
				// *
				// *  PRINCIPLE OF OPERATION
				// *
				// *    This function detects fixations by looking for sequences of gazepoint measurements that remain relatively constant. If a new gazepoint
				// *    lies within a ellipsoid region around the running average of an on-going fixation, the fixation is extended to include the new gazepoint.
				// *    The dimensions of the acceptance ellipsoid is user specified by setting the value of the function argument fAccuracyAngleRad. This angle
				// *    defines the horizontal and vertical radius of the ellipsoid and the depth parameter is calculated related to the fixation distance (for
				// *    further information see the comments on the CalcEllipsoid function.) The Ellipsoid is not radially symmetric and has to be oriented (yaw
				// *    and pitch) to the user. Therefore a cyclopedian eye is defined between the users right and the left eye.
				// *
				// *    To accommodate noisy eyegaze measurements, a gazepoint that exceeds the deviation threshold is included in an on-going fixation if the
				// *    subsequent gazepoint returns to a position within the threshold.
				// *
				// *    If a gazepoint is not found, during a blink for example, a fixation is extended if
				// *      a) the next legitimate gazepoint measurement falls within the acceptance ellipsoid, and
				// *      b) there are less than iMinimumFixSamples of successive missed gazepoints.  Otherwise, the previous fixation is considered to end at
				// *         the last good gazepoint measurement.
				// *
				// *
				// *  UNITS OF MEASURE
				// *
				// *    The gaze position/direction may be expressed in any units (e.g. millimeters, pixels, or radians), but the filter threshold must be
				// *    expressed in the same units.
				// *
				// *  INITIALIZING THE FUNCTION
				// *
				// *    Prior to analyzing a sequence of gazepoint data, the InitFixation function should be called to clear any previous, present and new
				// *    fixations and to initialize the ring buffers of prior gazepoint data.
				// *
				// *  PROGRAM NOTES
				// *
				// *    For purposes of describing an ongoing sequence of fixations, fixations in this program are referred to as "previous", "present", and "new".
				// *    The present fixation is the one that is going on right now, or, if a new fixation has just started, the present fixation is the one that
				// *    just finished.  The previous fixation is the one immediatly preceeding the present one, and a new fixation is the one immediately following
				// *    the present one.  Once the present fixation is declared to be completed, the present fixation becomes the previous one, the new fixation
				// *    becomes the present one, and there is not yet a new fixation.
				// *




















