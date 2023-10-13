using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SnapBehavior : MonoBehaviour
{ 

    [SerializeField] private Transform _targetTransform;                                                     //Transform where the ghost is spawned
    [SerializeField] private GameObject _emptyPrefab;                                                        //Prefab of the ghost
    [SerializeField] private SnapType _snapType;                                                             //Snap Type according to which snap is decided
    [SerializeField] private List<PlaceHolderBehavior> _childPlaceHolder = new List<PlaceHolderBehavior>();  //Subparts list
    
    //Part transform
    private Transform _transform;
    
    //Acceptable snaping conditions
    private float _acceptableDistance;
    private float _acceptableAngle;
    
    //Inder of the part in pool
    public int index;
    
    //Bool for checking if this part is base body or not
    [SerializeField] private bool _baseBody;
    
    //Placeholder or Ghost part reference
    private GameObject _placeholder;
    private Transform _placeholderTransform;
    private PlaceHolderBehavior _placeholderBehavior;

    //All interactables attached to the part
    private DistanceGrabInteractable _distanceInteractable;
    private DistanceHandGrabInteractable _distanceHandInteractable;
    private HandGrabInteractable _handInteractable;
    private GrabInteractable _interactable;
    
    //Accessing outline script to enable outline on the part
    private Outline _outline;
    
    //Bool for checking interation like hover and select
    private bool _hovered;
    private bool _doubleHovered;
    private bool _selected;
    private bool _doubleSelected;

    

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _distanceHandInteractable = GetComponent<DistanceHandGrabInteractable>();
        _distanceInteractable = GetComponent<DistanceGrabInteractable>();
        _handInteractable = GetComponent<HandGrabInteractable>();
        _interactable = GetComponent<GrabInteractable>();
        _acceptableDistance = 0.03f;
        _acceptableAngle = 30f;
        _outline = GetComponent<Outline>();
        _outline.enabled = false;


        CreateGhostObjects();
        //Delaying the funtion to get subparts so that it dont give null reference error 
        Invoke("GetPlaceholderInThisPart", 0.1f);
        
        PointableUnityEventWrapper pointableUnityEventWrapper = GetComponent<PointableUnityEventWrapper>();
        
        pointableUnityEventWrapper.WhenHover.AddListener((x) => OnHover());
        pointableUnityEventWrapper.WhenUnhover.AddListener((x) => UnHover());
        if (_baseBody)
        {
            return;
        }
        pointableUnityEventWrapper.WhenSelect.AddListener((x) => OnSelect());
        pointableUnityEventWrapper.WhenUnselect.AddListener((x) => UnSelect());
    }

    //Instantiating ghost objects
    public void CreateGhostObjects()
    {
        if (_targetTransform == null)
        {
            _baseBody = true;
            //Delaying the funtion of make child parts attachable for the base body
            Invoke("CanAttachParts", 0.5f);
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

    //Checking all snap conditions using jobs
    public void SnapCheck()
    {
        if (!_placeholderBehavior.canAttach)
        {
            return;
        }

        JobHandle _jobHandle = new JobHandle();
        NativeArray<bool> _jobResult = new NativeArray<bool>(1, Allocator.Persistent);
        
        switch (_snapType)
        {
            case SnapType.NORMAL:
            
                NormalSnapJob _normalJob = new NormalSnapJob(_placeholderTransform.position,_transform.position,_placeholderTransform.rotation,_transform.rotation,_acceptableDistance,_acceptableAngle,_jobResult);
                _jobHandle = _normalJob.Schedule();
          
            break;

            case SnapType.CYLINDRICAL:

                CylindricalSnapJob _cylindricalJob = new CylindricalSnapJob(_placeholderTransform.position, _transform.position, _placeholderTransform.rotation, _transform.rotation, _acceptableDistance, _acceptableAngle, _jobResult);
                _jobHandle = _cylindricalJob.Schedule();
           
            break;
            
            case SnapType.BIDIRECIONAL:

                BidriectionalSnapJob _bidriectionalJob = new BidriectionalSnapJob(_placeholderTransform.position, _transform.position, _placeholderTransform.rotation, _transform.rotation, _acceptableDistance, _acceptableAngle, _jobResult);
                _jobHandle = _bidriectionalJob.Schedule();
            
            break;
        }

        _jobHandle.Complete();

        if (_jobResult[0] == true)
        {
            Snap();
        }

        _jobResult.Dispose();
    
    }

    //Hiding ghost
    public void HideGhost()
    {
        if (_baseBody)
        {
            return;
        }
        _placeholderBehavior.Deactivate();
    }

    //Activation ghost based on subparts attached or not
    public void ShowGhost()
    {
        if (_baseBody)
        {
            return;
        }
        _placeholderBehavior.Activate();
    }

  

    //Snapping parts
    private void Snap()
    {
        _placeholderBehavior.Deactivate();
        _transform.SetParent(_placeholderTransform);
        _transform.localPosition = Vector3.zero;
        _transform.localRotation = Quaternion.identity;
        _placeholderBehavior.isAttached = true;
        SnapFeedback.instance.PlaySnapFeedback(_transform);

        CanAttachParts();
       
        _distanceInteractable.enabled = false;
        _distanceHandInteractable.enabled = false;
        _handInteractable.enabled = false;
        _interactable.enabled = false;
        partsPool.instance.PartAttached();
        HideOutline();
    }

    //Making subparts attachable
    private void CanAttachParts()
    {
        foreach(PlaceHolderBehavior child in _childPlaceHolder)
        {
            child.canAttach = true;
        }
    }

    //Caching all placeholderbehavior child
    private void GetPlaceholderInThisPart()
    {
        foreach (Transform child in _transform)
        {
            if(child != null)
            {
                _childPlaceHolder.Add(child.GetComponent<PlaceHolderBehavior>());
            }
        }
    }

    private void ShowOutline()
    {
        _outline.enabled = true;
    }
    
    private void HideOutline()
    {
        _outline.enabled = false;
    }

    private void OnHover()
    {
        if (!_hovered)
        {
            _hovered = true;
        }
        else
        {
            _doubleHovered = true;
        }
        ShowOutline();
    }

    private void UnHover()
    {
        if (_doubleHovered)
        {
            _doubleHovered = false;
            return;
        }
        else
        {
            _hovered = false;
            HideOutline();
        }
    }

    private void OnSelect()
    {
        if (!_selected)
        {
            _selected = true;
        }
        else
        {
            _doubleSelected = true;
            return;
        }
        ShowGhost();
        HideOutline();
    }

    private void UnSelect()
    {
        if (_doubleSelected)
        {
            _doubleSelected = false;
            return;
        }
        else
        {
            _selected = false;
            HideGhost();
            SnapCheck();
        }
    }
}
