using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBridge : MonoBehaviour
{
    [Header("Lift")]
    [SerializeField] AudioClip liftUp;
    [SerializeField] AudioClip liftDown;
    [SerializeField] AnimatorBridge liftAnimatorBridge = default;
    [SerializeField] AudioSource liftAudioSource;

    [Header("Door")]
    [SerializeField] AudioClip doorOpen;
    [SerializeField] AudioClip doorClose;
    [SerializeField] AnimatorBridge doorAnimatorBridge = default;
    [SerializeField] AudioSource doorAudioSource;

    [Header("Grabber")]
    [SerializeField] AudioClip grabberGrab;
    [SerializeField] AnimatorBridge grabberAnimatorBridge = default;
    [SerializeField] AudioSource grabberAudioSource;

    void Start()
    {

        liftAnimatorBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
            if (animationEvent.stringParameter == "DepotLiftUp")
            {
                Play(liftAudioSource, liftUp);
            }
            else if (animationEvent.stringParameter == "DepotLiftDown")
            {
                Play(liftAudioSource, liftDown);
            }
        };

        grabberAnimatorBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
            if (animationEvent.stringParameter == "DepotGrabberGrab")
            {
                grabberAudioSource.pitch = Random.Range(.8f, 1f);
                Play(grabberAudioSource, grabberGrab);
            }
        };

        doorAnimatorBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
            if (animationEvent.stringParameter == "DepotDoorOpening")
            {
                Play(doorAudioSource, doorOpen);
            }
            else if (animationEvent.stringParameter == "DepotDoorClosing")
            {
                Play(doorAudioSource, doorClose);
            }
        };

    }

    public void Play(AudioSource audioSource, AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
