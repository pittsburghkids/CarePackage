using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{

    void DepotDoorOpening()
    {
        CarePackageDepot.Instance.DoorOpening();
    }

    void DepotDoorClosed()
    {
        CarePackageDepot.Instance.DoorClosed();
    }
}
