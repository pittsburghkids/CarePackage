using UnityEngine;

public class RejectTriggerHandler : MonoBehaviour, ITriggerHandler
{
    public void OnTriggerEnter(Collider collider)
    {
        Box box = collider.gameObject.GetComponent<Box>();
        CarePackageDepot.Instance.DestroyBox(box);
    }

    public void OnTriggerExit(Collider collider)
    {
    }

}