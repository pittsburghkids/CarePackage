using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampTriggerHandler : MonoBehaviour, ITriggerHandler
{
    [SerializeField] Animator animator = default;
    [SerializeField] AnimationEventBridge animationEventBridge = default;

    private Box lastBox;

    void Start()
    {
        animationEventBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
            lastBox?.ShowDecal();
        };
    }

    public void OnTriggerEnter(Collider collider)
    {
        animator.SetTrigger("Activate");

        Box box = collider.gameObject.GetComponent<Box>();
        lastBox = box;
    }

    public void OnTriggerExit(Collider collider)
    {
    }
}
