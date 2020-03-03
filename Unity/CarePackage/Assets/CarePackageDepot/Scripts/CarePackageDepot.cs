using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarePackageDepot : MonoBehaviour
{

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject packagePrefab = default;
    [SerializeField] GameObject moverPrefab = default;
    [SerializeField] Animator depotAnimator = default;

    private CarePackage carePackage;

    private Queue<string> deliveryQueue = new Queue<string>();
    float lastDelivery = 0;

    // Singleton creation.
    private static CarePackageDepot instance;
    public static CarePackageDepot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CarePackageDepot>();
            }

            return instance;
        }
    }

    void Start()
    {
        carePackage = CarePackage.Instance;
        carePackage.OnData += OnCarePackageData;
    }

    void Update()
    {
        if (Time.time - lastDelivery > 1)
        {
            if (deliveryQueue.Count > 0)
            {
                string boxName = deliveryQueue.Dequeue();

                if (boxName != null)
                {
                    GameObject packageInstance = Instantiate(packagePrefab);
                    packageInstance.name = boxName;

                    GameObject moverInstance = Instantiate(moverPrefab, moverContainer);
                    Mover mover = moverInstance.GetComponent<Mover>();
                    mover.GetPackage(packageInstance);
                }

                lastDelivery = Time.time;
            }
        }

    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        // Box found.
        Debug.Log(carePackageData.itemName);

        if (carePackageData.boardName == "DepotBoxA" || carePackageData.boardName == "DepotBoxB")
        {
            if (carePackageData.type == "tag.found" && carePackageData.boxName != null)
            {
                Delivery(carePackageData.boxName);
            }
        }

        // Item inserted into box.
        if (carePackageData.type == "loader.insert" && carePackageData.itemName != null && carePackageData.boxName != null)
        {
            carePackage.Store(carePackageData.boxName, carePackageData.itemName);
        }
    }

    public void Delivery(string message)
    {
        deliveryQueue.Enqueue(message);
    }

    public void OpenDoor()
    {
        depotAnimator.SetBool("Open", true);
    }

    public void CloseDoor()
    {
        depotAnimator.SetBool("Open", false);
    }

}
