using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct NormalSnapJob : IJob
{
    Vector3 _pos1;
    Vector3 _pos2;

    NativeArray<bool> _result;
    Quaternion _rot1;
    Quaternion _rot2;
    float _acceptableAngle;
    float _acceptableDistance;

    //Constructor for initializing variables
    public NormalSnapJob(Vector3 pos1,Vector3 pos2,Quaternion rot1,Quaternion rot2, float acceptableDistance,float acceptableAngle, NativeArray<bool> result)
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
        if (DistanceCheck()
            && NormalRotationalCheck())
        {
            _result[0] = true;
        }
        else
        {
            _result[0] = false;
        }
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

    //Rotationnal angle checking for normal conditions
    private bool NormalRotationalCheck()
    {
        if (Quaternion.Angle(_rot1, _rot2) < _acceptableAngle)
        {
            return true;
        }
        return false;
    }
}
