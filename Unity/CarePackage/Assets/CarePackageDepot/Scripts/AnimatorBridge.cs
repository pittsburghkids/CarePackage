using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorBridge : MonoBehaviour
{
    public delegate void AnimationBridgeEventDelegate(AnimationEvent animationEvent);
    public event AnimationBridgeEventDelegate OnAnimationBridgeEvent;

    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    public void OnAnimationEvent(AnimationEvent animationEvent)
    {
        OnAnimationBridgeEvent?.Invoke(animationEvent);
    }
}
