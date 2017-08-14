unit u3DAlgorithm;

{
  Vector Unit
  ===========
    Sascha Weber 12.05.2016
    Algorithm for calculating the 3D gaze point with the Skewed Lines approach
    Algorithm for calculating the vergence angle between the gaze vectors

}


interface

uses Math, SysUtils, Dialogs, u3DVector, CodeSiteLogging;

function Calculate3dSVAlgorithm_SkewLines(pvGL,dvGL,pvGR,dvGR: TVector): TVector;
function CalculateGazeVectorAngle(vEyeLeft,vEyeRight: TVector): Double;

implementation

function Calculate3dSVAlgorithm_SkewLines(pvGL,dvGL,pvGR,dvGR: TVector): TVector;    // pvEi = gaze position vector (i.e. posittion of cornea center
                                                                                     // dvGi = gaze direction vector | i{L,R}
var xDepth,yDepth,zDepth      : Double;
    vDepth                    : TVector;
    relpos                    : Integer;
begin
  xDepth:=0;
  yDepth:=0;
  zDepth:=0;

  try

    relpos:=vCheckRelPos(pvGL,dvGL,pvGR,dvGR);

    case relpos of

    0: // ERROR
       zDepth:=0;

    1: // Skew Lines
       begin
         vDepth:=vSkewDistancePoint(pvGL,dvGL,pvGR,dvGR);
         xDepth:=vDepth.x1;
         yDepth:=vDepth.x2;
         zDepth:=vDepth.x3;
       end;

    2: // Intersection
       begin
         vDepth:=vIntersection(pvGL,dvGL,pvGR,dvGR);
         xDepth:=vDepth.x1;
         yDepth:=vDepth.x2;
         zDepth:=vDepth.x3;
       end;

    3: // Parallel
         zDepth:=0;

    end;

    Result.x1:=xDepth;
    Result.x2:=yDepth;
    Result.x3:=zDepth;

  except
    on e:Exception do
    raise Exception.CreateFmt('u3DAlgorithm: Failed Calculate3dSVAlgorithm_SkewLines: [%s]',[e.message]);
  end;
end;


function CalculateGazeVectorAngle(vEyeLeft,vEyeRight: TVector): Double;
begin
result:=0;

  try
    result:= RadToDeg (ArcCos( ((vEyeLeft.x1*vEyeRight.x1)+(vEyeLeft.x2*vEyeRight.x2)+(vEyeLeft.x3*vEyeRight.x3)) / (sqrt(sqr(vEyeLeft.x1)+sqr(vEyeLeft.x2)+sqr(vEyeLeft.x3))*sqrt(sqr(vEyeRight.x1)+sqr(vEyeRight.x2)+sqr(vEyeRight.x3))) ) );

  except
    on e:Exception do
    raise Exception.CreateFmt('u3DAlgorithm: Failed CalculateVergenceAngle: [%s]',[e.message]);
  end;
end;
end.
