using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Unity.Mathematics;
using UnityEngine;

public class SnapBehavior : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private GameObject _emptyPrefab;
    [SerializeField]private bool _symmetrical;
    private Transform _transform;
    private Transform _placeholderTransform;
    
    private GameObject _placeholder;
    private PlaceHolderBehavior _placeholderBehavior;
    //public Material placeholderMaterial;
    public int id;
    public bool subpartsAttached;
    private bool _baseBody;
    private float _acceptableDistance = 0.2f;
    private float _acceptableAngle = 30f;

    private DistanceGrabInteractable _distanceInteractable;
    private DistanceHandGrabInteractable _distanceHandInteractable;
    private HandGrabInteractable _handInteractable;
    private GrabInteractable _interactable;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _distanceHandInteractable = GetComponent<DistanceHandGrabInteractable>();
        _distanceInteractable = GetComponent<DistanceGrabInteractable>();
        _handInteractable = GetComponent<HandGrabInteractable>();
        _interactable = GetComponent<GrabInteractable>();

        CreateGhostObjects();
        if(_transform.childCount > 0)
        {
            subpartsAttached = false;
            
        }
        else
        {
            subpartsAttached = true;
        }
        
    }

    public void CreateGhostObjects()
    {
        if(_targetTransform == null)
        {
            _baseBody = true;
            return;
        }
        _placeholder = Instantiate(_emptyPrefab,_targetTransform.position,_targetTransform.rotation, _targetTransform.parent);
        Destroy(_targetTransform.gameObject);
        _placeholder.name = gameObject.name + " Ghost";
        _placeholderTransform = _placeholder.GetComponent<Transform>();
        _placeholderBehavior = _placeholder.GetComponent<PlaceHolderBehavior>();
        MeshRenderer mrThis = gameObject.GetComponent<MeshRenderer>();
        

        _placeholderBehavior.SetMesh(mrThis.sharedMaterials.Length, _transform.gameObject.GetComponent<MeshFilter>().mesh);
        _placeholderBehavior.Deactivate();
    }

    public void SnapCheck()
    {
        if(_baseBody)
        {
            return;
        }
        if (_symmetrical)
        {
            if ((Vector3.Distance(_placeholderTransform.position, _transform.position) < _acceptableDistance)
             && subpartsAttached
             && TwoDimenAngle(_placeholderTransform.rotation,_transform.rotation)< _acceptableAngle)
            {
                Snap();
            }
        }
        else
        {
            if ((Vector3.Distance(_placeholderTransform.position, _transform.position) < _acceptableDistance)
             && subpartsAttached
             && Quaternion.Angle(_placeholderTransform.rotation, _transform.rotation) < _acceptableAngle)
            {
                Snap();
            }
        }
    }

    public void HideGhost()
    {
        if (_baseBody)
        {
            return;
        }
        _placeholderBehavior.Deactivate();
    }

    public void ShowGhost()
    {
        if(_baseBody)
        {
            return;
        }
        if (subpartsAttached)
        {
            _placeholderBehavior.GreenGhost();
            _placeholderBehavior.Activate();
        }
        else
        {
            _placeholderBehavior.BlueGhost();
            _placeholderBehavior.Activate();
        }
    }

    public void CheckAllSubpartsAttached()
    {
        foreach(Transform child in _transform)
        {
            if(child.GetComponent<PlaceHolderBehavior>().isAttached == false)
            {
                subpartsAttached = false;
                return;
            }
        }
        subpartsAttached = true;
    }

    private void Snap()
    {
        _placeholderBehavior.Deactivate();
        _transform.SetParent(_placeholderTransform);
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;
        _placeholderBehavior.isAttached = true;
        _placeholderTransform.parent.GetComponent<SnapBehavior>().CheckAllSubpartsAttached();
        
        _distanceInteractable.enabled = false;  
        _distanceHandInteractable.enabled = false;
        _handInteractable.enabled = false;
        _interactable.enabled = false;
    }

    private float TwoDimenAngle(Quaternion x, Quaternion y)
    {
        Vector3 a = x.eulerAngles;
        Vector3 b = y.eulerAngles;

        a.x = 0;
        b.x = 0;

        x = quaternion.Euler(a);
        y = quaternion.Euler(b);

        print(Quaternion.Angle(x, y));

        return Quaternion.Angle(x, y);
    }
}
