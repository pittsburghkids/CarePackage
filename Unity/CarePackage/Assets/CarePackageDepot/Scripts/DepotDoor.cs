using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DepotDoor : MonoBehaviour
{
    public Animator Animator
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    void DepotDoorOpening()
    {
        CarePackageDepot.Instance.DoorOpening();
    }

    void DepotDoorClosed()
    {
        CarePackageDepot.Instance.DoorClosed();
    }
}
