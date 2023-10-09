using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partsPool : MonoBehaviour
{
    [SerializeField] Transform _parentTransform;
    [SerializeField] List<SnapBehavior> _parts;

    private void Awake()
    {
        AddObjectsToList();
    }

    private void AddObjectsToList()
    {
        foreach(Transform child in _parentTransform)
        {
            _parts.Add(child.GetComponent<SnapBehavior>());
        }
        for(int i=0 ; i< _parts.Count; i++)
        {
            _parts[i].id = i;
        }
    }
}
