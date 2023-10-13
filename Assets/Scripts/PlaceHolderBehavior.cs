using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceHolderBehavior : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _redGhostMaterial;
    [SerializeField] private Material _greenGhostMaterial;

    //[SerializeField] private Renderer _renderer;

    public bool isAttached;
    public bool canAttach;
    private GhostType _type;
    [SerializeField] private Outline _outline;

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
        if (canAttach)
        {
            AttachableGhost();
            _outline.enabled = true;
        }
        else
        {
            NotAttachableGhost();
        }
        _meshRenderer.enabled = true; 
    }
    public void Deactivate()
    {
        _meshRenderer.enabled = false;
        _outline.enabled = false;
    }

    public void NotAttachableGhost()
    {
        if(_type == GhostType.RED) { return; }

        Material[] mats = new Material[_meshRenderer.sharedMaterials.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = _redGhostMaterial;
        }
        _meshRenderer.sharedMaterials = mats;
        _type = GhostType.RED;
    }

    public void AttachableGhost()
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


