using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderTriggerHandler : MonoBehaviour, ITriggerHandler
{
    [SerializeField] CarePackageLoader loader = default;


    public void OnTriggerEnter(Collider collider)
    {
        loader.StoreItemFromTrigger(collider);
    }

    public void OnTriggerExit(Collider collider)
    {
    }
}
