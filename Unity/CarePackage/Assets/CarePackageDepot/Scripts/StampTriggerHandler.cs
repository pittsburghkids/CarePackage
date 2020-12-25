using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampTriggerHandler : MonoBehaviour, ITriggerHandler
{
    [SerializeField] Animator animator = default;
    [SerializeField] AnimatorBridge animationEventBridge = default;

    private Box currentBox;

    void Start()
    {
        animationEventBridge.OnAnimationBridgeEvent += (AnimationEvent animationEvent) =>
        {
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
