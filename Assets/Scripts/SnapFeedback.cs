using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapFeedback : MonoBehaviour
{
    public static SnapFeedback instance;
    [SerializeField] private AudioSource _snapAudio;
    [SerializeField] private Transform _snapAudioTransform;
    [SerializeField] private ParticleSystem _snapParticleSystem;


    private void Awake()
    {
        instance = this;
    }


    public void PlaySnapFeedback(Transform x)
    {
        _snapAudioTransform.position = x.position;
        _snapAudio.Play();
        _snapParticleSystem.Emit(1);
        //OVRInput.SetControllerVibration(60, 0.1f, OVRInput.Controller.RTouch);
        //OVRInput.SetControllerVibration(60, 0.1f, OVRInput.Controller.LTouch);
    }
}
