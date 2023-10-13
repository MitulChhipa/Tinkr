using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangingModels : MonoBehaviour
{
    [SerializeField] private Transform[] _models;
    [SerializeField] private Transform _centerPoint;

    [Range(0.0f,3.0f)]
    [SerializeField] private float _curveValue;
    Vector3 _deltaPos = new Vector3(0,0,0);

    private void Start()
    {
        foreach (Transform model in _models)
        {
            float x = Vector3.Distance(model.position, _centerPoint.position);
            _deltaPos.z = -x/ (4.5f - _curveValue);
            model.position = model.position + _deltaPos;
        }
    }
}
