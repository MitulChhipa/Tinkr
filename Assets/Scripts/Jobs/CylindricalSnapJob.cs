using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct CylindricalSnapJob : IJob
{
    Vector3 _pos1;
    Vector3 _pos2;

    NativeArray<bool> _result;
    Quaternion _rot1;
    Quaternion _rot2;
    float _acceptableAngle;
    float _acceptableDistance;

    //Constructor for initializing variables 
    public CylindricalSnapJob(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, float acceptableDistance, float acceptableAngle, NativeArray<bool> result)
    {
        _pos1 = pos1;
        _pos2 = pos2;
        _result = result;
        _rot1 = rot1;
        _rot2 = rot2;
        _acceptableAngle = acceptableAngle;
        _acceptableDistance = acceptableDistance;
    }

    //Interface method
    void IJob.Execute()
    {
        if (TwoDimenAngleAccept(_rot1,_rot2) 
            && DistanceCheck())
        {
            _result[0] = true;
        }
        else 
        {
            _result[0] = false; 
        }
    }
    
    //Rotational angle checking for symetrical parts
    private bool TwoDimenAngleAccept(Quaternion x, Quaternion y)
    {
        return TwoDimenAngle(x, y) < _acceptableAngle;
    }

    //Gets rotational angle for symetrical parts
    private float TwoDimenAngle(Quaternion x, Quaternion y)
    {
        Vector3 a = x.eulerAngles;
        Vector3 b = y.eulerAngles;

        a.z = 0;
        b.z = 0;

        Quaternion Ax = Quaternion.Euler(a);
        Quaternion Ay = Quaternion.Euler(b);


        return Quaternion.Angle(Ax, Ay);
    }

    //for distance checking
    private bool DistanceCheck()
    {
        if (Vector3.Distance(_pos1, _pos2) < _acceptableDistance)
        {
            return true;
        }
        return false;
    }
}
