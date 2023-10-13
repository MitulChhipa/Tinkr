using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingGrabbable : MonoBehaviour
{
    [SerializeField] private GameObject _interactableParent;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform[] _transformChild;

    private void Awake()
    {
        foreach(Transform child in _transformChild)
        {
            GameObject x = Instantiate(_interactableParent, child.position, child.rotation, transform);
            child.SetParent(x.transform);
            x.GetComponent<GrabInteractable>().enabled = true;
            x.GetComponent<DistanceGrabInteractable>().enabled = true;
            x.GetComponent<DistanceHandGrabInteractable>().enabled = true;
            x.GetComponent<HandGrabInteractable>().enabled = true;
            x.GetComponent<SnapBehavior>().SetTarget(child.GetComponent<TargetProvider>().target);
            x.GetComponent<SnapBehavior>().SetPrefab(child.gameObject);
        }
    }
}
