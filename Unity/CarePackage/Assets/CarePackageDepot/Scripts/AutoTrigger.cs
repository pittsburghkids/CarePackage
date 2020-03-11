using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AutoTriggerEvent : UnityEvent<Collider> { }

public class AutoTrigger : MonoBehaviour
{
    public AutoTriggerEvent OnAutoTriggerEnter;
    public AutoTriggerEvent OnAutoTriggerExit;

    private void OnTriggerEnter(Collider collider)
    {
        OnAutoTriggerEnter?.Invoke(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        OnAutoTriggerExit?.Invoke(collider);
    }
}