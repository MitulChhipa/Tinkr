using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceHolderBehavior : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _blueGhostMaterial;
    [SerializeField] private Material _greenGhostMaterial;
    public bool isAttached;
    private GhostType _type;

    public void SetMesh(int x,Mesh mesh)
    {
        _meshFilter.sharedMesh = mesh;
        Material[] mats = new Material[x];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = _greenGhostMaterial;
        }

        _meshRenderer.sharedMaterials = mats;
        _type = GhostType.GREEN;
    }
    public void Activate()
    {
        _meshRenderer.enabled = true; 
    }
    public void Deactivate()
    {
        _meshRenderer.enabled = false;
    }

    public void BlueGhost()
    {
        if(_type == GhostType.RED) { return; }
        Material[] mats = new Material[_meshRenderer.sharedMaterials.Length];
        for(int i = 0; i < mats.Length; i++)
        {
            mats[i] = _blueGhostMaterial;
        }
        _meshRenderer.sharedMaterials = mats;
        _type = GhostType.RED;
    }

    public void GreenGhost()
    {
        if(_type == GhostType.GREEN) { return; }
        Material[] mats = new Material[_meshRenderer.sharedMaterials.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = _greenGhostMaterial;
        }
        _meshRenderer.sharedMaterials = mats;
        _type = GhostType.GREEN;
    }
    
}


