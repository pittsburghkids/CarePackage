using UnityEngine;
using UnityEngine.Events;



public class AnimationEventBridge : MonoBehaviour
{
    public delegate void AnimationBridgeEventDelegate(AnimationEvent animationEvent);
    public event AnimationBridgeEventDelegate OnAnimationBridgeEvent;

    public void OnAnimationEvent(AnimationEvent animationEvent)
    {
        OnAnimationBridgeEvent?.Invoke(animationEvent);
    }
}
