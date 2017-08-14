//****************************************************************************************************************************************************
//****************************************************************************************************************************************************
//***
//***
//***       Fixation3D.dll - dispersion based fixation detection of 3D Fixations by an ellipsoid boundary volume
//***       =========================================================================================================================
//***
//***       Date:             23.03.2017
//***       Developer:        Sascha Weber (sascha.weber@tu-dresden.de)
//***
//***       Description:      Library for using the funtions of FixFuncClass3dCalcByEllipsoid.
//***
//***
//***
//***


library Fixation3D;

{ Important note about DLL memory management: ShareMem must be the
  first unit in your library's USES clause AND your project's (select
  Project-View Source) USES clause if your DLL exports any procedures or
  functions that pass strings as parameters or function results. This
  applies to all strings passed to and from your DLL--even those that
  are nested in records and classes. ShareMem is the interface unit to
  the BORLNDMM.DLL shared memory manager, which must be deployed along
  with your DLL. To avoid using BORLNDMM.DLL, pass string information
  using PChar or ShortString parameters. }




uses
  SysUtils,
  Classes,
  FixFuncClass3dCalcByEllipsoid in 'units\FixFuncClass3dCalcByEllipsoid.pas';

{$R *.res}

var
    FFixFuncClass3dCalcByEllipsoid: TFixFuncClass3dCalcByEllipsoid;


function Init3DFixation (iMinimumSamplesInit, iMaxMissedSamplesInit, iMaxOutSamplesInit: Integer): Integer; stdcall;
begin
  try
    Result:=0;
    FFixFuncClass3dCalcByEllipsoid:=TFixFuncClass3dCalcByEllipsoid.Create;
    FFixFuncClass3dCalcByEllipsoid.InitFixation(iMinimumSamplesInit,iMaxMissedSamplesInit, iMaxOutSamplesInit);
    if Assigned(FFixFuncClass3dCalcByEllipsoid)then
      Result:=1;
  except
    on e:Exception do
    raise Exception.CreateFmt('Calculate3DFixation: [%s]',[e.message]);
  end;
end;

function Calculate3DFixation( bValidSample: Integer;
                              fXLeftEye, fYLeftEye, fZLeftEye, fXRightEye, fYRightEye, fZRightEye, fXGaze, fYGaze, fZGaze: Single;
                              fAccuracyAngleRad: Single;
                              iMinimumSamples: Integer;

                        out   pbGazepointFoundDelayed: Integer;
                        out   pfXGazeDelayed, pfYGazeDelayed, pfZGazeDelayed: Single;
                        out   pfXFixDelayed, pfYFixDelayed, pfZFixDelayed: Single;
                        out   pfXEllipsoidRDelayed, pfYEllipsoidRDelayed, pfZEllipsoidRDelayed: Single;
                        out   pfEllipsoidYawDelayed, pfEllipsoidPitchDelayed: Single;
                        out   piSacDurationDelayed, piFixDurationDelayed: Integer):Integer; stdcall;


var EyeMotionState: Integer;




begin

  try

    if not Assigned(FFixFuncClass3dCalcByEllipsoid)then
      Result:=-1
    else
    begin

    EyeMotionState:=-1;

    pfXGazeDelayed:=0;
    pfYGazeDelayed:=0;
    pfZGazeDelayed:=0;

    // Übergabe des 3D-Samples an den 3D-Fixations-Alg.
    EyeMotionState:=FFixFuncClass3dCalcByEllipsoid.DetectFixation(  bValidSample,
                                                                    fXLeftEye, fYLeftEye, fZLeftEye,
                                                                    fXRightEye, fYRightEye, fZRightEye,
                                                                    fXGaze, fYGaze, fZGaze,
                                                                    fAccuracyAngleRad, iMinimumSamples,

                                                                    pbGazepointFoundDelayed,
                                                                    pfXGazeDelayed, pfYGazeDelayed, pfZGazeDelayed,
                                                                    pfXFixDelayed, pfYFixDelayed, pfZFixDelayed,
                                                                    pfXEllipsoidRDelayed, pfYEllipsoidRDelayed, pfZEllipsoidRDelayed,
                                                                    pfEllipsoidYawDelayed, pfEllipsoidPitchDelayed,
                                                                    piSacDurationDelayed, piFixDurationDelayed);

    Result:=EyeMotionState;

    end;


  except
    on e:Exception do
    raise Exception.CreateFmt('Calculate3DFixation: [%s]',[e.message]);
  end;

end;



exports
  Init3DFixation,
  Calculate3DFixation;



begin

end.
