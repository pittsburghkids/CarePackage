using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarePackageDepot : MonoBehaviour
{
    [SerializeField] GameObject packagePrefab;

    private SerialReader slotReaderA;
    private SerialReader slotReaderB;

    void Start()
    {
        CarePackage carePackage = CarePackage.Instance;

        slotReaderA = new SerialReader(carePackage.carePackageConfig.depotSlotA);
        slotReaderB = new SerialReader(carePackage.carePackageConfig.depotSlotB);

        slotReaderA.OnSerialMessage += Delivery;
        slotReaderB.OnSerialMessage += Delivery;
    }

    void OnDestroy()
    {
        slotReaderA.OnSerialMessage -= Delivery;
        slotReaderB.OnSerialMessage += Delivery;

        slotReaderA?.Destroy();
        slotReaderB?.Destroy();
    }

    public void Delivery(string message)
    {
        if (message.StartsWith("1794"))
        {
            Instantiate(packagePrefab);
        }
    }
}
