using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partsPool : MonoBehaviour
{
    [SerializeField] Transform _parentTransform;
    [SerializeField] List<SnapBehavior> _parts;
    [SerializeField] int _attachedPartsCount;

    public static partsPool instance;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
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
            _parts[i].index = i;
        }
    }
    
    public void PartAttached()
    {
        _attachedPartsCount++;
        if(_attachedPartsCount == _parts.Count - 1)
        {
            print("Won");
        }
    }
}
