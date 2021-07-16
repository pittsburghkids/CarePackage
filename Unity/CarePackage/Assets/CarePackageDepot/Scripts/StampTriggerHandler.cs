using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StampTriggerHandler : MonoBehaviour, ITriggerHandler
{
    [SerializeField] Animator animator = default;
    [SerializeField] AnimatorBridge animationEventBridge = default;

    private Box currentBox;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animationEventBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
            audioSource.PlayOneShot(audioSource.clip);
            currentBox?.ShowDecal();
        };
    }

    public void OnTriggerEnter(Collider collider)
    {
        animator.SetTrigger("Activate");

        Box box = collider.gameObject.GetComponent<Box>();
        currentBox = box;
    }

    public void OnTriggerExit(Collider collider)
    {
    }
}
